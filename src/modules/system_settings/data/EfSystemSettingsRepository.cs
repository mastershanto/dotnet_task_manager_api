using System.Collections.Concurrent;
using SystemSettings.Domain;

namespace SystemSettings.Data;

public class EfSystemSettingsRepository : ISystemSettingsRepository
{
    private static readonly ConcurrentDictionary<string, SystemSettingModel> _settings = new(StringComparer.OrdinalIgnoreCase);

    static EfSystemSettingsRepository()
    {
        SeedDefaults();
    }

    private static void SeedDefaults()
    {
        var defaults = new[]
        {
            new SystemSettingModel { Key = "site.title", Value = "Sunset Dance Center" },
            new SystemSettingModel { Key = "site.logo", Value = "https://sunsetdance.thewarriors.team/assets/images/logo.png" },
            new SystemSettingModel { Key = "terms-of-service", Value = "https://sunsetdance.thewarriors.team/terms-of-service" },
            new SystemSettingModel { Key = "privacy-policy", Value = "https://sunsetdance.thewarriors.team/privacy-policy" },
            new SystemSettingModel { Key = "app-version", Value = "1.0.0" },
            new SystemSettingModel { Key = "profile-message", Value = "Welcome to Sunset Dance Center!" }
        };

        foreach (var item in defaults)
        {
            _settings.TryAdd(item.Key, item);
        }
    }

    public Task<IEnumerable<SystemSettingModel>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<SystemSettingModel>>(_settings.Values.ToList());
    }

    public Task<SystemSettingModel?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        _settings.TryGetValue(key, out var setting);
        return Task.FromResult(setting);
    }

    public Task<SystemSettingModel> SetAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        var model = new SystemSettingModel
        {
            Key = key,
            Value = value,
            UpdatedAt = DateTime.UtcNow
        };
        _settings[key] = model;
        return Task.FromResult(model);
    }
}
