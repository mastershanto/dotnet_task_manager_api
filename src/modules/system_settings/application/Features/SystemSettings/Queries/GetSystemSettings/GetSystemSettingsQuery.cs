using BuildingBlocks.CQRS;
using SystemSettings.Domain;

namespace SystemSettings.Application.Features.SystemSettings.Queries.GetSystemSettings;

public record GetSystemSettingsQuery() : IQuery<IEnumerable<SystemSettingModel>>;
