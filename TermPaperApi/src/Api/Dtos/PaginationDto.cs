namespace Api.Dtos;

public record PaginationDto
{
    public int? Page { get; set; } = null;
    public int? PageSize { get; set; } = null;
}