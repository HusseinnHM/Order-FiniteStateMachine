namespace OrderSample.API.Entities;

public enum OrderState
{
    Draft,
    WaitingForWarehouseSupervisorApproval,
    WaitingForWarehouseWorkerApproval,
    WaitingForShippingAndReceivingClerkApproval,
    WaitingForOrderPickerApproval,
    WaitingDriver,
    OnWay,
    Canceled,
    Refund,
    AcceptRefund,
    ConfirmDelivery
}