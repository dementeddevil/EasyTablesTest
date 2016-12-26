using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppServiceHelpers.Abstractions;
using AppServiceHelpers.Models;

namespace Zen.Tracker.Client.Entities
{
    public static class TableDataStoreExtensions
    {
        public static async Task<IEnumerable<T>> WhereAsync<T>(
            this ITableDataStore<T> store, Func<T, bool> predicate)
            where T : EntityData
        {
            var items = await store.GetItemsAsync().ConfigureAwait(false);
            return items.Where(predicate);
        }
    }
}
