using System;
using System.Collections.Generic;

namespace WebUI.Models
{
    public class PaginationModel<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartIndex => (CurrentPage - 1) * PageSize;
        public int EndIndex => Math.Min(StartIndex + PageSize - 1, TotalItems - 1);

        public PaginationModel(List<T> allItems, int page, int pageSize)
        {
            PageSize = pageSize;
            TotalItems = allItems.Count;
            TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);
            CurrentPage = Math.Max(1, Math.Min(page, TotalPages));
            
            var skip = (CurrentPage - 1) * PageSize;
            Items = allItems.Skip(skip).Take(PageSize).ToList();
        }

        public List<int> GetPageNumbers()
        {
            var pages = new List<int>();
            var start = Math.Max(1, CurrentPage - 2);
            var end = Math.Min(TotalPages, CurrentPage + 2);

            for (int i = start; i <= end; i++)
            {
                pages.Add(i);
            }

            return pages;
        }
    }
} 