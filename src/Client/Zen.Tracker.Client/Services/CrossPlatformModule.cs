using AppServiceHelpers;
using Autofac;
using Microsoft.Rest;
using Microsoft.WindowsAzure.MobileServices;
using Zen.Tracker.Client.Entities;
using Zen.Tracker.Client.ViewModels;

namespace Zen.Tracker.Client.Services
{
    public class CrossPlatformModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register bot connector
            //builder.RegisterType<BotConnectorDirectLineAPIV30>()
            //    .As<IBotConnectorDirectLineAPIV30>()
            //    .WithParameter(
            //        "credentials",
            //        new TokenCredentials(""));

            // Register mobile service client
            var mobileServiceClient = (EasyMobileServiceClient)EasyMobileServiceClient.Create();
            mobileServiceClient.Initialize(Constants.ApplicationURL);
            RegisterSchemaTables(mobileServiceClient);
            mobileServiceClient.FinalizeSchema();
            builder.RegisterInstance(mobileServiceClient)
                .SingleInstance();
            builder.RegisterInstance(mobileServiceClient.MobileService)
                .As<IMobileServiceClient>()
                .SingleInstance();

            // Register core services
            builder.RegisterType<TodoItemManager>().As<ITodoItemManager>().SingleInstance();

            // Register view models
            builder.RegisterType<TodoListViewModel>().AsSelf();
        }

        private void RegisterSchemaTables(EasyMobileServiceClient mobileServiceClient)
        {
            mobileServiceClient.RegisterTable<TodoItem, TodoItemDataTable>();
        }
    }
}