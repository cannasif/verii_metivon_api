using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Modules.Organization.Domain.Entities;

public sealed class Branch : Entity
{
    public string Code { get; set; } = "0";
    public string Name { get; set; } = "Default Branch";
    public bool IsDefault { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public ICollection<User> Users { get; set; } = new List<User>();
}
