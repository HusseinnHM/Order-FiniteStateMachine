namespace OrderSample.API.Services;

public interface IService
{
    ValueTask<bool> CheckAsync();
    bool Check();
}