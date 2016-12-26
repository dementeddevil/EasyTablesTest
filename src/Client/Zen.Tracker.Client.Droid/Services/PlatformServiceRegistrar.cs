using Autofac;
using Zen.Tracker.Client.Services;

namespace Zen.Tracker.Client.Droid.Services
{
    public class PlatformServiceRegistrar : CommonServiceRegistrar
    {
        public static void RegisterServices()
        {
            var platformContainerBuilder = new ContainerBuilder();
            platformContainerBuilder.RegisterModule<PlatformSpecificModule>();
            RegisterServices(platformContainerBuilder);
        }
    }
}