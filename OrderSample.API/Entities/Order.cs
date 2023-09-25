namespace OrderSample.API.Entities;

public class Order
{
    public Order()
    {
        
    }
    public Order(Guid id)
    {
        Id = id;
        OrderStages.Add(new OrderStage(OrderState.Draft));
    }
    public Guid Id { get; set; }
    
    public string? DeliveryAddress { get; set; }
    public Guid? SupervisorId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? WarehouseWorkerId { get; set; }
    public Guid? OrderPickerId { get; set; }
    public DateTimeOffset? ExpectedDate { get; set; }
    public Guid? DriverId { get; set; }
    public string? CancellationReason { get; set; }
    public OrderState CurrentState => OrderStages.OrderByDescending(o => o.Date).First().OrderState;
    public List<OrderStage> OrderStages { get; set; } = new();

}