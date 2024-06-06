using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RGamaFelix.TimeTracker.Request.Model;

namespace RGamaFelix.TimeTracker.DataContext;

public static class TimeTrackerExtensions
{
    public static async Task<PagedResponse<TDto>> GetPaginated<TEntity, TDto>(this DbSet<TEntity> set,
        Expression<Func<TEntity, bool>> filter, int page, int? pageSize, Func<TEntity, TDto> map,
        params string[] includes) where TEntity : class
    {
        IQueryable<TEntity> items;
        if (includes.Length != 0)
        {
            var iSet = set.Include(includes[0]);
            foreach (var include in includes.Skip(1))
            {
                iSet = iSet.Include(include);
            }

            items = iSet.Where(filter);
        }
        else
        {
            items = set.Where(filter);
        }

        var total = await items.CountAsync();
        var dtoItems = pageSize.HasValue
            ? await items.Skip((page - 1) * pageSize.Value).Take(pageSize.Value).Select(i => map(i)).ToListAsync()
            : await items.Select(i => map(i)).ToListAsync();
        return new PagedResponse<TDto>(dtoItems, total, (int)Math.Ceiling((double)total / pageSize ?? total), page,
            pageSize ?? total);
    }
}