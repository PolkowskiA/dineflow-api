using DineFlow.Application.Common.Orders;
using DineFlow.Application.Common.Products;
using DineFlow.Application.Common;
using DineFlow.Application.Common.Exceptions;
using DineFlow.Application.Orders.Commands.CreateOrder;
using DineFlow.API.Services;
using DineFlow.Domain.Common.Exceptions;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using DineFlow.API.Extensions;
using DineFlow.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IOrderRepository, FakeOrderRepository>();
builder.Services.AddSingleton<IProductService, FakeProductService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, ApiUserContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
   options.UseSqlServer(
       builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower, allowIntegerValues: false));
    });

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
});

builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        var schemaType = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;
        if (!schemaType.IsEnum)
            return Task.CompletedTask;

        schema.Type = JsonSchemaType.String;
        schema.Format = null;
        schema.Enum = Enum.GetNames(schemaType)
            .Select(name => JsonValue.Create(JsonNamingPolicy.SnakeCaseLower.ConvertName(name)))
            .ToList<JsonNode>();

        return Task.CompletedTask;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddSignalRServices();

var app = builder.Build();

app.UseRouting();

app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception exception) when (exception is NotFoundException or DomainException or ArgumentException or InvalidOperationException)
    {
        var statusCode = GetProblemStatusCode(exception);

        await Results.Problem(
            title: GetProblemTitle(statusCode),
            detail: exception.Message,
            statusCode: statusCode)
            .ExecuteAsync(context);
    }
});

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapSignalRHubs();

app.Run();

static int GetProblemStatusCode(Exception exception)
{
    return exception is NotFoundException
        ? StatusCodes.Status404NotFound
        : StatusCodes.Status400BadRequest;
}

static string GetProblemTitle(int statusCode)
{
    return statusCode == StatusCodes.Status404NotFound
        ? "Resource not found"
        : "Invalid request";
}