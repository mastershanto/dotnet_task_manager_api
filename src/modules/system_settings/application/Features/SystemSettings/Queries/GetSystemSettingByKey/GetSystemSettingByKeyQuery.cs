using BuildingBlocks.CQRS;
using SystemSettings.Domain;

namespace SystemSettings.Application.Features.SystemSettings.Queries.GetSystemSettingByKey;

public record GetSystemSettingByKeyQuery(string Key) : IQuery<SystemSettingModel?>;
