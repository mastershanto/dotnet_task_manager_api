using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Profile.Domain;

namespace Profile.Application.Features.Profiles.Queries;

public record GetCardsQuery(int? UserId = null) : IQuery<IEnumerable<UserCardModel>>;
public record GetCreditsQuery() : IQuery<object>;
public record GetLowCreditCardQuery() : IQuery<object>;
public record GetProfileChildrenQuery() : IQuery<IEnumerable<ChildSummaryModel>>;

public class ProfileQueryHandlers :
    IQueryHandler<GetCardsQuery, IEnumerable<UserCardModel>>,
    IQueryHandler<GetCreditsQuery, object>,
    IQueryHandler<GetLowCreditCardQuery, object>,
    IQueryHandler<GetProfileChildrenQuery, IEnumerable<ChildSummaryModel>>
{
    private readonly IProfileRepository _repository;

    public ProfileQueryHandlers(IProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<UserCardModel>>> Handle(GetCardsQuery request, CancellationToken cancellationToken)
    {
        var cards = await _repository.GetCardsAsync(request.UserId, cancellationToken);
        return Result<IEnumerable<UserCardModel>>.Success(cards);
    }

    public async Task<Result<object>> Handle(GetCreditsQuery request, CancellationToken cancellationToken)
    {
        var credits = await _repository.GetCreditsAsync(cancellationToken);
        return Result<object>.Success(credits);
    }

    public async Task<Result<object>> Handle(GetLowCreditCardQuery request, CancellationToken cancellationToken)
    {
        var card = await _repository.GetLowCreditCardAsync(cancellationToken);
        return Result<object>.Success(card);
    }

    public async Task<Result<IEnumerable<ChildSummaryModel>>> Handle(GetProfileChildrenQuery request, CancellationToken cancellationToken)
    {
        var children = await _repository.GetChildrenAsync(cancellationToken);
        return Result<IEnumerable<ChildSummaryModel>>.Success(children);
    }
}
