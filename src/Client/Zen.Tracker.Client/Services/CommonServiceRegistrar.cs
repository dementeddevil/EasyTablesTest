using Autofac;
using Autofac.Extras.CommonServiceLocator;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;

namespace Zen.Tracker.Client.Services
{
    public class CommonServiceRegistrar
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
            ContainerBuilder platformContainerBuilder = null,
            bool registerFakes = false)
        {
            var builder = new ContainerBuilder();

            if (ViewModelBase.IsInDesignModeStatic || registerFakes)
            {
                builder.RegisterModule<FakeCrossPlatformModule>();
            }
            else
            {
                builder.RegisterModule<CrossPlatformModule>();
            }

            var container = builder.Build();
            platformContainerBuilder?.Update(container);

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
        }
    }
}
