using Microsoft.EntityFrameworkCore;

namespace InformationManagementSystem;

public class PaginatedList<T> : List<T>
{
    public int PageNumber { get; private set; }
    public bool HasPrevPage { get; set; }
    public bool HasNextPage { get; set; }

    public PaginatedList(List<T> items, int pageNumber, int totalPages)
    {
        PageNumber = pageNumber;
        HasPrevPage = pageNumber > 1;
        HasNextPage = pageNumber < totalPages;
        AddRange(items);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        int count = source.Count(); //total num students
        int totalPages = (int)Math.Ceiling((decimal)count / pageSize);
        List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, pageNumber, totalPages);
    }
}