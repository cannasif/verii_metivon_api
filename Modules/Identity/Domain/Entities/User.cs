using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Core.Domain;

public sealed class User : Entity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Admin";
    public long BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public long? ManagerUserId { get; set; }
    public User? ManagerUser { get; set; }
    public ICollection<User> DirectReports { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public UserDetail? Detail { get; set; }
}
