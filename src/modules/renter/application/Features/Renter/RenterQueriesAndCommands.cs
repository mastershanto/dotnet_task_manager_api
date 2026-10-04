using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Abstractions.Results;
using Renter.Domain;

namespace Renter.Application.Features.Renter;

public record GetRenterCitiesQuery() : IQuery<IEnumerable<RenterCityModel>>;
public record GetRenterRepeatPatternsQuery() : IQuery<IEnumerable<string>>;
public record GetRenterStudiosQuery(string? City) : IQuery<IEnumerable<RenterStudioModel>>;
public record CheckBookingCommand(CheckBookingRequest Request) : ICommand<CheckBookingResult>;
public record BookStudioCommand(BookStudioRequest Request) : ICommand<BookStudioResponse>;
public record GetMyBookingsQuery() : IQuery<IEnumerable<BookedStudioItem>>;
public record CancelBookingCommand(int Id) : ICommand<bool>;
public record CancelRecurringBookingCommand() : ICommand<bool>;

public class RenterHandlers :
    IQueryHandler<GetRenterCitiesQuery, IEnumerable<RenterCityModel>>,
    IQueryHandler<GetRenterRepeatPatternsQuery, IEnumerable<string>>,
    IQueryHandler<GetRenterStudiosQuery, IEnumerable<RenterStudioModel>>,
    ICommandHandler<CheckBookingCommand, CheckBookingResult>,
    ICommandHandler<BookStudioCommand, BookStudioResponse>,
    IQueryHandler<GetMyBookingsQuery, IEnumerable<BookedStudioItem>>,
    ICommandHandler<CancelBookingCommand, bool>,
    ICommandHandler<CancelRecurringBookingCommand, bool>
{
    private readonly IRenterRepository _repo;

    public RenterHandlers(IRenterRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IEnumerable<RenterCityModel>>> Handle(GetRenterCitiesQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetCitiesAsync(cancellationToken);
        return Result<IEnumerable<RenterCityModel>>.Success(data);
    }

    public async Task<Result<IEnumerable<string>>> Handle(GetRenterRepeatPatternsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetRepeatPatternsAsync(cancellationToken);
        return Result<IEnumerable<string>>.Success(data);
    }

    public async Task<Result<IEnumerable<RenterStudioModel>>> Handle(GetRenterStudiosQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetStudiosAsync(request.City, cancellationToken);
        return Result<IEnumerable<RenterStudioModel>>.Success(data);
    }

    public async Task<Result<CheckBookingResult>> Handle(CheckBookingCommand request, CancellationToken cancellationToken)
    {
        var data = await _repo.CheckBookingAsync(request.Request, cancellationToken);
        return Result<CheckBookingResult>.Success(data);
    }

    public async Task<Result<BookStudioResponse>> Handle(BookStudioCommand request, CancellationToken cancellationToken)
    {
        var data = await _repo.BookStudioAsync(request.Request, cancellationToken);
        return Result<BookStudioResponse>.Success(data);
    }

    public async Task<Result<IEnumerable<BookedStudioItem>>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetMyBookingsAsync(cancellationToken);
        return Result<IEnumerable<BookedStudioItem>>.Success(data);
    }

    public async Task<Result<bool>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.CancelBookingAsync(request.Id, cancellationToken);
        return Result<bool>.Success(ok);
    }

    public async Task<Result<bool>> Handle(CancelRecurringBookingCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.CancelRecurringBookingAsync(cancellationToken);
        return Result<bool>.Success(ok);
    }
}
