using System.Configuration;
using System.Web.Http;
using Autofac;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Mobile.Server.Notifications;
using Zen.Tracker.Server.Site.Services;

namespace Zen.Tracker.Server.Site
{
    public class IocModule : Module
    {
        private readonly HttpConfiguration _httpConfig;

        public IocModule(HttpConfiguration httpConfig)
        {
            _httpConfig = httpConfig;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Setup application insights tracking
            var tc = 
                new TelemetryClient
                {
                    InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"]
                };
            builder.RegisterInstance(tc);

            // Add our custom services
            builder.RegisterType<UserConversationStore>()
                .As<IUserConversationStore>();

            // Ensure push client is added to IoC container
            var pushClient = new PushClient(_httpConfig);
            builder.RegisterInstance(pushClient);
        }
    }
}