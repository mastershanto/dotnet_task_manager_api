using App.Features.Auth.Domain;
using Shared;

namespace App.Features.Auth.Application;

public interface IAuthAppService
{
    Task<Result<AuthResult>> LoginAsync(AuthenticationRequest request);
}
