using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.ApplicationService.Contracts;
using RGamaFelix.TimeTracker.Repository;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

public class ListClientHandler : IRequestHandler<ListClientRequest, IServiceResultOf<PagedResponse<ListClientResponse>>>
{
    private readonly IAuthenticationResolver _authenticationResolver;
    private readonly TimeTrackerDbContext _dbContext;
    private readonly ILogger<ListClientHandler> _logger;

    public ListClientHandler(ILogger<ListClientHandler> logger, TimeTrackerDbContext dbContext,
        IAuthenticationResolver authenticationResolver)
    {
        _logger = logger;
        _dbContext = dbContext;
        _authenticationResolver = authenticationResolver;
    }

    public async Task<IServiceResultOf<PagedResponse<ListClientResponse>>> Handle(ListClientRequest request,
        CancellationToken cancellationToken)
    {
        var data = await _dbContext.Clients.GetPaginated(
            client => EF.Functions.Like(client.NormalizedName, $"%{(request.Name ?? "").ToUpperInvariant()}%"),
            request.Page, request.PageSize, client => new ListClientResponse(client.Id, client.Name));
        return ServiceResultOf<PagedResponse<ListClientResponse>>.Success(data, ResultTypeCode.Ok);
    }
}