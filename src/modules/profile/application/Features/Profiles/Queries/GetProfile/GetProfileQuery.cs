using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Profile.Domain;

namespace Profile.Application.Features.Profiles.Queries.GetProfile;

public record GetProfileQuery(int? Id = null) : IQuery<ProfileModel?>;

public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, ProfileModel?>
{
    private readonly IProfileRepository _repository;

    public GetProfileQueryHandler(IProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProfileModel?>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetProfileAsync(request.Id, cancellationToken);
        return Result<ProfileModel?>.Success(profile);
    }
}
