using MediatR;
using RGamaFelix.ServiceResponse;

namespace RGamaFelix.TimeTracker.Request.Model;

public abstract record AbstractListRequest<TResponse>(int PageSize, int Page)
    : IRequest<ServiceResultOf<PagedResponse<TResponse>>>;