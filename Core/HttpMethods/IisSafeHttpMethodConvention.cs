using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;

namespace verii_metivon_api.Core.HttpMethods;

/// <summary>
/// Keeps native mutation routes available while adding POST-compatible aliases
/// for hosts whose request filtering blocks PUT and DELETE before ASP.NET Core.
/// </summary>
public sealed class IisSafeHttpMethodConvention : IActionModelConvention
{
    private const string Post = "POST";
    private const string Put = "PUT";
    private const string Delete = "DELETE";

    public void Apply(ActionModel action)
    {
        foreach (var selector in action.Selectors.ToArray())
        {
            var methods = GetMethods(selector);
            if (methods.Contains(Put, StringComparer.OrdinalIgnoreCase))
            {
                AddPostAliasIfMissing(action, selector, GetTemplate(selector));
            }

            if (methods.Contains(Delete, StringComparer.OrdinalIgnoreCase))
            {
                var template = GetTemplate(selector)?.Trim('/');
                AddPostAliasIfMissing(
                    action,
                    selector,
                    string.IsNullOrWhiteSpace(template) ? "delete" : $"{template}/delete");
            }
        }
    }

    private static string? GetTemplate(SelectorModel selector) =>
        selector.AttributeRouteModel?.Template;

    private static IReadOnlyList<string> GetMethods(SelectorModel selector) =>
        selector.ActionConstraints
            .OfType<HttpMethodActionConstraint>()
            .SelectMany(constraint => constraint.HttpMethods)
            .ToArray();

    private static void AddPostAliasIfMissing(
        ActionModel action,
        SelectorModel source,
        string? aliasTemplate)
    {
        var aliasKey = aliasTemplate ?? string.Empty;
        if (action.Selectors.Any(selector =>
                string.Equals(GetTemplate(selector) ?? string.Empty, aliasKey, StringComparison.OrdinalIgnoreCase)
                && GetMethods(selector).Contains(Post, StringComparer.OrdinalIgnoreCase)))
        {
            return;
        }

        var alias = new SelectorModel
        {
            AttributeRouteModel = source.AttributeRouteModel is null
                ? null
                : new AttributeRouteModel(source.AttributeRouteModel)
                {
                    Template = aliasTemplate,
                    Name = null
                }
        };

        alias.ActionConstraints.Add(new HttpMethodActionConstraint([Post]));
        alias.EndpointMetadata.Add(new HttpMethodMetadata([Post]));
        action.Selectors.Add(alias);
    }
}
