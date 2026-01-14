using System.Security.Claims;

namespace TodoApi.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public int? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub");
            if (int.TryParse(value, out var id))
                return id;
            return null;
        }
    }

    public string? Username => User?.FindFirstValue(ClaimTypes.Name) ?? User?.FindFirstValue("preferred_username");

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);
}
