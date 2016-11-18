using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Im.Basket.Client.Entities
{
    public class TodoItemDataTable : AzureDataTable<TodoItem>
    {
        protected override IMobileServiceTableQuery<TodoItem> GetAllOrderBy(IMobileServiceSyncTable<TodoItem> table)
        {
            return table
                .OrderByDescending(c => c.DueAt)
                .ThenByDescending(c => c.UpdatedAt);
        }
    }
}