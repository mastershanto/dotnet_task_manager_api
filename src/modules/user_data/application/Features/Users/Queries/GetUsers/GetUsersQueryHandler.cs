using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Users.Domain;

namespace Users.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IEnumerable<UserModel>>
{
    private readonly IUserRepository _repository;

    public GetUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<UserModel>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _repository.ListAsync();
        return Result<IEnumerable<UserModel>>.Success(users);
    }
}
