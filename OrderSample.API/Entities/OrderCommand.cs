namespace OrderSample.API.Entities;

public enum OrderCommand
{
    ConfirmByCustomer,
    AssignWarehouseWorker,
    CollectOrder,
    ConfirmShippingAndReceivingClerk,
    ConfirmByDriverOrderPicker,
    ConfirmByDriver,
    Cancel,
    Refund,
    ConfirmDelivery,
    ConfirmRefund
}