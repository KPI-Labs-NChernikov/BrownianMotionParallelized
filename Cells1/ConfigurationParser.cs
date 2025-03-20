using Microsoft.Extensions.Configuration;

namespace Cells1;

public static class ConfigurationParser
{
    public delegate bool TryParseHandler<T>(string? value, out T result);
    
    public static T Parse<T>(this IConfiguration configuration, string key, T defaultValue, TryParseHandler<T> parser)
    {
        if (configuration[key] == null)
        {
            return defaultValue;
        }

        var parsed = parser.Invoke(configuration[key], out var value);
        if (!parsed)
        {
            throw new ArgumentException($"Invalid configuration key: {key}");
        }
        
        return value;
    }
}
