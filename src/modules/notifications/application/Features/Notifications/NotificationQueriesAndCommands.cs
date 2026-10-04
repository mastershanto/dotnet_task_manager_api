using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Abstractions.Results;
using Notifications.Domain;

namespace Notifications.Application.Features.Notifications;

public record GetNotificationsQuery(string? Filter) : IQuery<NotificationsDataModel>;
public record MarkNotificationSeenCommand(int Id) : ICommand<bool>;
public record DeleteNotificationCommand(int Id) : ICommand<bool>;

public class NotificationsHandlers :
    IQueryHandler<GetNotificationsQuery, NotificationsDataModel>,
    ICommandHandler<MarkNotificationSeenCommand, bool>,
    ICommandHandler<DeleteNotificationCommand, bool>
{
    private readonly INotificationsRepository _repo;

    public NotificationsHandlers(INotificationsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<NotificationsDataModel>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetNotificationsAsync(request.Filter, cancellationToken);
        return Result<NotificationsDataModel>.Success(data);
    }

    public async Task<Result<bool>> Handle(MarkNotificationSeenCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.MarkSeenAsync(request.Id, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.DeleteNotificationAsync(request.Id, cancellationToken);
        return Result<bool>.Success(ok);
    }
}
