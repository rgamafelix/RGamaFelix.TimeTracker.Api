using RGamaFelix.TimeTracker.Request.Preprocessor;

namespace RGamaFelix.TimeTracker.Request.Model;

[AuthorizeRequest]
public record ListClientRequest(string? Name, int PageSize, int Page)
    : AbstractListRequest<ListClientResponse>(PageSize, Page);