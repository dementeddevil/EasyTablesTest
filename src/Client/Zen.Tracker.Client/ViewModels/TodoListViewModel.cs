using Zen.Tracker.Client.Entities;

namespace Zen.Tracker.Client.ViewModels
{
    public class TodoListViewModel : TableViewModel<TodoItem>
    {
        public TodoListViewModel(IAzureDataTableClient client) : base(client)
        {
        }
    }
}
