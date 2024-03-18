namespace RGamaFelix.TimeTracker.Rest.Model;

public record ListClientRequest(string? Name, int PageSize, int Page) : AbstractListRequest<ListClientResponse>(PageSize, Page);
