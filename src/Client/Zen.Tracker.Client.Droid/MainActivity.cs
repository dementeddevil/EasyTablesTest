using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using Zen.Tracker.Client.Droid.Services;
using Zen.Tracker.Client.Services;

namespace Zen.Tracker.Client.Droid
{
    [Activity(Label = "Zen.Tracker.Client", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(this).As<IAuthenticate>();
            PlatformServiceRegistrar.RegisterServices(containerBuilder);

            LoadApplication(new AndroidTrackerApplication());
        }

        public async Task<bool> LoginAsync()
        {
            var mobileServiceClient = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
            var user = await mobileServiceClient.LoginAsync(this, MobileServiceAuthenticationProvider.Google).ConfigureAwait(false);
            return user != null;
        }

        public async Task LogoutAsync()
        {
            CookieManager.Instance.RemoveAllCookie();
            var mobileServiceClient = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
            await mobileServiceClient.LogoutAsync().ConfigureAwait(false);
        }
    }
}

