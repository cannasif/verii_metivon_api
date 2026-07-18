using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Core.Domain;

public sealed class UserDetail : Entity
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Description { get; set; }
}
