using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace verii_metivon_api.Core.Paging;

public sealed class PagedQueryModelBinder : IModelBinder
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    static PagedQueryModelBinder()
    {
        JsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var request = bindingContext.HttpContext.Request;
        if (!Microsoft.AspNetCore.Http.HttpMethods.IsGet(request.Method)
            && !Microsoft.AspNetCore.Http.HttpMethods.IsHead(request.Method)
            && request.HasJsonContentType())
        {
            await BindJsonBodyAsync(bindingContext);
            return;
        }

        BindQuery(bindingContext);
    }

    private static async Task BindJsonBodyAsync(ModelBindingContext context)
    {
        var request = context.HttpContext.Request;
        request.EnableBuffering();
        request.Body.Position = 0;

        try
        {
            var model = await JsonSerializer.DeserializeAsync(
                request.Body,
                context.ModelType,
                JsonOptions,
                context.HttpContext.RequestAborted);
            request.Body.Position = 0;

            if (model is null)
            {
                context.ModelState.TryAddModelError(context.ModelName, "Paged query body is required.");
                context.Result = ModelBindingResult.Failed();
                return;
            }

            context.Result = ModelBindingResult.Success(model);
        }
        catch (JsonException exception)
        {
            request.Body.Position = 0;
            context.ModelState.TryAddModelError(context.ModelName, exception.Message);
            context.Result = ModelBindingResult.Failed();
        }
    }

    private static void BindQuery(ModelBindingContext context)
    {
        var model = Activator.CreateInstance(context.ModelType);
        if (model is null)
        {
            context.Result = ModelBindingResult.Failed();
            return;
        }

        foreach (var property in context.ModelType.GetProperties()
                     .Where(property => property.SetMethod is not null))
        {
            if (!context.HttpContext.Request.Query.TryGetValue(property.Name, out var values)
                || values.Count == 0)
            {
                continue;
            }

            try
            {
                var value = ConvertValue(
                    values.Select(value => value ?? string.Empty).ToArray(),
                    property.PropertyType);
                property.SetValue(model, value);
            }
            catch (Exception exception) when (exception is FormatException or InvalidCastException or ArgumentException or NotSupportedException)
            {
                context.ModelState.TryAddModelError(property.Name, exception.Message);
            }
        }

        context.Result = ModelBindingResult.Success(model);
    }

    private static object? ConvertValue(string[] values, Type destinationType)
    {
        var underlyingType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
        if (values.Length == 0 || string.IsNullOrWhiteSpace(values[0]))
        {
            return Nullable.GetUnderlyingType(destinationType) is not null || destinationType == typeof(string)
                ? null
                : Activator.CreateInstance(destinationType);
        }

        if (underlyingType == typeof(string)) return values[0];
        if (underlyingType == typeof(DateOnly)) return DateOnly.Parse(values[0], CultureInfo.InvariantCulture);
        if (underlyingType == typeof(TimeOnly)) return TimeOnly.Parse(values[0], CultureInfo.InvariantCulture);
        if (underlyingType == typeof(Guid)) return Guid.Parse(values[0]);
        if (underlyingType.IsEnum) return Enum.Parse(underlyingType, values[0], true);

        if (destinationType.IsArray)
        {
            var elementType = destinationType.GetElementType()!;
            var array = Array.CreateInstance(elementType, values.Length);
            for (var index = 0; index < values.Length; index++)
            {
                array.SetValue(ConvertValue([values[index]], elementType), index);
            }
            return array;
        }

        var converter = TypeDescriptor.GetConverter(underlyingType);
        if (converter.CanConvertFrom(typeof(string)))
        {
            return converter.ConvertFrom(null, CultureInfo.InvariantCulture, values[0]);
        }

        throw new NotSupportedException($"Query value cannot be converted to {underlyingType.Name}.");
    }
}
