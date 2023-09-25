# Finite State Machines

<hr/>

Managing the states and transitions of an entity with simple & lightweight ```StateMachine<TState,TCommand>```.

```csharp

public class OrderStateMachine : StateMachine<OrderState, OrderCommand>
{
    private readonly Order _order;

    public OrderStateMachine(Order order) : base(order.CurrentState)
    {
        _order = order;
        
        When(OrderStatus.WaitingForWarehouseSupervisorApproval)
            .Allow(OrderCommand.AssignWarehouseWorker, OrderStatus.WaitingForWarehouseWorkerApproval)
            .If(() => true)
            .OnExecuteAsync(OnAssignWarehouseWorkerAsync)
            .OrAllow(OrderCommand.Cancel, OrderStatus.Canceled)
            .OnExecute<string>((cancellationReason) => _order.CancellationReason = cancellationReason);
        
        // ...
    }
}

var stateMachine = new OrderStateMachine(order);
await stateMachine.FireAsync(OrderCommand.AssignWarehouseWorker);

```