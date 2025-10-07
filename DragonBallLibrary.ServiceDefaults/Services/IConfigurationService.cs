namespace DragonBallLibrary.ServiceDefaults.Services;

public interface IConfigurationService
{
    Task<string> GetSettingAsync(string key);
    Task<T> GetSettingAsync<T>(string key, T defaultValue = default(T));
}

public class ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    : IConfigurationService
{
    public async Task<string> GetSettingAsync(string key)
    {
        try
        {
            // Simulate Azure App Configuration retrieval
            await Task.Delay(10);

            var value = configuration[key];

            logger.LogInformation("Retrieved configuration setting {Key} from Azure App Configuration", key);

            return value ?? string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving configuration setting {Key}", key);
            throw;
        }
    }

    public async Task<T> GetSettingAsync<T>(string key, T defaultValue = default)
    {
        try
        {
            var stringValue = await GetSettingAsync(key);

            if (string.IsNullOrEmpty(stringValue))
            {
                return defaultValue;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)stringValue;
            }

            if (typeof(T) == typeof(int) && int.TryParse(stringValue, out var intValue))
            {
                return (T)(object)intValue;
            }

            if (typeof(T) == typeof(bool) && bool.TryParse(stringValue, out var boolValue))
            {
                return (T)(object)boolValue;
            }

            return defaultValue;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error parsing configuration setting {Key} as type {Type}", key, typeof(T).Name);
            return defaultValue;
        }
    }
}