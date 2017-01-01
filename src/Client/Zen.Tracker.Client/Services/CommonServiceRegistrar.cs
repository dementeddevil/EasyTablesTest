using Autofac;
using Autofac.Extras.CommonServiceLocator;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace Zen.Tracker.Client.Services
{
    public static class CommonServiceRegistrar
    {
        static CommonServiceRegistrar()
        {
            if (!ServiceLocator.IsLocationProviderSet)
            {
                RegisterServices(registerFakes: true);
            }
        }

        /// <summary>
        /// Registers the services.
        /// </summary>
        /// <param name="platformContainerBuilder">The platform specific container builder.</param>
        /// <param name="registerFakes">if set to <c>true</c> then register fakes cross-platform services.</param>
        public static void RegisterServices(
            ContainerBuilder platformContainerBuilder = null, bool registerFakes = false)
        {
            platformContainerBuilder = platformContainerBuilder ?? new ContainerBuilder();

            if (ViewModelBase.IsInDesignModeStatic || registerFakes)
            {
                platformContainerBuilder.RegisterModule<FakeCrossPlatformModule>();
            }
            else
            {
                platformContainerBuilder.RegisterModule<CrossPlatformModule>();
            }

            var container = platformContainerBuilder.Build();
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
        }
    }
}
