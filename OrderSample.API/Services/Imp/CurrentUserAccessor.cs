namespace OrderSample.API.Services.Imp;

public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid CurrentUserId()
    {
        return Guid.NewGuid();
    }

    public bool IsInRole(string name) =>
        _httpContextAccessor.HttpContext!.Request.Headers["Role"].ToString().Equals(name);
}