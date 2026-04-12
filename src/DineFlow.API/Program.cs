using DineFlow.Application.Common.Orders;
using DineFlow.Application.Common.Products;
using DineFlow.Application.Orders.Commands.CreateOrder;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IOrderRepository, FakeOrderRepository>();
builder.Services.AddSingleton<IProductService, FakeProductService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
});

var app = builder.Build();
app.MapControllers();

app.Run();