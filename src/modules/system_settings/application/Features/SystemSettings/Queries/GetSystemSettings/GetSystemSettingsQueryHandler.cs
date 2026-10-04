using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using SystemSettings.Domain;

namespace SystemSettings.Application.Features.SystemSettings.Queries.GetSystemSettings;

public class GetSystemSettingsQueryHandler : IQueryHandler<GetSystemSettingsQuery, IEnumerable<SystemSettingModel>>
{
    private readonly ISystemSettingsRepository _repository;

    public GetSystemSettingsQueryHandler(ISystemSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<SystemSettingModel>>> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _repository.ListAsync(cancellationToken);
        return Result<IEnumerable<SystemSettingModel>>.Success(settings);
    }
}
