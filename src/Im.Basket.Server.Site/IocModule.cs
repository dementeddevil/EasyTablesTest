using System.Web.Http;
using Autofac;
using Microsoft.Azure.Mobile.Server.Notifications;

namespace Im.Basket.Server.Site
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
            base.Load(builder);

            // Ensure push client is added to IoC container
            var pushClient = new PushClient(_httpConfig);
            builder.RegisterInstance(pushClient);
        }
    }
}