using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RGamaFelix.ServiceResponse;
using RGamaFelix.TimeTracker.Repository;
using RGamaFelix.TimeTracker.Rest.Model;

namespace RGamaFelix.TimeTracker.Domain.Service.Handler;

public class ListClientHandler : ValidatedRequestHandler<ListClientRequest, PagedResponse<ListClientResponse>>
{
    private readonly IAuthenticationResolver _authenticationResolver;
    private readonly TimeTrackerDbContext _dbContext;

    public ListClientHandler
    (IValidator<ListClientRequest> validator, ILogger<ListClientHandler> logger, TimeTrackerDbContext dbContext,
        IAuthenticationResolver authenticationResolver) : base(validator, logger)
    {
        _dbContext = dbContext;
        _authenticationResolver = authenticationResolver;
    }

    protected override async Task<IServiceResultOf<PagedResponse<ListClientResponse>>> HandleValidatedRequest
        (ListClientRequest request, CancellationToken cancellationToken)
    {
        if (!_authenticationResolver.Resolve(true))
        {
            return ServiceResultOf<PagedResponse<ListClientResponse>>.Fail("Unauthorized", ResultTypeCode.AuthorizationError);
        }

        var data = await _dbContext.Clients.GetPaginated(client => EF.Functions.Like(client.NormalizedName, $"%{(request.Name ?? "").ToUpperInvariant()}%"),
            request.Page, request.PageSize, client => new ListClientResponse(client.Id, client.Name));

        return ServiceResultOf<PagedResponse<ListClientResponse>>.Success(data, ResultTypeCode.Ok);
    }
}
