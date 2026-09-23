using BuildingBlocks.CQRS;
using Users.Domain;

namespace Users.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Name,
    string Email
) : ICommand<UserModel>;
