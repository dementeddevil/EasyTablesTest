using AppServiceHelpers;
using Autofac;
using Microsoft.Rest;
using Zen.Tracker.Client.Entities;

namespace Zen.Tracker.Client.Services
{
    public class CrossPlatformModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register bot connector
            builder.RegisterType<BotConnectorDirectLineAPIV30>()
                .As<IBotConnectorDirectLineAPIV30>()
                .WithParameter(
                    "credentials",
                    new TokenCredentials(""));

            // Register data services
            builder
                .WithTableClient(Constants.ApplicationURL)
                .AddTable<TodoItem, TodoItemDataTable>()
                .BuildAsync();

            // Register mobile service client
            var azureClient = EasyMobileServiceClient.Create();
            azureClient.Initialize(Constants.ApplicationURL);
            azureClient.RegisterTable<TodoItem>();
            azureClient.FinalizeSchema();
            builder.RegisterInstance(azureClient);
        }
    }
}