namespace OrderSample.API.Requests;

public record ConfirmShippingAndReceivingClerkRequest(Guid Id, Guid OrderPickerId,DateTimeOffset ExpectedDate);