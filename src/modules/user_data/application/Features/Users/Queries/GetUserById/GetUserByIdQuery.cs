using BuildingBlocks.CQRS;
using Users.Domain;

namespace Users.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IQuery<UserModel?>;
