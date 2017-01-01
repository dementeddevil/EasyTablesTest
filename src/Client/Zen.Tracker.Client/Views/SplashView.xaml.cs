using Microsoft.Practices.ServiceLocation;
using Zen.Tracker.Client.Services;

namespace Zen.Tracker.Client.Views
{
    public partial class SplashView : AsyncContentPage
    {
        public SplashView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ExecuteAsyncHandler(
                async () =>
                {
                    var authenticate = ServiceLocator.Current.GetInstance<IAuthenticate>();
                    var result = await authenticate.LoginAsync(false).ConfigureAwait(true);
                    if (result)
                    {
                        Navigation.InsertPageBefore(new TodoListView(), this);
                    }
                    else
                    {
                        Navigation.InsertPageBefore(new LoginView(), this);
                    }
                    await Navigation.PopAsync(true).ConfigureAwait(true);
                },
                exception => $"Login transition failed: {exception.Message}");
        }
    }
}
