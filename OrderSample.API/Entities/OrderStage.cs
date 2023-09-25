namespace OrderSample.API.Entities;

public class OrderStage
{
        
    public OrderStage()
    {
    }
    public OrderStage(OrderState orderState)
    {
        OrderState = orderState;
        Date = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset Date { get; set; }
    public OrderState OrderState { get; set; }
}