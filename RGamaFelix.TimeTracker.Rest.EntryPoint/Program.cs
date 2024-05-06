using RGamaFelix.TimeTracker.ApplicationService.Configuration;
using RGamaFelix.TimeTracker.Domain.Service.Configuration;
using RGamaFelix.TimeTracker.Repository.Adapter.SqlServer;
using RGamaFelix.TimeTracker.Rest.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();
builder.Services.AddDomainService();
builder.Services.AddApplicationService(builder.Configuration.GetSection("JwtConfiguration"));
// Must Choose One
//builder.Services.UsePostgresSql(builder.Configuration);
builder.Services.UseSqlServer(builder.Configuration);
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
app.AddMiddlewares();
app.Run();
