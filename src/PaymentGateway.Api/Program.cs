using FluentValidation;

using Microsoft.OpenApi;

using PaymentGateway.Api.Clients.Bank;
using PaymentGateway.Api.MiddleWares;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Validators;

using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("service", context.Configuration["DD_SERVICE"] ?? "payment-gateway-api")
    .Enrich.WithProperty("env", context.Configuration["DD_ENV"] ?? context.HostingEnvironment.EnvironmentName)
    .Enrich.WithProperty("version", context.Configuration["DD_VERSION"] ?? "unknown")
    .WriteTo.Console(new CompactJsonFormatter()));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Payment Gateway API",
        Version = "2.0.0"
    });
});
builder.Services.AddSingleton<IPaymentRepository, PaymentsRepository>();
builder.Services.AddHttpClient<IBankClient, BankClient>()
    .ConfigureHttpClient(cfg =>
    {
        cfg.BaseAddress = new Uri("http://localhost:8080/");
    });
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IValidator<PostPaymentRequest>, PostPaymentRequestValidator>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v2/swagger.json",
            "Payment Gateway API v2");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program()
{

}