using AppServiceHelpers.Abstractions;
using Zen.Tracker.Client.Entities;

namespace Zen.Tracker.Client.ViewModels
{
    public class TodoListViewModel : TableViewModel<TodoItem>
    {
        public TodoListViewModel(IEasyMobileServiceClient client) : base(client)
        {
        }
    }
}
