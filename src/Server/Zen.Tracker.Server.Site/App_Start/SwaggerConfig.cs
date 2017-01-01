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
                        c.SingleApiVersion("v1", "Zen Tracker")
                            .Description("API for interacting with the Zen Tracker todo repository")
                            .Contact(
                                cc => cc
                                .Name("Developer Support")
                                .Email("api@zendesignsoftware.com")
                                .Url("http://www.zendesignsoftware.com/"))
                            .License(
                                li => li
                                .Name("Limited License")
                                .Url("http://www.zendesignsoftware.com/tracker/api/license"));

                        // Enable use of HTTP andd HTTPS schemes
                        c.Schemes(new[] { "http", "https" });

                        // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                        c.IgnoreObsoleteActions();
                        c.IgnoreObsoleteProperties();

                        // Incorporate annotated Controllers and API Types with Xml comments 
                        c.IncludeXmlComments(GetXmlCommentsPath());
                    })
                .EnableSwaggerUi();
        }

        private static string GetXmlCommentsPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory + @"\bin\Zen.Tracker.Server.Site.xml";
        }
    }
}
