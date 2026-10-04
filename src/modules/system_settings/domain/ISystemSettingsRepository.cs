namespace SystemSettings.Domain;

public interface ISystemSettingsRepository
{
    Task<IEnumerable<SystemSettingModel>> ListAsync(CancellationToken cancellationToken = default);
    Task<SystemSettingModel?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<SystemSettingModel> SetAsync(string key, string value, CancellationToken cancellationToken = default);
}
