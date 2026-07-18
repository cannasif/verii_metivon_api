using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.UnitOfWork;

namespace verii_metivon_api.Modules.GeneralSettings;

public sealed class GeneralSetting : Entity
{
    public string ScopeKey { get; set; } = "GLOBAL";
    public string NumberFormat { get; set; } = "tr-TR";
    public int AmountDecimalPlaces { get; set; } = 2;
    public int PriceDecimalPlaces { get; set; } = 4;
    public int QuantityDecimalPlaces { get; set; } = 3;
    public int ExchangeRateDecimalPlaces { get; set; } = 6;
    public int PercentageDecimalPlaces { get; set; } = 2;
    public int CostDecimalPlaces { get; set; } = 6;
    public int RoundingMethod { get; set; } = 1;
    public bool UseThousandsSeparator { get; set; } = true;
    public bool TrimTrailingZeros { get; set; }
    public string DefaultCurrencyCode { get; set; } = "TRY";
    public int CurrencyDisplay { get; set; } = 1;
    public bool CurrencySymbolOnRight { get; set; } = true;
    public string DateFormat { get; set; } = "dd.MM.yyyy";
    public int TimeFormat { get; set; } = 24;
    public string TimeZoneId { get; set; } = "Europe/Istanbul";
    public int FirstDayOfWeek { get; set; } = 1;
    public bool StoreDateTimeAsUtc { get; set; } = true;
    public int DefaultPageSize { get; set; } = 20;
    public int MaximumExportRows { get; set; } = 10000;
    public bool AllowUserDisplayOverrides { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}

public sealed class GeneralSettingConfiguration : IEntityTypeConfiguration<GeneralSetting>
{
    public void Configure(EntityTypeBuilder<GeneralSetting> b)
    {
        b.ToTable("RII_GENERAL_SETTINGS", t =>
        {
            t.HasCheckConstraint("CK_RII_GENERAL_SETTINGS_DECIMALS", "[AmountDecimalPlaces] BETWEEN 0 AND 8 AND [PriceDecimalPlaces] BETWEEN 0 AND 8 AND [QuantityDecimalPlaces] BETWEEN 0 AND 8 AND [ExchangeRateDecimalPlaces] BETWEEN 0 AND 8 AND [PercentageDecimalPlaces] BETWEEN 0 AND 8 AND [CostDecimalPlaces] BETWEEN 0 AND 8");
            t.HasCheckConstraint("CK_RII_GENERAL_SETTINGS_LIMITS", "[DefaultPageSize] BETWEEN 10 AND 100 AND [MaximumExportRows] BETWEEN 100 AND 100000");
        });
        b.HasQueryFilter(x => !x.IsDeleted); b.HasIndex(x => x.ScopeKey).IsUnique().HasFilter("[IsDeleted] = 0");
        b.Property(x => x.ScopeKey).HasMaxLength(30).IsRequired(); b.Property(x => x.NumberFormat).HasMaxLength(20).IsRequired();
        b.Property(x => x.DefaultCurrencyCode).HasMaxLength(3).IsUnicode(false).IsRequired(); b.Property(x => x.DateFormat).HasMaxLength(30).IsRequired(); b.Property(x => x.TimeZoneId).HasMaxLength(100).IsRequired(); b.Property(x => x.RowVersion).IsRowVersion();
    }
}

public sealed record GeneralSettingsDto(string NumberFormat,int DecimalPlaces,int AmountDecimalPlaces,int PriceDecimalPlaces,int QuantityDecimalPlaces,int ExchangeRateDecimalPlaces,int PercentageDecimalPlaces,int CostDecimalPlaces,int RoundingMethod,bool UseThousandsSeparator,bool TrimTrailingZeros,string DefaultCurrencyCode,int CurrencyDisplay,bool CurrencySymbolOnRight,string DateFormat,int TimeFormat,string TimeZoneId,int FirstDayOfWeek,bool StoreDateTimeAsUtc,int DefaultPageSize,int MaximumExportRows,bool AllowUserDisplayOverrides,string NumberPreview,string CurrencyPreview,string DatePreview);
public sealed record SaveGeneralSettingsRequest(string NumberFormat,int AmountDecimalPlaces,int PriceDecimalPlaces,int QuantityDecimalPlaces,int ExchangeRateDecimalPlaces,int PercentageDecimalPlaces,int CostDecimalPlaces,int RoundingMethod,bool UseThousandsSeparator,bool TrimTrailingZeros,string DefaultCurrencyCode,int CurrencyDisplay,bool CurrencySymbolOnRight,string DateFormat,int TimeFormat,string TimeZoneId,int FirstDayOfWeek,bool StoreDateTimeAsUtc,int DefaultPageSize,int MaximumExportRows,bool AllowUserDisplayOverrides);

public interface IGeneralSettingsService { Task<ApiResponse<GeneralSettingsDto>> GetAsync(CancellationToken ct); Task<ApiResponse<GeneralSettingsDto>> SaveAsync(SaveGeneralSettingsRequest request,CancellationToken ct); }
public sealed class GeneralSettingsService(IUnitOfWork u) : IGeneralSettingsService
{
    static readonly HashSet<string> SupportedDateFormats = new(StringComparer.Ordinal) { "dd.MM.yyyy", "dd/MM/yyyy", "dd-MM-yyyy", "MM.dd.yyyy", "MM/dd/yyyy", "MM-dd-yyyy", "yyyy.MM.dd", "yyyy/MM/dd", "yyyy-MM-dd" };
    public async Task<ApiResponse<GeneralSettingsDto>> GetAsync(CancellationToken ct) { var entity=await u.Repository<GeneralSetting>().Query().FirstOrDefaultAsync(x=>x.ScopeKey=="GLOBAL",ct); return ApiResponse<GeneralSettingsDto>.Ok(Map(entity??new GeneralSetting())); }
    public async Task<ApiResponse<GeneralSettingsDto>> SaveAsync(SaveGeneralSettingsRequest r,CancellationToken ct)
    {
        Validate(r); var repo=u.Repository<GeneralSetting>(); var entity=await repo.FirstOrDefaultAsync(x=>x.ScopeKey=="GLOBAL",true,ct);
        if(entity is null){entity=new GeneralSetting();await repo.AddAsync(entity,ct);} Apply(entity,r);if(entity.Id>0)repo.Update(entity);await u.SaveChangesAsync(ct);return ApiResponse<GeneralSettingsDto>.Ok(Map(entity),"General settings saved.");
    }
    static void Validate(SaveGeneralSettingsRequest r){var decimals=new[]{r.AmountDecimalPlaces,r.PriceDecimalPlaces,r.QuantityDecimalPlaces,r.ExchangeRateDecimalPlaces,r.PercentageDecimalPlaces,r.CostDecimalPlaces};if(decimals.Any(x=>x is <0 or >8))throw new ArgumentException("Decimal places must be between 0 and 8.");if(r.DefaultCurrencyCode.Trim().Length!=3)throw new ArgumentException("Currency code must contain three characters.");if(!SupportedDateFormats.Contains(r.DateFormat.Trim()))throw new ArgumentException("Date format is invalid.");if(r.RoundingMethod is <0 or >2||r.CurrencyDisplay is <0 or >2||r.TimeFormat is not(12 or 24)||r.FirstDayOfWeek is <0 or >6||r.DefaultPageSize is <10 or >100||r.MaximumExportRows is <100 or >100000)throw new ArgumentException("One or more general setting values are invalid.");try{_=System.Globalization.CultureInfo.GetCultureInfo(r.NumberFormat.Trim());}catch{throw new ArgumentException("Number format is invalid.");}try{_=TimeZoneInfo.FindSystemTimeZoneById(r.TimeZoneId);}catch{throw new ArgumentException("Time zone is invalid.");}}
    static void Apply(GeneralSetting e,SaveGeneralSettingsRequest r){e.NumberFormat=r.NumberFormat.Trim();e.AmountDecimalPlaces=r.AmountDecimalPlaces;e.PriceDecimalPlaces=r.PriceDecimalPlaces;e.QuantityDecimalPlaces=r.QuantityDecimalPlaces;e.ExchangeRateDecimalPlaces=r.ExchangeRateDecimalPlaces;e.PercentageDecimalPlaces=r.PercentageDecimalPlaces;e.CostDecimalPlaces=r.CostDecimalPlaces;e.RoundingMethod=r.RoundingMethod;e.UseThousandsSeparator=r.UseThousandsSeparator;e.TrimTrailingZeros=r.TrimTrailingZeros;e.DefaultCurrencyCode=r.DefaultCurrencyCode.Trim().ToUpperInvariant();e.CurrencyDisplay=r.CurrencyDisplay;e.CurrencySymbolOnRight=r.CurrencySymbolOnRight;e.DateFormat=r.DateFormat.Trim();e.TimeFormat=r.TimeFormat;e.TimeZoneId=r.TimeZoneId.Trim();e.FirstDayOfWeek=r.FirstDayOfWeek;e.StoreDateTimeAsUtc=r.StoreDateTimeAsUtc;e.DefaultPageSize=r.DefaultPageSize;e.MaximumExportRows=r.MaximumExportRows;e.AllowUserDisplayOverrides=r.AllowUserDisplayOverrides;}
    static GeneralSettingsDto Map(GeneralSetting e)
    {
        var culture=System.Globalization.CultureInfo.GetCultureInfo(e.NumberFormat);
        var numberFormat=(System.Globalization.NumberFormatInfo)culture.NumberFormat.Clone();
        if(!e.UseThousandsSeparator) numberFormat.NumberGroupSeparator=string.Empty;
        var minimumDigits=e.TrimTrailingZeros?0:e.AmountDecimalPlaces;
        var number=1234567.89123.ToString($"N{e.AmountDecimalPlaces}",numberFormat);
        if(minimumDigits==0&&e.AmountDecimalPlaces>0) number=number.TrimEnd('0').TrimEnd(numberFormat.NumberDecimalSeparator.ToCharArray());
        var marker=e.CurrencyDisplay switch {0=>string.Empty,2=>e.DefaultCurrencyCode,_=>culture.NumberFormat.CurrencySymbol};
        var currency=e.CurrencyDisplay==0?number:e.CurrencySymbolOnRight?$"{number} {marker}":$"{marker} {number}";
        return new(e.NumberFormat,e.AmountDecimalPlaces,e.AmountDecimalPlaces,e.PriceDecimalPlaces,e.QuantityDecimalPlaces,e.ExchangeRateDecimalPlaces,e.PercentageDecimalPlaces,e.CostDecimalPlaces,e.RoundingMethod,e.UseThousandsSeparator,e.TrimTrailingZeros,e.DefaultCurrencyCode,e.CurrencyDisplay,e.CurrencySymbolOnRight,e.DateFormat,e.TimeFormat,e.TimeZoneId,e.FirstDayOfWeek,e.StoreDateTimeAsUtc,e.DefaultPageSize,e.MaximumExportRows,e.AllowUserDisplayOverrides,number,currency,DateTime.Today.ToString(e.DateFormat));
    }
}

[ApiController,Authorize,Route("api/general-settings")]
public sealed class GeneralSettingsController(IGeneralSettingsService service):ControllerBase
{
    [HttpGet]public async Task<IActionResult>Get(CancellationToken ct){var r=await service.GetAsync(ct);return StatusCode(r.StatusCode,r);}
    [HttpPut]public async Task<IActionResult>Put(SaveGeneralSettingsRequest request,CancellationToken ct){try{var r=await service.SaveAsync(request,ct);return StatusCode(r.StatusCode,r);}catch(ArgumentException ex){return BadRequest(ApiResponse<object>.Error(ex.Message,400));}}
}
public static class GeneralSettingsModule { public static IServiceCollection AddGeneralSettingsModule(this IServiceCollection services){services.AddScoped<IGeneralSettingsService,GeneralSettingsService>();return services;} }
