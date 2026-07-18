using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace verii_metivon_api.Core.Paging;

/// <summary>
/// Applies database-side ordering from a paged request. Only real entity
/// properties or explicitly supplied aliases are accepted, so the client can
/// never inject an arbitrary dynamic LINQ expression.
/// </summary>
public static class QueryableSortingExtensions
{
    /// <summary>
    /// Applies CRM-compatible advanced filters in the database. A filter may
    /// target only a real public property path or an explicitly supplied alias.
    /// Unknown columns and invalid values are ignored instead of becoming
    /// executable dynamic expressions.
    /// </summary>
    public static IQueryable<TEntity> ApplyPagedFilters<TEntity>(
        this IQueryable<TEntity> source,
        PagedQuery query,
        IReadOnlyDictionary<string, string>? aliases = null)
    {
        if (query.Filters.Count == 0) return source;

        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        Expression? combined = null;

        foreach (var filter in query.Filters.Take(50))
        {
            if (string.IsNullOrWhiteSpace(filter.Column) || string.IsNullOrWhiteSpace(filter.Value)) continue;

            var requested = filter.Column.Trim();
            var propertyPath = aliases?.FirstOrDefault(x =>
                string.Equals(x.Key, requested, StringComparison.OrdinalIgnoreCase)).Value;
            propertyPath ??= ResolvePropertyPath(typeof(TEntity), requested);
            if (propertyPath is null) continue;

            Expression member = parameter;
            foreach (var segment in propertyPath.Split('.')) member = Expression.Property(member, segment);

            var predicate = BuildFilterExpression(member, filter.Operator, filter.Value);
            if (predicate is null) continue;
            combined = combined is null
                ? predicate
                : query.UseOrFilterLogic
                    ? Expression.OrElse(combined, predicate)
                    : Expression.AndAlso(combined, predicate);
        }

        return combined is null
            ? source
            : source.Where(Expression.Lambda<Func<TEntity, bool>>(combined, parameter));
    }

    public static IOrderedQueryable<TEntity> ApplyPagedSort<TEntity>(
        this IQueryable<TEntity> source,
        PagedQuery query,
        string defaultProperty,
        bool defaultDescending = false,
        IReadOnlyDictionary<string, string>? aliases = null)
    {
        var requested = query.SortBy?.Trim() ?? string.Empty;
        var propertyPath = aliases?.FirstOrDefault(x =>
            string.Equals(x.Key, requested, StringComparison.OrdinalIgnoreCase)).Value;

        propertyPath ??= ResolvePropertyPath(typeof(TEntity), requested);
        propertyPath ??= ResolvePropertyPath(typeof(TEntity), defaultProperty)
            ?? throw new InvalidOperationException($"Sort property '{defaultProperty}' does not exist on {typeof(TEntity).Name}.");

        var descending = string.IsNullOrWhiteSpace(requested)
            ? defaultDescending
            : query.IsDescending;

        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        Expression body = parameter;
        foreach (var segment in propertyPath.Split('.'))
            body = Expression.Property(body, segment);

        var lambda = Expression.Lambda(body, parameter);
        var method = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        var call = Expression.Call(
            typeof(Queryable),
            method,
            [typeof(TEntity), body.Type],
            source.Expression,
            Expression.Quote(lambda));

        return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(call);
    }

    private static string? ResolvePropertyPath(Type rootType, string requested)
    {
        if (string.IsNullOrWhiteSpace(requested)) return null;
        var current = rootType;
        var resolved = new List<string>();
        foreach (var segment in requested.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var property = current.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => string.Equals(x.Name, segment, StringComparison.OrdinalIgnoreCase));
            if (property is null) return null;
            resolved.Add(property.Name);
            current = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        }
        return string.Join('.', resolved);
    }

    private static Expression? BuildFilterExpression(Expression member, string? requestedOperator, string rawValue)
    {
        var operatorName = (requestedOperator ?? "Equals").Trim().ToLowerInvariant();
        var nullableType = Nullable.GetUnderlyingType(member.Type);
        var valueType = nullableType ?? member.Type;

        if (valueType == typeof(string))
        {
            var value = Expression.Constant(rawValue.Trim(), typeof(string));
            var notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
            Expression comparison = operatorName switch
            {
                "contains" => Expression.Call(member, nameof(string.Contains), Type.EmptyTypes, value),
                "startswith" => Expression.Call(member, nameof(string.StartsWith), Type.EmptyTypes, value),
                "endswith" => Expression.Call(member, nameof(string.EndsWith), Type.EmptyTypes, value),
                "equals" or "eq" or "=" => Expression.Equal(member, value),
                _ => Expression.Call(member, nameof(string.Contains), Type.EmptyTypes, value)
            };
            return Expression.AndAlso(notNull, comparison);
        }

        if (!TryConvert(rawValue, valueType, out var converted)) return null;

        Expression comparableMember = member;
        Expression? hasValue = null;
        if (nullableType is not null)
        {
            hasValue = Expression.Property(member, nameof(Nullable<int>.HasValue));
            comparableMember = Expression.Property(member, nameof(Nullable<int>.Value));
        }

        var constant = Expression.Constant(converted, valueType);
        Expression comparisonExpression;
        try
        {
            comparisonExpression = operatorName switch
            {
                ">" or "gt" => Expression.GreaterThan(comparableMember, constant),
                ">=" or "gte" => Expression.GreaterThanOrEqual(comparableMember, constant),
                "<" or "lt" => Expression.LessThan(comparableMember, constant),
                "<=" or "lte" => Expression.LessThanOrEqual(comparableMember, constant),
                "equals" or "eq" or "=" => Expression.Equal(comparableMember, constant),
                _ => Expression.Equal(comparableMember, constant)
            };
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        return hasValue is null ? comparisonExpression : Expression.AndAlso(hasValue, comparisonExpression);
    }

    private static bool TryConvert(string value, Type destinationType, out object? converted)
    {
        converted = null;
        try
        {
            if (destinationType == typeof(DateOnly)) converted = DateOnly.Parse(value, CultureInfo.InvariantCulture);
            else if (destinationType == typeof(TimeOnly)) converted = TimeOnly.Parse(value, CultureInfo.InvariantCulture);
            else if (destinationType == typeof(DateTime)) converted = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            else if (destinationType == typeof(Guid)) converted = Guid.Parse(value);
            else if (destinationType.IsEnum) converted = Enum.Parse(destinationType, value, true);
            else
            {
                var converter = TypeDescriptor.GetConverter(destinationType);
                if (!converter.CanConvertFrom(typeof(string))) return false;
                converted = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
            }
            return converted is not null;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or NotSupportedException)
        {
            return false;
        }
    }
}
