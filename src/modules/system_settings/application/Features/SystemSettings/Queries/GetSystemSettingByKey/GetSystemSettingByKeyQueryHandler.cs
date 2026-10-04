using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using SystemSettings.Domain;

namespace SystemSettings.Application.Features.SystemSettings.Queries.GetSystemSettingByKey;

public class GetSystemSettingByKeyQueryHandler : IQueryHandler<GetSystemSettingByKeyQuery, SystemSettingModel?>
{
    private readonly ISystemSettingsRepository _repository;

    public GetSystemSettingByKeyQueryHandler(ISystemSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<SystemSettingModel?>> Handle(GetSystemSettingByKeyQuery request, CancellationToken cancellationToken)
    {
        var setting = await _repository.GetByKeyAsync(request.Key, cancellationToken);
        return Result<SystemSettingModel?>.Success(setting);
    }
}
