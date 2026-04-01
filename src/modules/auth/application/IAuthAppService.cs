using Auth.Domain;
using BuildingBlocks.Abstractions;

namespace Auth.Application;

public interface IAuthAppService
{
    Task<Result<AuthResult>> LoginAsync(AuthenticationRequest request);
}
