using Microsoft.AspNetCore.Http;
using RGamaFelix.TimeTracker.Repository;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler.Middleware;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context, TimeTrackerDbContext dbContext)
    {
        
        await _next(context);
    }
}
