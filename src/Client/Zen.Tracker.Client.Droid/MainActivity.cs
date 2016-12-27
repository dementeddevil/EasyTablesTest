using System.Linq;
using System.Threading.Tasks;
using Android.Accounts;
using Android.App;
using Android.Content.PM;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Plus;
using Android.OS;
using Android.Webkit;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Zen.Tracker.Client.Droid.Services;
using Zen.Tracker.Client.Services;

namespace Zen.Tracker.Client.Droid
{
    [Activity(
        Label = "Zen Tracker",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity,
        IAuthenticate,
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener
    {
        private GoogleApiClient _googleApiClient;

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

            _googleApiClient = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .UseDefaultAccount()
                .AddApi(Auth.GOOGLE_SIGN_IN_API)
                .AddScope(new Scope(Scopes.Profile))
                .Build();
        }

        protected override void OnStart()
        {
            base.OnStart();
            _googleApiClient.Connect();
        }

        protected override void OnStop()
        {
            _googleApiClient.Disconnect();
            base.OnStop();
        }

        public async Task<bool> LoginAsync()
        {
            // Get the auth token
            var token = await GetLoginAuthTokenAsync().ConfigureAwait(false); 
            var tokenObject = new JObject(token);

            // Get the mobile service client object
            var mobileServiceClient = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
            var user = await mobileServiceClient
                .LoginAsync(MobileServiceAuthenticationProvider.Google, tokenObject)
                .ConfigureAwait(false);
            return user != null;
        }

        public async Task LogoutAsync()
        {
            CookieManager.Instance.RemoveAllCookie();
            var mobileServiceClient = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
            await mobileServiceClient.LogoutAsync().ConfigureAwait(false);
        }

        public async Task<string> GetLoginAuthTokenAsync()
        {
//            GoogleApiClient.Build()
            return string.Empty;
        }

        public async void OnConnected(Bundle connectionHint)
        {
            var signin = Auth.GoogleSignInApi.SilentSignIn(_googleApiClient);
            await signin.AsAsync().ConfigureAwait(false);
            var result = (GoogleSignInResult) signin.Get();
            if (result != null)
            {
                result.SignInAccount.IdToken
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            throw new System.NotImplementedException();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new System.NotImplementedException();
        }
    }
}

