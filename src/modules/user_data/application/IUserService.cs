using Users.Domain;
using BuildingBlocks.Abstractions;

namespace Users.Application;

public interface IUserService
{
    Task<Result<IEnumerable<UserModel>>> GetUsersAsync();
    Task<Result<UserModel>> GetUserAsync(Guid id);
    Task<Result<UserModel>> CreateUserAsync(UserModel user);
    Task<Result<UserModel>> UpdateUserAsync(Guid id, UserModel user);
    Task<Result<bool>> DeleteUserAsync(Guid id);
}
