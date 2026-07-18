using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.Parameters.Domain.Entities;

namespace verii_metivon_api.Modules.Parameters.Application;

public sealed record BusinessPartnerParametersDto(
    long? BranchId,
    bool IsAutomatic,
    bool AllowManual,
    string Format,
    long NextNumber,
    int IncrementBy,
    long MinimumNumber,
    long MaximumNumber,
    bool IsContinuous,
    bool ForceUppercase,
    bool TrimWhitespace,
    bool RequireTaxNumber,
    bool RequireTaxOffice,
    bool RequireEmail,
    bool RequirePhone,
    bool RequireNationalIdentityNumber,
    bool PreventDuplicateTaxNumber,
    bool PreventDuplicateNationalIdentityNumber,
    bool PreventDuplicateEmail,
    long? DefaultBusinessPartnerTypeId,
    long? DefaultCustomerGroupId,
    long? DefaultPaymentTermId,
    long? DefaultCurrencyId,
    long? DefaultTaxGroupId,
    decimal DefaultCreditLimit,
    bool DefaultUnlimitedCredit,
    bool CreateActiveByDefault,
    string Preview);

public sealed record SaveBusinessPartnerParametersRequest(
    long? BranchId,
    bool IsAutomatic,
    bool AllowManual,
    string Format,
    long NextNumber,
    int IncrementBy,
    long MinimumNumber,
    long MaximumNumber,
    bool IsContinuous,
    bool ForceUppercase,
    bool TrimWhitespace,
    bool RequireTaxNumber,
    bool RequireTaxOffice,
    bool RequireEmail,
    bool RequirePhone,
    bool RequireNationalIdentityNumber,
    bool PreventDuplicateTaxNumber,
    bool PreventDuplicateNationalIdentityNumber,
    bool PreventDuplicateEmail,
    long? DefaultBusinessPartnerTypeId,
    long? DefaultCustomerGroupId,
    long? DefaultPaymentTermId,
    long? DefaultCurrencyId,
    long? DefaultTaxGroupId,
    decimal DefaultCreditLimit,
    bool DefaultUnlimitedCredit,
    bool CreateActiveByDefault);

public interface IParameterService
{
    Task<BusinessPartnerParametersDto> GetBusinessPartnerParametersAsync(long? branchId, CancellationToken ct);
    Task<BusinessPartnerParametersDto> SaveBusinessPartnerParametersAsync(SaveBusinessPartnerParametersRequest request, CancellationToken ct);
    Task<string?> ResolveBusinessPartnerCodeAsync(string? requestedCode, long branchId, long partnerTypeId, CancellationToken ct);
}

public sealed class ParameterService(MetivonDbContext db) : IParameterService
{
    private const string Module = "BusinessPartners";
    private const string Reference = "BusinessPartnerCode";
    private static readonly Regex NumberToken = new(@"\{NUMBER(?::(?<length>\d{1,2}))?\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public async Task<BusinessPartnerParametersDto> GetBusinessPartnerParametersAsync(long? branchId, CancellationToken ct)
    {
        var sequence = await ResolveSequence(branchId, ct);
        var values = await ResolveValues(branchId, ct);
        var branch = branchId.HasValue ? await db.Branches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == branchId, ct) : null;
        return Map(sequence, values, branch?.Code ?? "0", "CUSTOMER");
    }

    public async Task<BusinessPartnerParametersDto> SaveBusinessPartnerParametersAsync(SaveBusinessPartnerParametersRequest request, CancellationToken ct)
    {
        Validate(request);
        await ValidateReferences(request, ct);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var sequence = await db.Set<NumberSequence>().FirstOrDefaultAsync(
            x => x.Module == Module && x.Reference == Reference && x.BranchId == request.BranchId, ct);
        if (sequence is null)
        {
            sequence = new NumberSequence { Module = Module, Reference = Reference, BranchId = request.BranchId };
            db.Add(sequence);
        }

        sequence.Format = request.Format.Trim();
        sequence.CurrentNumber = request.NextNumber;
        sequence.IncrementBy = request.IncrementBy;
        sequence.MinimumNumber = request.MinimumNumber;
        sequence.MaximumNumber = request.MaximumNumber;
        sequence.IsAutomatic = request.IsAutomatic;
        sequence.AllowManual = request.AllowManual;
        sequence.IsContinuous = request.IsContinuous;
        sequence.IsActive = true;

        var values = new (string Key, object? Value, string Type, int Order)[]
        {
            ("ForceUppercase", request.ForceUppercase, "Boolean", 10),
            ("TrimWhitespace", request.TrimWhitespace, "Boolean", 20),
            ("RequireTaxNumber", request.RequireTaxNumber, "Boolean", 30),
            ("RequireTaxOffice", request.RequireTaxOffice, "Boolean", 40),
            ("RequireEmail", request.RequireEmail, "Boolean", 50),
            ("RequirePhone", request.RequirePhone, "Boolean", 60),
            ("RequireNationalIdentityNumber", request.RequireNationalIdentityNumber, "Boolean", 70),
            ("PreventDuplicateTaxNumber", request.PreventDuplicateTaxNumber, "Boolean", 80),
            ("PreventDuplicateNationalIdentityNumber", request.PreventDuplicateNationalIdentityNumber, "Boolean", 90),
            ("PreventDuplicateEmail", request.PreventDuplicateEmail, "Boolean", 100),
            ("DefaultBusinessPartnerTypeId", request.DefaultBusinessPartnerTypeId, "Reference", 110),
            ("DefaultCustomerGroupId", request.DefaultCustomerGroupId, "Reference", 120),
            ("DefaultPaymentTermId", request.DefaultPaymentTermId, "Reference", 130),
            ("DefaultCurrencyId", request.DefaultCurrencyId, "Reference", 140),
            ("DefaultTaxGroupId", request.DefaultTaxGroupId, "Reference", 150),
            ("DefaultCreditLimit", request.DefaultCreditLimit, "Decimal", 160),
            ("DefaultUnlimitedCredit", request.DefaultUnlimitedCredit, "Boolean", 170),
            ("CreateActiveByDefault", request.CreateActiveByDefault, "Boolean", 180)
        };
        foreach (var value in values)
            await UpsertValue(value.Key, Serialize(value.Value), value.Type, request.BranchId, value.Order, ct);

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return await GetBusinessPartnerParametersAsync(request.BranchId, ct);
    }

    public async Task<string?> ResolveBusinessPartnerCodeAsync(string? requestedCode, long branchId, long partnerTypeId, CancellationToken ct)
    {
        var sequence = await ResolveSequence(branchId, ct);
        var values = await ResolveValues(branchId, ct);
        var manual = Normalize(requestedCode, values);
        if (!string.IsNullOrWhiteSpace(manual))
        {
            if (sequence.IsAutomatic && !sequence.AllowManual)
                throw new InvalidOperationException("Manual business partner code is not allowed by parameters.");
            return manual;
        }
        if (!sequence.IsAutomatic) return null;
        var branch = await db.Branches.AsNoTracking().FirstAsync(x => x.Id == branchId, ct);
        var type = await db.BusinessPartnerTypes.AsNoTracking().FirstAsync(x => x.Id == partnerTypeId, ct);
        var number = await Consume(sequence, ct);
        return Normalize(Format(sequence.Format, number, branch.Code, type.Code), values);
    }

    private async Task<NumberSequence> ResolveSequence(long? branchId, CancellationToken ct) =>
        await db.Set<NumberSequence>().AsNoTracking()
            .Where(x => x.Module == Module && x.Reference == Reference && x.IsActive && (x.BranchId == branchId || x.BranchId == null))
            .OrderByDescending(x => x.BranchId.HasValue)
            .FirstOrDefaultAsync(ct)
        ?? new NumberSequence
        {
            Module = Module, Reference = Reference, Format = "CAR-{NUMBER:6}", CurrentNumber = 1,
            IncrementBy = 1, MinimumNumber = 1, MaximumNumber = 999999, IsAutomatic = true, AllowManual = true
        };

    private async Task<Dictionary<string, string>> ResolveValues(long? branchId, CancellationToken ct)
    {
        var rows = await db.Set<ErpParameter>().AsNoTracking()
            .Where(x => x.Module == Module && (x.BranchId == branchId || x.BranchId == null))
            .OrderByDescending(x => x.BranchId.HasValue)
            .ToListAsync(ct);
        return rows.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First().Value, StringComparer.OrdinalIgnoreCase);
    }

    private static string? Normalize(string? value, IReadOnlyDictionary<string, string> values)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var result = GetBool(values, "TrimWhitespace", true) ? value.Trim() : value;
        return GetBool(values, "ForceUppercase", true) ? result.ToUpperInvariant() : result;
    }

    private static bool GetBool(IReadOnlyDictionary<string, string> values, string key, bool fallback) =>
        values.TryGetValue(key, out var value) && bool.TryParse(value, out var parsed) ? parsed : fallback;

    private static long? GetLong(IReadOnlyDictionary<string, string> values, string key) =>
        values.TryGetValue(key, out var value) && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;

    private static decimal GetDecimal(IReadOnlyDictionary<string, string> values, string key, decimal fallback) =>
        values.TryGetValue(key, out var value) && decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) ? parsed : fallback;

    private static string Format(string format, long number, string branch, string type)
    {
        var result = format.Replace("{BRANCH}", branch, StringComparison.OrdinalIgnoreCase)
            .Replace("{TYPE}", type, StringComparison.OrdinalIgnoreCase)
            .Replace("{YYYY}", DateTime.UtcNow.ToString("yyyy"), StringComparison.OrdinalIgnoreCase)
            .Replace("{YY}", DateTime.UtcNow.ToString("yy"), StringComparison.OrdinalIgnoreCase)
            .Replace("{MM}", DateTime.UtcNow.ToString("MM"), StringComparison.OrdinalIgnoreCase);
        return NumberToken.Replace(result, match => number.ToString().PadLeft(
            Math.Clamp(int.TryParse(match.Groups["length"].Value, out var length) ? length : 1, 1, 18), '0'));
    }

    private static void Validate(SaveBusinessPartnerParametersRequest request)
    {
        if (!NumberToken.IsMatch(request.Format)) throw new ArgumentException("Format must contain a {NUMBER} or {NUMBER:n} token.");
        if (request.Format.Length > 120) throw new ArgumentException("Format cannot exceed 120 characters.");
        if (request.IncrementBy < 1 || request.MinimumNumber < 0 || request.NextNumber < request.MinimumNumber || request.MaximumNumber < request.NextNumber)
            throw new ArgumentException("Number range values are invalid.");
        if (request.DefaultCreditLimit < 0) throw new ArgumentException("Default credit limit cannot be negative.");
        if (Format(request.Format, request.NextNumber, "0", "CUSTOMER").Length > 50)
            throw new ArgumentException("Generated business partner code cannot exceed 50 characters.");
    }

    private async Task ValidateReferences(SaveBusinessPartnerParametersRequest request, CancellationToken ct)
    {
        if (request.DefaultBusinessPartnerTypeId.HasValue && !await db.BusinessPartnerTypes.AnyAsync(x => x.Id == request.DefaultBusinessPartnerTypeId && x.IsActive, ct))
            throw new ArgumentException("Selected default business partner type is not active.");
        if (request.DefaultCustomerGroupId.HasValue && !await db.CustomerGroups.AnyAsync(x => x.Id == request.DefaultCustomerGroupId && x.IsActive, ct))
            throw new ArgumentException("Selected default customer group is not active.");
        if (request.DefaultPaymentTermId.HasValue && !await db.PaymentTerms.AnyAsync(x => x.Id == request.DefaultPaymentTermId && x.IsActive, ct))
            throw new ArgumentException("Selected default payment term is not active.");
        if (request.DefaultCurrencyId.HasValue && !await db.Currencies.AnyAsync(x => x.Id == request.DefaultCurrencyId && x.IsActive, ct))
            throw new ArgumentException("Selected default currency is not active.");
        if (request.DefaultTaxGroupId.HasValue && !await db.TaxGroups.AnyAsync(x => x.Id == request.DefaultTaxGroupId && x.IsActive, ct))
            throw new ArgumentException("Selected default tax group is not active.");
    }

    private BusinessPartnerParametersDto Map(NumberSequence sequence, IReadOnlyDictionary<string, string> values, string branch, string type) =>
        new(
            sequence.BranchId, sequence.IsAutomatic, sequence.AllowManual, sequence.Format, sequence.CurrentNumber,
            sequence.IncrementBy, sequence.MinimumNumber, sequence.MaximumNumber, sequence.IsContinuous,
            GetBool(values, "ForceUppercase", true), GetBool(values, "TrimWhitespace", true),
            GetBool(values, "RequireTaxNumber", false), GetBool(values, "RequireTaxOffice", false),
            GetBool(values, "RequireEmail", false), GetBool(values, "RequirePhone", false),
            GetBool(values, "RequireNationalIdentityNumber", false), GetBool(values, "PreventDuplicateTaxNumber", true),
            GetBool(values, "PreventDuplicateNationalIdentityNumber", true), GetBool(values, "PreventDuplicateEmail", false),
            GetLong(values, "DefaultBusinessPartnerTypeId"), GetLong(values, "DefaultCustomerGroupId"),
            GetLong(values, "DefaultPaymentTermId"), GetLong(values, "DefaultCurrencyId"), GetLong(values, "DefaultTaxGroupId"),
            GetDecimal(values, "DefaultCreditLimit", 0), GetBool(values, "DefaultUnlimitedCredit", false),
            GetBool(values, "CreateActiveByDefault", true), Format(sequence.Format, sequence.CurrentNumber, branch, type));

    private async Task UpsertValue(string key, string value, string type, long? branchId, int order, CancellationToken ct)
    {
        var parameter = await db.Set<ErpParameter>().FirstOrDefaultAsync(x => x.Module == Module && x.Key == key && x.BranchId == branchId, ct);
        if (parameter is null)
        {
            parameter = new ErpParameter { Module = Module, Key = key, BranchId = branchId };
            db.Add(parameter);
        }
        parameter.Value = value;
        parameter.ValueType = type;
        parameter.DisplayOrder = order;
        parameter.IsEditable = true;
    }

    private static string Serialize(object? value) => value switch
    {
        null => string.Empty,
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty
    };

    private async Task<long> Consume(NumberSequence sequence, CancellationToken ct)
    {
        if (sequence.Id == 0) return sequence.CurrentNumber;
        if (sequence.CurrentNumber > sequence.MaximumNumber) throw new InvalidOperationException("Business partner number sequence is exhausted.");
        var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync(ct);
        await using var command = connection.CreateCommand();
        command.Transaction = db.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText = "UPDATE RII_NUMBER_SEQUENCES WITH (UPDLOCK, ROWLOCK) SET CurrentNumber = CurrentNumber + IncrementBy, UpdatedAt = SYSUTCDATETIME() OUTPUT DELETED.CurrentNumber WHERE Id = @id AND IsActive = 1 AND CurrentNumber <= MaximumNumber";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@id";
        parameter.Value = sequence.Id;
        command.Parameters.Add(parameter);
        var result = await command.ExecuteScalarAsync(ct);
        return result is null || result == DBNull.Value
            ? throw new InvalidOperationException("Business partner number sequence could not be consumed.")
            : Convert.ToInt64(result);
    }
}
