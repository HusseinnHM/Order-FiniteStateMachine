namespace OrderSample.API.Services;

public interface ICurrentUserAccessor
{
    Guid CurrentUserId();
    bool IsInRole(string name);
}