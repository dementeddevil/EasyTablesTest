using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Zen.Tracker.Client.Entities;

namespace Zen.Tracker.Client.Services
{
    public interface ITodoItemManager
    {
        Task<ObservableCollection<TodoItem>> GetTodoItemsAsync(bool syncItems = false);
        Task SaveTaskAsync(TodoItem item);
        Task SyncAsync();
    }
}