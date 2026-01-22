namespace TodoApi.Presentation.DTOs;

/// <summary>
/// Common query parameters for pagination and filtering
/// (Defined in TodoApi.DTOs for use with repository - this is just the DTO portion)
/// </summary>
public class QueryParamsDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
    public string? SearchTerm { get; set; }
}
