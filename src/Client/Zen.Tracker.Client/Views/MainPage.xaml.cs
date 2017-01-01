using System;
using Microsoft.Practices.ServiceLocation;
using Zen.Tracker.Client.Services;

namespace Zen.Tracker.Client.Views
{
    public partial class MainPage : AsyncContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void LogoutButton_OnClicked(object sender, EventArgs e)
        {
            ExecuteAsyncHandler(
                async () =>
                {
                    var authenticator = ServiceLocator.Current.GetInstance<IAuthenticate>();
                    await authenticator.LogoutAsync().ConfigureAwait(true);
                },
                exception => $"Logout failed: {exception.Message}");
        }
    }
}
