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
                var result = await authenticator.LoginAsync(true).ConfigureAwait(true);
                if (result)
                {
                    Navigation.InsertPageBefore(new MainPage(), this);
                    await Navigation.PopAsync(true).ConfigureAwait(true);
                }
            }
            catch (Exception exception)
            {
            }
        }
    }
}
