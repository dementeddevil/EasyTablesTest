using Autofac;
using Zen.Tracker.Client.Entities;

namespace Zen.Tracker.Client
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register data services
            builder
                .WithTableClient("http://im-basket-site.azurewebsites.net")
                .AddTable<TodoItem, TodoItemDataTable>()
                .BuildAsync();
        }
    }
}