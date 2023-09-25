using FiniteStateMachine;
using OrderSample.API.Entities;
using OrderSample.API.Requests;
using OrderSample.API.Services;

namespace OrderSample.API.States;

public class OrderStateMachine : StateMachine<OrderState, OrderCommand>
{
    private readonly Order _order;

    public OrderStateMachine(Order order) : base(order.CurrentState)
    {
        _order = order;
        Build();
    }

    private void Build()
    {
        OnTransition(t =>
        {
            _order.OrderStages.Add(new OrderStage(t.Next));
            Console.WriteLine($"Order with Id {_order.Id} Changed From {t.Current} to {t.Next}");
        });

        When(OrderState.Draft)
            .Allow(OrderCommand.ConfirmByCustomer, OrderState.WaitingForWarehouseSupervisorApproval)
            .OnExecute<(Guid, string)>(OnConfirmByCustomer);

        When(OrderState.WaitingForWarehouseSupervisorApproval)
            .Allow(OrderCommand.AssignWarehouseWorker, OrderState.WaitingForWarehouseWorkerApproval)
            .OnExecute<Guid>(OnAssignWarehouseWorker)
            .OrAllow(OrderCommand.Cancel, OrderState.Canceled)
            .OnExecute<string>(OnCancel);

        When(OrderState.WaitingForWarehouseWorkerApproval)
            .Allow(OrderCommand.CollectOrder, OrderState.WaitingForShippingAndReceivingClerkApproval)
            .OrAllow(OrderCommand.Cancel, OrderState.Canceled)
            .OnExecute<string>(OnCancel);

        When(OrderState.WaitingForShippingAndReceivingClerkApproval)
            .Allow(OrderCommand.ConfirmShippingAndReceivingClerk, OrderState.WaitingForOrderPickerApproval)
            .OnExecute<ConfirmShippingAndReceivingClerkRequest>(OnConfirmShippingAndReceivingClerk)
            .OrAllow(OrderCommand.Cancel, OrderState.Canceled)
            .OnExecute<string>(OnCancel);

        When(OrderState.WaitingForOrderPickerApproval)
            .Allow(OrderCommand.ConfirmByDriverOrderPicker, OrderState.WaitingDriver)
            .IfAsync<IService>(async service => await service.CheckAsync());

        When(OrderState.WaitingDriver)
            .Allow(OrderCommand.ConfirmByDriver, OrderState.OnWay)
            .If<IService>(service => service.Check());

        When(OrderState.OnWay)
            .Allow(OrderCommand.ConfirmByDriver, OrderState.ConfirmDelivery)
            .OrAllow(OrderCommand.Refund, OrderState.Refund);

        When(OrderState.ConfirmDelivery)
            .Allow(OrderCommand.Refund, OrderState.Refund);

        When(OrderState.Refund)
            .Allow(OrderCommand.ConfirmRefund, OrderState.AcceptRefund);
    }

    private void OnConfirmShippingAndReceivingClerk(ConfirmShippingAndReceivingClerkRequest request)
    {
        _order.OrderPickerId = request.OrderPickerId;
        _order.ExpectedDate = request.ExpectedDate;
    }


    private void OnConfirmByCustomer((Guid customerId, string deliveryAddress) commandParams)
    {
        _order.CustomerId = commandParams.customerId;
        _order.DeliveryAddress = commandParams.deliveryAddress;
    }

    private void OnAssignWarehouseWorker(Guid warehouseWorkerId)
    {
        _order.WarehouseWorkerId = warehouseWorkerId;
    }

    private void OnCancel(string cancellationReason)
    {
        _order.CancellationReason = cancellationReason;
    }
}