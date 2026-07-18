using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Persistence;

namespace verii_metivon_api.Modules.Identity.Api;

[ApiController, Authorize, Route("api/UserDetail")]
public sealed class UserDetailController(MetivonDbContext db, IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("user/{userId:long}")]
    public async Task<IActionResult> GetByUserId(long userId, CancellationToken ct)
    {
        if (!CanAccess(userId)) return Forbid();
        var detail = await db.UserDetails.AsNoTracking().Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId, ct);
        return detail is null
            ? NotFound(ApiResponse<UserDetailDto>.Error("User detail was not found.", 404))
            : Ok(ApiResponse<UserDetailDto>.Ok(ToDto(detail)));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var detail = await db.UserDetails.AsNoTracking().Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (detail is null) return NotFound(ApiResponse<UserDetailDto>.Error("User detail was not found.", 404));
        if (!CanAccess(detail.UserId)) return Forbid();
        return Ok(ApiResponse<UserDetailDto>.Ok(ToDto(detail)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveUserDetailRequest request, CancellationToken ct)
    {
        if (!CanAccess(request.UserId)) return Forbid();
        if (await db.UserDetails.AnyAsync(x => x.UserId == request.UserId, ct))
            return Conflict(ApiResponse<UserDetailDto>.Error("User detail already exists.", 409));
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, ct);
        if (user is null) return NotFound(ApiResponse<UserDetailDto>.Error("User was not found.", 404));
        var detail = new UserDetail { UserId = request.UserId };
        Apply(detail, user, request);
        db.UserDetails.Add(detail);
        await db.SaveChangesAsync(ct);
        return StatusCode(201, ApiResponse<UserDetailDto>.Ok(ToDto(detail), "User detail created."));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveUserDetailRequest request, CancellationToken ct)
    {
        var detail = await db.UserDetails.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (detail is null) return NotFound(ApiResponse<UserDetailDto>.Error("User detail was not found.", 404));
        if (!CanAccess(detail.UserId)) return Forbid();
        Apply(detail, detail.User, request);
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<UserDetailDto>.Ok(ToDto(detail), "User detail updated."));
    }

    [HttpPost("users/{userId:long}/profile-picture")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadProfilePicture(long userId, IFormFile file, CancellationToken ct)
    {
        if (!CanAccess(userId)) return Forbid();
        if (file.Length == 0 || file.Length > 5 * 1024 * 1024 || file.ContentType is not ("image/jpeg" or "image/png" or "image/webp"))
            return BadRequest(ApiResponse<UserDetailDto>.Error("Only JPEG, PNG or WebP images up to 5 MB are allowed.", 400));
        var detail = await db.UserDetails.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId, ct);
        if (detail is null) return NotFound(ApiResponse<UserDetailDto>.Error("User detail was not found.", 404));
        var extension = file.ContentType switch { "image/png" => ".png", "image/webp" => ".webp", _ => ".jpg" };
        var directory = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "uploads", "profiles");
        Directory.CreateDirectory(directory);
        var fileName = $"{userId}-{Guid.NewGuid():N}{extension}";
        await using (var stream = System.IO.File.Create(Path.Combine(directory, fileName))) await file.CopyToAsync(stream, ct);
        detail.ProfilePictureUrl = $"/uploads/profiles/{fileName}";
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<UserDetailDto>.Ok(ToDto(detail), "Profile picture updated."));
    }

    private bool CanAccess(long userId) => CurrentUserId() == userId || User.IsInRole("Admin");
    private long CurrentUserId() => long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    private static void Apply(UserDetail detail, User user, SaveUserDetailRequest request)
    {
        detail.ProfilePictureUrl = request.ProfilePictureUrl ?? detail.ProfilePictureUrl;
        detail.Description = request.Description;
        detail.PhoneNumber = request.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(request.Email)) user.Email = request.Email.Trim();
    }
    private static UserDetailDto ToDto(UserDetail detail) => new(detail.Id, detail.UserId, detail.ProfilePictureUrl,
        detail.Description, detail.PhoneNumber, detail.User?.Email, detail.CreatedAt, detail.UpdatedAt, false);
}

public sealed record SaveUserDetailRequest(long UserId, string? ProfilePictureUrl, string? Description, string? PhoneNumber, string? Email);
public sealed record UserDetailDto(long Id, long UserId, string? ProfilePictureUrl, string? Description, string? PhoneNumber,
    string? Email, DateTime CreatedDate, DateTime? UpdatedDate, bool IsDeleted);

[ApiController, Authorize, Route("api/Auth")]
public sealed class CurrentUserSecurityController(MetivonDbContext db) : ControllerBase
{
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized(ApiResponse<string>.Error("Authentication is required.", 401));
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsActive, ct);
        if (user is null) return NotFound(ApiResponse<string>.Error("User was not found.", 404));
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return BadRequest(ApiResponse<string>.Error("Current password is incorrect.", 400));
        if (request.NewPassword.Length is < 8 or > 100)
            return BadRequest(ApiResponse<string>.Error("New password must contain between 8 and 100 characters.", 400));
        if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
            return BadRequest(ApiResponse<string>.Error("New password must be different from the current password.", 400));
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;
        await db.SaveChangesAsync(ct);
        return Ok(ApiResponse<string>.Ok("changed", "Password changed successfully."));
    }
}

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
