namespace RGamaFelix.TimeTracker.Rest.Model;

public record PagedResponse<T>(IEnumerable<T> Data, int ItemsInPage, int ItemsPerPage, int TotalPages, int TotalItems);