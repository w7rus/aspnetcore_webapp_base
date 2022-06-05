using System;
using System.Linq;
using Common.Models;
using DAL.Repository.Base;
using Domain.Entities.Base;

namespace DAL.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> GetPage<T>(this IQueryable<T> query, PageModel model) where T : class
        {
            return query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize);
        }
    }
}