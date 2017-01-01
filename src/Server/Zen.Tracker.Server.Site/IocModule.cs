using System.Configuration;
using System.Web.Http;
using Autofac;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Mobile.Server.Notifications;
using Zen.Tracker.Server.Site.Services;

namespace Zen.Tracker.Server.Site
{
    /// <summary>
    /// Defines IoC configuration for the application
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class IocModule : Module
    {
        private readonly HttpConfiguration _httpConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="IocModule"/> class.
        /// </summary>
        /// <param name="httpConfig">The HTTP configuration.</param>
        public IocModule(HttpConfiguration httpConfig)
        {
            _httpConfig = httpConfig;
        }

        /// <summary>
        /// Adds registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
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