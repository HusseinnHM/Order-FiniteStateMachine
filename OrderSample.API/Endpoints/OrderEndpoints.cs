using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderSample.API.DbContexts;
using OrderSample.API.Entities;
using OrderSample.API.Requests;
using OrderSample.API.Services;
using OrderSample.API.States;

namespace OrderSample.API.Endpoints;

public static class OrderEndpoints
{
    public static WebApplication MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/orders");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", Get);
        group.MapPost("/New", New);
        group.MapPost("/ConfirmByCustomer", ConfirmByCustomer);
        group.MapPost("/AssignWarehouseWorker", AssignWarehouseWorker);
        group.MapPost("/CollectOrder/{id:guid}", CollectOrder);
        group.MapPost("/ConfirmShippingAndReceivingClerk", ConfirmShippingAndReceivingClerk);
        group.MapPost("/Cancel", Cancel);
        group.MapPost("/Other", Other);

        return app;
    }

    private static async Task<IResult> Get(Guid id, OrderDbContext context)
    {
        return Results.Ok(await context.Orders.FirstOrDefaultAsync(o => o.Id == id));
    }

    private static async Task<IResult> GetAll(OrderDbContext context)
    {
        return Results.Ok(await context.Orders.ToListAsync());
    }

    private static async Task<IResult> New(OrderDbContext context)
    {
        var order = new Order(Guid.NewGuid());
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return Results.Ok(order);
    }

    private static async Task<IResult> ConfirmByCustomer([FromBody] ConfirmByCustomerRequest request,
        OrderDbContext context,
        ICurrentUserAccessor currentUserAccessor)
    {
        var order = await context.Orders.FirstAsync(o => o.Id == request.Id);
        var stateMachine = new OrderStateMachine(order);
        var isFired = await stateMachine.FireWithCommandParamsAsync(OrderCommand.ConfirmByCustomer,
            (currentUserAccessor.CurrentUserId(), request.DeliveryAddress));
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            isFired,
            order
        });
    }

    private static async Task<IResult> AssignWarehouseWorker([FromBody] AssignWarehouseWorkerRequest request,
        OrderDbContext context)
    {
        var order = await context.Orders.FirstAsync(o => o.Id == request.Id);
        var stateMachine = new OrderStateMachine(order);
        var isFired = await stateMachine.FireWithCommandParamsAsync(OrderCommand.AssignWarehouseWorker, request.WarehouseWorkerId);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            isFired,
            order
        });
    }

    private static async Task<IResult> CollectOrder(Guid id, OrderDbContext context)
    {
        var order = await context.Orders.FirstAsync(o => o.Id == id);
        var stateMachine = new OrderStateMachine(order);
        var isFired = await stateMachine.FireAsync(OrderCommand.CollectOrder);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            isFired,
            order
        });
    }

    private static async Task<IResult> ConfirmShippingAndReceivingClerk(
        [FromBody] ConfirmShippingAndReceivingClerkRequest request,
        OrderDbContext context)
    {
        var order = await context.Orders.FirstAsync(o => o.Id == request.Id);
        var stateMachine = new OrderStateMachine(order);
        var isFired = await stateMachine.FireWithCommandParamsAsync(OrderCommand.ConfirmShippingAndReceivingClerk, request);

        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            isFired,
            order
        });
    }

    private static async Task<IResult> Cancel([FromBody] CancelRequest request,
        OrderDbContext context)
    {
        var order = await context.Orders.FirstAsync(o => o.Id == request.Id);
        var stateMachine = new OrderStateMachine(order);
        var isFired = await stateMachine.FireWithCommandParamsAsync(OrderCommand.Cancel, request.Reason);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            isFired,
            order
        });
    }

    private static async Task<IResult> Other([FromQuery] Guid id,[FromQuery]OrderCommand  orderCommand, OrderDbContext context)
    {
        var order = await context.Orders.FirstAsync(o => o.Id == id);
        var stateMachine = new OrderStateMachine(order);
        var isFired = await stateMachine.FireAsync(orderCommand);
        await context.SaveChangesAsync();
        return Results.Ok(new
        {
            isFired,
            order
        });
    }
}