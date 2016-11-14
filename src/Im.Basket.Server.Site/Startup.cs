using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Im.Basket.Server.Site.Startup))]

namespace Im.Basket.Server.Site
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Use this class to set configuration options for your mobile service
            var provider = new MobileAppSettingsProvider();
            var options = provider.GetMobileAppSettings();
            options.HostName = "http://localhost:24933/";

            // TODO: Create autofac container
            var builder = new ContainerBuilder();
            var container = builder.Build();

            // Apply configuration to WebApi
            var httpConfig =
                new HttpConfiguration
                {
                    DependencyResolver = new AutofacWebApiDependencyResolver(container),
                    IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always,
                };
            httpConfig.GetMobileAppConfiguration()
                .MapApiControllers()
                .AddPushNotifications()
                .AddTables();
            httpConfig.MapHttpAttributeRoutes();

            app.UseAutofacWebApi(httpConfig);
            app.UseWebApi(httpConfig);

            Database.SetInitializer(new MobileServiceInitializer());
        }
    }
}
