using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using OrderSample.API.DbContexts;
using OrderSample.API.Endpoints;
using OrderSample.API.Entities;
using OrderSample.API.Services;
using OrderSample.API.Services.Imp;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .Configure<JsonOptions>(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDbContext<OrderDbContext>(o => o.UseInMemoryDatabase("InMemory"))
    .AddLogging()
    .AddHttpContextAccessor()
    .AddTransient<ICurrentUserAccessor, CurrentUserAccessor>()
    .AddTransient<IService, Service>();

var app = builder.Build();

app
    .UseSwagger()
    .UseSwaggerUI();

app.MapOrderEndpoints();

var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<OrderDbContext>();
dbContext.Add(new Order(new("EEE8E03C-CD63-4ABA-A440-856BAF6BD488")));
await dbContext.SaveChangesAsync();

app.Run();