using BuildingBlocks.CQRS;
using Users.Domain;

namespace Users.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string Name,
    string Email
) : ICommand<UserModel>;
