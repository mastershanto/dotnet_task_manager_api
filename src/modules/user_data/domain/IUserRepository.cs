namespace Users.Domain;

public interface IUserRepository
{
    Task<IEnumerable<UserModel>> ListAsync();
    Task<UserModel?> GetAsync(Guid id);
    Task<UserModel> CreateAsync(UserModel user);
    Task<UserModel?> UpdateAsync(Guid id, UserModel user);
    Task<bool> DeleteAsync(Guid id);
}
