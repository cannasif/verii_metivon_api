using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using verii_metivon_api.Core.HttpMethods;
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

    [Theory]
    [InlineData("/api/branches/7/update", "/api/branches/7", "PUT")]
    [InlineData("/api/branches/7/delete", "/api/branches/7", "DELETE")]
    [InlineData("/api/parameters/accounting/update", "/api/parameters/accounting", "PUT")]
    public void Mutation_suffixes_are_restored_before_routing(string requestPath, string expectedPath, string expectedMethod)
    {
        var resolved = IisSafeHttpMethodMiddleware.TryResolveSuffix(
            new PathString(requestPath),
            out var normalizedPath,
            out var normalizedMethod);

        Assert.True(resolved);
        Assert.Equal(expectedPath, normalizedPath.Value);
        Assert.Equal(expectedMethod, normalizedMethod);
    }
}
