using System.Globalization;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using verii_metivon_api.Core.Localization;
using verii_metivon_api.Core.Modularity;
using verii_metivon_api.Modules.Organization;
using verii_metivon_api.Modules.Organization.Application.Dtos;
using verii_metivon_api.Modules.Organization.Application.Mappings;
using verii_metivon_api.Modules.Organization.Application.Validators;
using verii_metivon_api.Modules.Organization.Domain.Entities;
using verii_metivon_api.Modules.Organization.Localization;
using Xunit;

namespace verii_metivon_api.ArchitectureTests.Organization;

public sealed class OrganizationModuleTests
{
    private static readonly string[] SupportedCultures =
        ["ar", "de", "en", "es", "fa", "fr", "it", "ja", "ko", "nl", "pl", "pt", "ru", "tr", "zh"];

    [Fact]
    public void Mapping_profile_is_valid()
    {
        var configuration = new MapperConfiguration(config => config.AddProfile<OrganizationMappingProfile>());
        configuration.AssertConfigurationIsValid();
        var mapped = configuration.CreateMapper().Map<BranchRow>(new Branch { Id = 7, Code = "01", Name = "Merkez" });
        Assert.Equal(7, mapped.Id);
    }

    [Fact]
    public async Task Validator_returns_stable_error_codes()
    {
        IValidator<SaveBranchRequest> validator = new SaveBranchRequestValidator();
        var result = await validator.ValidateAsync(new SaveBranchRequest("", "x", false, true));
        Assert.Contains(result.Errors, x => x.ErrorCode == "Organization.Branch.CodeRequired");
        Assert.Contains(result.Errors, x => x.ErrorCode == "Organization.Branch.NameLength");
    }

    [Fact]
    public void Manifest_publishes_module_contract()
    {
        var manifest = new OrganizationModuleManifest().Manifest;
        Assert.Equal("organization", manifest.Key);
        Assert.Equal("/api/branches", manifest.EndpointGroup);
        Assert.Contains("organization.branches.update", manifest.Permissions);
    }

    [Fact]
    public void Registration_exposes_validator_mapping_localizer_and_manifest()
    {
        var services = new ServiceCollection();
        services.AddOrganizationModule();
        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<IValidator<SaveBranchRequest>>());
        Assert.NotNull(provider.GetService<IMapper>());
        Assert.NotNull(provider.GetService<IModuleLocalizer<OrganizationLocalizationResource>>());
        Assert.Contains(provider.GetServices<IModuleManifestProvider>(), x => x.Manifest.Key == "organization");
    }

    [Fact]
    public void Every_supported_culture_contains_every_organization_message()
    {
        var localizer = new JsonModuleLocalizer<OrganizationLocalizationResource>();
        var keys = new[]
        {
            "Organization.Branch.CodeRequired", "Organization.Branch.CodeLength",
            "Organization.Branch.NameRequired", "Organization.Branch.NameLength",
            "Organization.Branch.CodeExists", "Organization.Branch.NotFound",
            "Organization.Branch.DefaultRequired", "Organization.Branch.DefaultDeleteForbidden",
            "Organization.Branch.InUse"
        };

        var original = CultureInfo.CurrentUICulture;
        try
        {
            foreach (var culture in SupportedCultures)
            {
                CultureInfo.CurrentUICulture = new CultureInfo(culture);
                foreach (var key in keys) Assert.NotEqual(key, localizer[key]);
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = original;
        }
    }
}
