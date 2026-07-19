using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace verii_metivon_api.Core.Localization;

public interface IModuleLocalizer<TResource>
{
    string this[string key] { get; }
}

public sealed class JsonModuleLocalizer<TResource> : IModuleLocalizer<TResource>
{
    private static readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> Cache = new();
    private static readonly Assembly Assembly = typeof(TResource).Assembly;
    private static readonly string ResourceNamespace = $"{typeof(TResource).Namespace}.Resources";

    public string this[string key]
    {
        get
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var localized = Load(culture);
            if (localized.TryGetValue(key, out var value)) return value;
            var fallback = Load("en");
            return fallback.TryGetValue(key, out value) ? value : key;
        }
    }

    private static IReadOnlyDictionary<string, string> Load(string culture) =>
        Cache.GetOrAdd(culture, static language =>
        {
            var resourceName = $"{ResourceNamespace}.{language}.json";
            using var stream = Assembly.GetManifestResourceStream(resourceName);
            if (stream is null) return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
                ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        });
}

