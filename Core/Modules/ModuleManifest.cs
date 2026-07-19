namespace verii_metivon_api.Core.Modularity;

public sealed record ModuleManifest(
    string Key,
    string Version,
    IReadOnlyList<string> Dependencies,
    IReadOnlyList<string> Permissions,
    Type LocalizationResource,
    string EndpointGroup);

public interface IModuleManifestProvider
{
    ModuleManifest Manifest { get; }
}
