using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace verii_metivon_api.ArchitectureTests.HttpMethods;

public sealed class IisSafeMutationRouteTests
{
    [Fact]
    public void Controllers_do_not_declare_post_update_or_delete_suffixes()
    {
        var invalidRoutes = typeof(Program).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .SelectMany(method => method.GetCustomAttributes<HttpPostAttribute>()
                .Select(attribute => new { Controller = method.DeclaringType!.Name, Method = method.Name, attribute.Template }))
            .Where(route => route.Template is not null
                && (route.Template.EndsWith("/update", StringComparison.OrdinalIgnoreCase)
                    || route.Template.EndsWith("/delete", StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        Assert.Empty(invalidRoutes);
    }
}
