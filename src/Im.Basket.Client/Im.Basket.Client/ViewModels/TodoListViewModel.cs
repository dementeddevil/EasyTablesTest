using Im.Basket.Client.Entities;

namespace Im.Basket.Client.ViewModels
{
    public class TodoListViewModel : TableViewModel<TodoItem>
    {
        public TodoListViewModel(IAzureDataTableClient client) : base(client)
        {
        }
    }
}
