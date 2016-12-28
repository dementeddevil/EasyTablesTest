using System;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using Zen.Tracker.Client.Services;

namespace Zen.Tracker.Client.Views
{
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void LoginButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var authenticator = ServiceLocator.Current.GetInstance<IAuthenticate>();
                await authenticator.LoginAsync().ConfigureAwait(true);
            }
            catch (Exception exception)
            {
            }
        }
    }
}
