using System;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using Zen.Tracker.Client.Services;

namespace Zen.Tracker.Client
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void LogoutButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var authenticator = ServiceLocator.Current.GetInstance<IAuthenticate>();
                await authenticator.LogoutAsync().ConfigureAwait(true);
            }
            catch (Exception exception)
            {
            }
        }
    }
}
