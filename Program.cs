using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Core.UnitOfWork;
using verii_metivon_api.Core.HttpMethods;
using verii_metivon_api.Modules.BusinessPartners;
using verii_metivon_api.Modules.Products;
using verii_metivon_api.Modules.Warehouses;
using verii_metivon_api.Modules.Inventory;
using verii_metivon_api.Modules.Procurement;
using verii_metivon_api.Modules.Receiving;
using verii_metivon_api.Modules.Transfers;
using verii_metivon_api.Modules.Pricing;
using verii_metivon_api.Modules.Sales;
using verii_metivon_api.Modules.Shipping;
using verii_metivon_api.Modules.Counting;
using verii_metivon_api.Modules.EDocuments;
using verii_metivon_api.Modules.Accounting;
using verii_metivon_api.Modules.LandedCosts;
using verii_metivon_api.Modules.TradeOperations;
using verii_metivon_api.Modules.Parameters;
using verii_metivon_api.Modules.Traceability;
using verii_metivon_api.Modules.GeneralSettings;
using verii_metivon_api.Modules.ExchangeRates;
using verii_metivon_api.Modules.NumberSeries;
using verii_metivon_api.Modules.AccessControl;
using verii_metivon_api.Modules.AccessControl.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is not configured.");

builder.Services.AddDbContext<MetivonDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new IisSafeHttpMethodConvention());
    options.Conventions.Add(new verii_metivon_api.Core.Paging.PagedQueryPostConvention());
    options.Filters.Add<ErpPermissionAuthorizationFilter>();
});
builder.Services.AddBusinessPartnersModule();
builder.Services.AddProductsModule();
builder.Services.AddWarehousesModule();
builder.Services.AddInventoryModule();
builder.Services.AddProcurementModule();
builder.Services.AddReceivingModule();
builder.Services.AddTransfersModule();
builder.Services.AddPricingModule();
builder.Services.AddSalesModule();
builder.Services.AddShippingModule();
builder.Services.AddCountingModule();
builder.Services.AddEDocumentsModule();
builder.Services.AddAccountingModule();
builder.Services.AddLandedCostsModule();
builder.Services.AddParametersModule();
builder.Services.AddTraceabilityModule();
builder.Services.AddGeneralSettingsModule();
builder.Services.AddExchangeRatesModule();
builder.Services.AddNumberSeriesModule();
builder.Services.AddTradeOperationsModule();
builder.Services.AddAccessControlModule();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
    .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var jwtKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseMiddleware<IisSafeHttpMethodMiddleware>();
app.UseRouting();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();

app.MapGet("/api/branches", async (IUnitOfWork unitOfWork) =>
    ApiResponse<IReadOnlyList<BranchItem>>.Ok(await unitOfWork.Branches.Query().Where(x => x.IsActive)
        .OrderByDescending(x => x.IsDefault).ThenBy(x => x.Name)
        .Select(x => new BranchItem(x.Id, x.Code, x.Name, x.IsDefault)).ToListAsync()));

app.MapPost("/api/auth/login", async (LoginRequest request, IUnitOfWork unitOfWork, JwtTokenService tokens) =>
{
    var normalized = request.Email.Trim().ToLowerInvariant();
    var user = await unitOfWork.Users.Query(tracking: true).Include(x => x.Detail)
        .FirstOrDefaultAsync(x => (x.Email.ToLower() == normalized || x.Username.ToLower() == normalized) && x.IsActive);
    if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        return Results.Json(ApiResponse<LoginResult>.Error("Email or password is incorrect.", 401), statusCode: 401);

    user.LastLoginAt = DateTime.UtcNow;
    user.RefreshToken = JwtTokenService.CreateRefreshToken();
    user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(request.RememberMe ? 30 : 1);
    await unitOfWork.SaveChangesAsync();
    var result = new LoginResult(tokens.Create(user), user.RefreshToken, user.RefreshTokenExpiresAt.Value,
        user.Id, Guid.NewGuid().ToString("N"), request.RememberMe);
    return Results.Ok(ApiResponse<LoginResult>.Ok(result, "Login successful."));
});

app.MapPost("/api/auth/refresh-token", async (RefreshRequest request, IUnitOfWork unitOfWork, JwtTokenService tokens) =>
{
    var user = await unitOfWork.Users.Query(tracking: true).Include(x => x.Detail).FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken);
    if (user is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow || !user.IsActive)
        return Results.Json(ApiResponse<LoginResult>.Error("Refresh token is invalid or expired.", 401), statusCode: 401);
    user.RefreshToken = JwtTokenService.CreateRefreshToken();
    user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1);
    await unitOfWork.SaveChangesAsync();
    return Results.Ok(ApiResponse<LoginResult>.Ok(new LoginResult(tokens.Create(user), user.RefreshToken,
        user.RefreshTokenExpiresAt.Value, user.Id, Guid.NewGuid().ToString("N"), false)));
});

static async Task<(User? User, IResult? Error)> ResolveAuthenticatedUserAsync(
    ClaimsPrincipal principal,
    IUnitOfWork unitOfWork)
{
    var rawUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!long.TryParse(rawUserId, out var userId))
        return (null, Results.Json(ApiResponse<object>.Error("Authenticated user could not be resolved.", 401), statusCode: 401));

    var user = await unitOfWork.Users.Query().Include(x => x.Detail)
        .FirstOrDefaultAsync(x => x.Id == userId && x.IsActive);
    return user is null
        ? (null, Results.Json(ApiResponse<object>.Error("Authenticated user is not active.", 401), statusCode: 401))
        : (user, null);
}

app.MapGet("/api/auth/me/permissions", async (ClaimsPrincipal principal, IPermissionService permissionService, CancellationToken cancellationToken) =>
{
    var snapshot = await permissionService.GetSnapshotAsync(principal, cancellationToken);
    return snapshot is null
        ? Results.Json(ApiResponse<object>.Error("Authenticated user could not be resolved.", 401), statusCode: 401)
        : Results.Ok(ApiResponse<MyPermissionsResult>.Ok(snapshot));
}).RequireAuthorization();

app.MapGet("/api/auth/bootstrap", async (
    ClaimsPrincipal principal,
    IUnitOfWork unitOfWork,
    IPermissionService permissionService,
    IGeneralSettingsService generalSettings,
    CancellationToken cancellationToken) =>
{
    var resolved = await ResolveAuthenticatedUserAsync(principal, unitOfWork);
    if (resolved.Error is not null) return resolved.Error;

    var user = resolved.User!;
    var settings = await generalSettings.GetAsync(cancellationToken);
    var displayName = string.Join(' ', new[] { user.Detail?.FirstName, user.Detail?.LastName }
        .Where(x => !string.IsNullOrWhiteSpace(x)));
    if (string.IsNullOrWhiteSpace(displayName)) displayName = user.Username;

    var permissionSnapshot = await permissionService.GetSnapshotAsync(principal, cancellationToken);
    if (permissionSnapshot is null) return Results.Json(ApiResponse<object>.Error("Permissions could not be resolved.", 401), statusCode: 401);
    var result = new AppBootstrapResult(
        new AppBootstrapUser(user.Id, user.Email, displayName),
        permissionSnapshot,
        settings.Data ?? new object());
    return Results.Ok(ApiResponse<AppBootstrapResult>.Ok(result));
}).RequireAuthorization();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MetivonDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.ApplyAsync(db, builder.Configuration);
}

app.Run();
public partial class Program { }
