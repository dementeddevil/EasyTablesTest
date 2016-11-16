using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.ApplicationInsights;
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
            // Create HTTP configuration object
            var httpConfig =
                new HttpConfiguration
                {
                    IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
                };

            // Create autofac container (it may need to update the HTTP configuration)
            var builder = new ContainerBuilder();
            builder.RegisterModule(new IocModule(httpConfig));
            var container = builder.Build();

            // Get telemetry client and log
            var telemetryClient = container.Resolve<TelemetryClient>();
            telemetryClient.TrackEvent("Service startup");

            // Setup WEBAPI dependency resolver
            httpConfig.DependencyResolver = new AutofacWebApiDependencyResolver(container);

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
