using Xamarin.Forms;
using Zen.Tracker.Client.Views;

namespace Zen.Tracker.Client
{
    public partial class TrackerApplication : Application
    {
        public TrackerApplication()
        {
            InitializeComponent();

            MainPage = new SplashView();
        }

        public void ShowLoginView()
        {
            MainPage = new LoginView();
        }

        public void ShowMainView()
        {
            MainPage = new MainPage();
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
