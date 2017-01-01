using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
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
        private const int SignInRequestCode = 55511;

        private GoogleApiClient _googleApiClient;
        private bool _isConnecting;
        private bool _showLoginUI;
        private TaskCompletionSource<bool> _loginTask;

        public Task<bool> LoginAsync(bool showLoginUI)
        {
            if (!_isConnecting)
            {
                InitiateLogin(showLoginUI);
            }
            return _loginTask.Task;
        }

        public async Task LogoutAsync()
        {
            CookieManager.Instance.RemoveAllCookie();
            var mobileServiceClient = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
            await mobileServiceClient.LogoutAsync().ConfigureAwait(true);
        }

        public async void OnConnected(Bundle connectionHint)
        {
            if (_showLoginUI)
            {
                var intent = Auth.GoogleSignInApi.GetSignInIntent(_googleApiClient);
                StartActivityForResult(intent, SignInRequestCode);
            }
            else
            {
                var signin = Auth.GoogleSignInApi.SilentSignIn(_googleApiClient);
                var signinResult = await signin.AsAsync<GoogleSignInResult>().ConfigureAwait(true);
                HandleSignInResult(signinResult);
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            throw new System.NotImplementedException();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            var connectionException = new Exception(
                $"Login failed: {result.ErrorMessage}, Code: {result.ErrorCode}");
            _loginTask.TrySetException(connectionException);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            var containerBuilder = new ContainerBuilder();
            var application = new AndroidTrackerApplication();
            containerBuilder.RegisterInstance(application).As<TrackerApplication>();
            containerBuilder.RegisterInstance(this).As<IAuthenticate>();
            PlatformServiceRegistrar.RegisterServices(containerBuilder);

            LoadApplication(application);

            var signInOptions = new GoogleSignInOptions
                .Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken("379304893141-r75kkh22memf9mi873od0pittmtdj3tp.apps.googleusercontent.com")
                .Build();

            _googleApiClient = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions)
                .Build();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnStop()
        {
            _googleApiClient.Disconnect();
            base.OnStop();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == SignInRequestCode)
            {
                var signinResult = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                HandleSignInResult(signinResult);
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void InitiateLogin(bool showLoginUI)
        {
            _loginTask = new TaskCompletionSource<bool>();
            _isConnecting = true;
            _showLoginUI = showLoginUI;
            _googleApiClient.Connect();
        }

        private async void HandleSignInResult(GoogleSignInResult signInResult)
        {
            try
            {
                if (signInResult == null || !signInResult.IsSuccess || signInResult.SignInAccount == null)
                {
                    _googleApiClient.Disconnect();
                    _loginTask.TrySetResult(false);
                    return;
                }

                // Create authentication token
                var idToken = signInResult.SignInAccount.IdToken;
                var token = new JObject { { "id_token", idToken } };

                // Login to Azure Mobile Services
                var mobileServiceClient = ServiceLocator.Current.GetInstance<IMobileServiceClient>();
                var user = await mobileServiceClient
                    .LoginAsync(MobileServiceAuthenticationProvider.Google, token)
                    .ConfigureAwait(true);
                _loginTask.TrySetResult(user != null);
            }
            catch (Exception exception)
            {
                _loginTask.TrySetException(exception);
            }
            finally
            {
                _isConnecting = false;
            }

        }
    }
}

