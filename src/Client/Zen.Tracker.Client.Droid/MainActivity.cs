using Android.App;
using Android.Content.PM;
using Android.OS;
using AppServiceHelpers;
using Zen.Tracker.Client.Entities;

namespace Zen.Tracker.Client.Droid
{
    [Activity(Label = "Zen.Tracker.Client", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            var azureClient = EasyMobileServiceClient.Create();
            azureClient.Initialize("http://zen-tracker-site.azurewebsites.net");
            azureClient.RegisterTable<TodoItem>();
            azureClient.FinalizeSchema();

            LoadApplication(new App());
        }
    }
}

