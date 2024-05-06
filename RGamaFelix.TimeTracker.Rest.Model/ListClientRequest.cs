using RGamaFelix.TimeTracker.Infrastructure;

namespace RGamaFelix.TimeTracker.Rest.Model;

[AuthorizeRequest]
public record ListClientRequest(string? Name, int PageSize, int Page)
    : AbstractListRequest<ListClientResponse>(PageSize, Page);
