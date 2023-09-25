namespace OrderSample.API.Services.Imp;

public class Service : IService
{
    public ValueTask<bool> CheckAsync()
    {
        return ValueTask.FromResult(DateTimeOffset.UtcNow.Nanosecond + Random.Shared.Next() % 2 == 0);
    }

    public bool Check()
    {
        return DateTimeOffset.UtcNow.Nanosecond + Random.Shared.Next() % 2 == 0;
    }
}