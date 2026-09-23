using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Users.Domain;

namespace Users.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserModel?>
{
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserModel?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetAsync(request.Id);
        return user is not null
            ? Result<UserModel?>.Success(user)
            : Result<UserModel?>.Failure($"User with ID '{request.Id}' was not found.");
    }
}
