using Autofac;
using Zen.Tracker.Client.Services;

namespace Zen.Tracker.Client.Droid.Services
{
    public static class PlatformServiceRegistrar
    {
        public static void RegisterServices(ContainerBuilder platformContainerBuilder)
        {
            platformContainerBuilder = platformContainerBuilder ?? new ContainerBuilder();
            platformContainerBuilder.RegisterModule<PlatformSpecificModule>();
            CommonServiceRegistrar.RegisterServices(platformContainerBuilder);
        }
    }
}