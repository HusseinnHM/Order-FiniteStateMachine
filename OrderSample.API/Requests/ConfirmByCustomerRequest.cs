namespace OrderSample.API.Requests;

public record ConfirmByCustomerRequest(Guid Id,string DeliveryAddress);