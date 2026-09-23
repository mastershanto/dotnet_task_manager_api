using BuildingBlocks.CQRS;
using Users.Domain;

namespace Users.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery : IQuery<IEnumerable<UserModel>>;
