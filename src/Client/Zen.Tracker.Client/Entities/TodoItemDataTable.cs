using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppServiceHelpers.Tables;

namespace Zen.Tracker.Client.Entities
{
    public class TodoItemDataTable : BaseTableDataStore<TodoItem>
    {
        public override async Task<IEnumerable<TodoItem>> GetItemsAsync()
        {
            var table = await base.GetItemsAsync().ConfigureAwait(false);
            return table
                .OrderByDescending(c => c.DueAt)
                .ThenByDescending(c => c.UpdatedAt);
        }
    }
}