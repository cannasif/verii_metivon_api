using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace verii_metivon_api.Core.Paging;

/// <summary>
/// Adds POST .../query with a JSON PagedQuery body to every paged GET action.
/// The original GET route remains available for backward compatibility.
/// </summary>
public sealed class PagedQueryPostConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        var parameter = action.Parameters.FirstOrDefault(candidate =>
            typeof(PagedQuery).IsAssignableFrom(candidate.ParameterInfo.ParameterType));
        if (parameter is null)
        {
            return;
        }

        parameter.BindingInfo ??= new BindingInfo();
        parameter.BindingInfo.BinderType = typeof(PagedQueryModelBinder);
        parameter.BindingInfo.BindingSource = BindingSource.ModelBinding;

        foreach (var selector in action.Selectors.ToArray())
        {
            if (!GetMethods(selector).Contains("GET", StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var template = selector.AttributeRouteModel?.Template?.Trim('/');
            var queryTemplate = string.IsNullOrWhiteSpace(template) ? "query" : $"{template}/query";
            if (action.Selectors.Any(existing =>
                    string.Equals(existing.AttributeRouteModel?.Template, queryTemplate, StringComparison.OrdinalIgnoreCase)
                    && GetMethods(existing).Contains("POST", StringComparer.OrdinalIgnoreCase)))
            {
                continue;
            }

            var alias = new SelectorModel
            {
                AttributeRouteModel = selector.AttributeRouteModel is null
                    ? new AttributeRouteModel { Template = queryTemplate }
                    : new AttributeRouteModel(selector.AttributeRouteModel)
                    {
                        Template = queryTemplate,
                        Name = null
                    }
            };
            alias.ActionConstraints.Add(new HttpMethodActionConstraint(["POST"]));
            alias.EndpointMetadata.Add(new HttpMethodMetadata(["POST"]));
            action.Selectors.Add(alias);
        }
    }

    private static IReadOnlyList<string> GetMethods(SelectorModel selector) =>
        selector.ActionConstraints
            .OfType<HttpMethodActionConstraint>()
            .SelectMany(constraint => constraint.HttpMethods)
            .ToArray();
}
