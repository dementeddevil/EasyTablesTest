using Autofac;
using Xamarin.Forms;
using Zen.Tracker.Client.Services;

namespace Zen.Tracker.Client.Droid.Services
{
    public class PlatformSpecificModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register our platform-specific authentication provider
            builder.RegisterInstance(Application.Current)
                .As<IAuthenticate>();
        }
    }
}