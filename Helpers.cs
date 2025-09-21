using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Webshop.Controllers;

namespace Webshop;
using System.Collections.Generic;

public class ProductQueryParams
{
    private int _pageSize;
    private const int MaxPageSize = 50;
    private const double MaxProductPrice = 1000.00;
    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public double MinPrice { get; set; } = 0.00;

    public double MaxPrice { get; set; } = MaxProductPrice;

    public string? Manufacturer { get; set; }

    public string? Category { get; set; }

    public string? ProductName { get; set; }
}

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int ResultCount { get; set; }

    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        ResultCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
}

public static class Extensions
{
    public static PagedList<T> ToPagedList<T>(this IQueryable<T> list, int pageNumber, int pageSize)
    {
        var items = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, list.Count(), pageNumber, pageSize);
    }

    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}