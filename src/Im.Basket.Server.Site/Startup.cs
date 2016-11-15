using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Tables.Config;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Im.Basket.Server.Site.Startup))]

namespace Im.Basket.Server.Site
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Create autofac container
            var builder = new ContainerBuilder();
            builder.RegisterModule<IocModule>();
            var container = builder.Build();

            // Apply configuration to WebApi configuration
            var httpConfig =
                new HttpConfiguration
                {
                    DependencyResolver = new AutofacWebApiDependencyResolver(container),
                    IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always,
                };

            // Setup mobile application app settings
            var provider = httpConfig.GetMobileAppSettingsProvider();
            var options = provider.GetMobileAppSettings();

            // Setup mobile app configuration settings
            var mobileAppConfig = new MobileAppConfiguration()
                .MapApiControllers()
                .AddMobileAppHomeController()
                .AddPushNotifications()
                .AddTables(
                    new MobileAppTableConfiguration()
                        .MapTableControllers()
                        .AddEntityFramework());
            mobileAppConfig.ApplyTo(httpConfig);

            // Enable attribute routing for everything else we expose
            httpConfig.MapHttpAttributeRoutes();

            // Hook up autofac webapi controller dependency injection
            app.UseAutofacWebApi(httpConfig);
            app.UseWebApi(httpConfig);

            Database.SetInitializer(new MobileServiceInitializer());
        }
    }
}
