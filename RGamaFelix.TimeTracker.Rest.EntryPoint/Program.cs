using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using RGamaFelix.TimeTracker.Application.Service.Configuration;
using RGamaFelix.TimeTracker.Domain.Service.Configuration;
using RGamaFelix.TimeTracker.Rest.Api.Configuration;

//using RGamaFelix.TimeTracker.DataContext.Adapter.InMemory;
 using RGamaFelix.TimeTracker.DataContext.Adapter.PostgresSql;
// using RGamaFelix.TimeTracker.DataContext.Adapter.SqlServer;
var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry Logging
builder.Logging.ClearProviders();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddOpenTelemetry(static configure => configure.AddConsoleExporter());
}

builder.Logging.AddOpenTelemetry(configure =>
{
    configure.SetResourceBuilder(ResourceBuilder.CreateEmpty().AddService(builder.Environment.ApplicationName)
        .AddAttributes(new Dictionary<string, object> { ["environment"] = builder.Environment.EnvironmentName }));
    configure.IncludeScopes = true;
    configure.IncludeFormattedMessage = true;
    configure.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
        options.Protocol = OtlpExportProtocol.HttpProtobuf;
        options.Headers = "X-SEC-API-KEY=aYyrfau0fJdV3FPvU3X8";
    });
});
builder.Services.AddDomainService();
builder.Services.AddApplicationService(builder.Configuration.GetSection("JwtConfiguration"));
// Must Choose One
//builder.Services.UseInMemoryDatabase(builder.Configuration);
 builder.Services.UsePostgresSql(builder.Configuration);
// builder.Services.UseSqlServer(builder.Configuration);
// ----------------
builder.Services.AddControllers().AddTimeTrackerControllers();
builder.Services.AddIdentityServices(builder.Configuration.GetSection("JwtConfiguration"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policyBuilder => { policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
