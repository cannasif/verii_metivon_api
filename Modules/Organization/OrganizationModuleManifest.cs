using verii_metivon_api.Core.Modularity;
using verii_metivon_api.Modules.Organization.Localization;

namespace verii_metivon_api.Modules.Organization;

public sealed class OrganizationModuleManifest : IModuleManifestProvider
{
    public ModuleManifest Manifest { get; } = new(
        Key: "organization",
        Version: "1.0.0",
        Dependencies: Array.Empty<string>(),
        Permissions:
        [
            "organization.branches.view",
            "organization.branches.create",
            "organization.branches.update",
            "organization.branches.delete"
        ],
        LocalizationResource: typeof(OrganizationLocalizationResource),
        EndpointGroup: "/api/branches");
}
