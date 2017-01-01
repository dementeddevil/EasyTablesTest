using System;
using System.Web.Http;
using Swashbuckle.Application;

namespace Zen.Tracker.Server.Site
{
    public class SwaggerConfig
    {
        public static void Register(HttpConfiguration httpConfiguration)
        {
            httpConfiguration
                .EnableSwagger(
                    c =>
                    {
                        // Single API version information
                        c.SingleApiVersion("v1", "Zen Tracker");

                        // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                        c.IgnoreObsoleteActions();
                        c.IgnoreObsoleteProperties();

                        // Incorporate annotated Controllers and API Types with Xml comments 
                        c.IncludeXmlComments(GetXmlCommentsPath());
                    })
                .EnableSwaggerUi(
                    c =>
                    {
                    });
        }

        private static string GetXmlCommentsPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + @"\bin\Zen.Tracker.Server.Site.xml";
        }
    }
}
