using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;

namespace verii_metivon_api.Modules.Identity.Api;

[ApiController, Authorize, Route("api/User")]
public sealed class UsersController(MetivonDbContext db) : ControllerBase
{
    [HttpPost("query")]
    public async Task<IActionResult> Query(UserQuery request, CancellationToken ct)
    {
        var query = db.Users.AsNoTracking().Include(x => x.Detail).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.Username.Contains(search) ||
                x.Email.Contains(search) ||
                (x.Detail != null &&
                    ((x.Detail.FirstName != null && x.Detail.FirstName.Contains(search)) ||
                     (x.Detail.LastName != null && x.Detail.LastName.Contains(search)))));
        }

        foreach (var filter in request.Filters.Take(50))
        {
            if (string.IsNullOrWhiteSpace(filter.Value)) continue;
            var value = filter.Value.Trim();
            switch (filter.Column.Trim().ToLowerInvariant())
            {
                case "id" when long.TryParse(value, out var id):
                    query = query.Where(x => x.Id == id);
                    break;
                case "username":
                    query = query.Where(x => x.Username.Contains(value));
                    break;
                case "email":
                    query = query.Where(x => x.Email.Contains(value));
                    break;
                case "isactive" when bool.TryParse(value, out var isActive):
                    query = query.Where(x => x.IsActive == isActive);
                    break;
                // fullName is already covered by the database-side Search predicate.
            }
        }

        var total = await query.CountAsync(ct);
        query = (request.SortBy ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "username" => request.IsDescending ? query.OrderByDescending(x => x.Username) : query.OrderBy(x => x.Username),
            "email" => request.IsDescending ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
            "isactive" => request.IsDescending ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
            _ => request.IsDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
        };

        var rows = await query
            .Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize)
            .Take(request.NormalizedPageSize)
            .Select(x => new UserListDto(
                x.Id,
                x.Username,
                x.Email,
                x.Detail == null ? null : x.Detail.FirstName,
                x.Detail == null ? null : x.Detail.LastName,
                x.Detail == null ? null : x.Detail.PhoneNumber,
                x.Role,
                x.IsActive,
                x.LastLoginAt,
                x.Detail == null
                    ? x.Username
                    : ((x.Detail.FirstName ?? "") + " " + (x.Detail.LastName ?? "")).Trim() == ""
                        ? x.Username
                        : ((x.Detail.FirstName ?? "") + " " + (x.Detail.LastName ?? "")).Trim(),
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync(ct);

        return Ok(ApiResponse<PagedResult<UserListDto>>.Ok(new(
            rows,
            request.NormalizedPageNumber,
            request.NormalizedPageSize,
            total)));
    }
}

public sealed class UserQuery : PagedQuery;

public sealed record UserListDto(
    long Id,
    string Username,
    string Email,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string Role,
    bool IsActive,
    DateTime? LastLoginDate,
    string FullName,
    DateTime CreationTime,
    DateTime? LastModificationTime);
