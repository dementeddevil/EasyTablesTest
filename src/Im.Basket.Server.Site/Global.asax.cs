using System.Web.Http;
using System.Web.Routing;

namespace Im.Basket.Server.Site
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register();
        }
    }
}