using Microsoft.EntityFrameworkCore;
using QMS.Core.Models;
using QMS.Core.Services.Extensions;
using QMS.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

namespace QMS.Storage.EntityFramework
{
    public class DatabaseService<Context, Model> : IReadCmsItem, IWriteCmsItem where Context : DbContext where Model : class
    {
        public bool CanSort(string cmsType) => true;

        private readonly Context dbContext;

        public DatabaseService(Context dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool HandlesType(string cmsType)
        {
            return typeof(Model).Name.Equals(cmsType, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<(IReadOnlyList<CmsItem> results, int total)> List(string cmsType, string? sortField, string? sortOrder, int pageSize = 20, int pageIndex = 0, string? searchQuery = null)
        {
            IQueryable<Model> returnItems = dbContext.Set<Model>();
            if (sortField != null)
            {
                sortOrder = sortOrder ?? "ASC";
                if (sortOrder == "Asc")
                    returnItems = returnItems.OrderBy(sortField);
                else
                    returnItems = returnItems.OrderBy(sortField + " DESC");
            }

            var count = await returnItems.CountAsync().ConfigureAwait(false);
            returnItems = returnItems.Skip(pageIndex).Take(pageSize);

            var result = await returnItems.ToListAsync().ConfigureAwait(false);

            return (result.Select(x => {
                var item = x.ToCmsItem();
                item.CmsType = cmsType;
                return item;
            }).ToList(), count);
        }

       

        public async Task<T?> Read<T>(string cmsType, Guid id, string? lang) where T : CmsItem
        {
            var item = await dbContext.Set<Model>().FindAsync(id).ConfigureAwait(false);
            if(item != null)
                dbContext.Entry(item).State = EntityState.Detached;

            return item?.ToObject<T>();
        }

        public async Task Write<T>(T item, string cmsType, Guid id, string? lang, string? currentUser) where T : CmsItem
        {
            var dbObj = item.ToObject<Model>();

            var existing = await this.Read<T>(cmsType, id, lang);

            if(existing == null)
                dbContext.Set<Model>().Add(dbObj);
            else
                dbContext.Set<Model>().Update(dbObj);

            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(string cmsType, Guid id, string? lang, string? currentUser)
        {
            var dbObj = await dbContext.Set<Model>().FindAsync(id).ConfigureAwait(false);
            dbContext.Set<Model>().Remove(dbObj);
            await dbContext.SaveChangesAsync();
        }

    }
}
