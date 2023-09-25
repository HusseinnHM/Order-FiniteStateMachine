namespace OrderSample.API.Requests;

public record CancelRequest(Guid Id, string Reason);