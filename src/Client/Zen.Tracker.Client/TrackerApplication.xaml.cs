using Xamarin.Forms;
using Zen.Tracker.Client.Views;

namespace Zen.Tracker.Client
{
    public partial class TrackerApplication : Application
    {
        public TrackerApplication()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new SplashView());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
