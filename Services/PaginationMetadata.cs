namespace Drinks.API.Services;

public class PaginationMetadata
{
    public int TotalItemCount { get; }
    public int PageSize { get; }
    public int PageNumber { get; }
    public int TotalPageCount => 
        (int)Math.Ceiling((double)TotalItemCount / PageSize);

    public PaginationMetadata(int totalItemCount, int pageSize, int pageNumber)
    {
        TotalItemCount = totalItemCount;
        PageSize = pageSize;
        PageNumber = pageNumber;
    }
}