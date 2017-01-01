using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Zen.Tracker.Client.Views
{
    public class AsyncContentPage : ContentPage
    {
        protected async void ExecuteAsyncHandler(Func<Task> handler, Func<Exception, string> failureHandler)
        {
            Exception error = null;
            try
            {
                await handler().ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                error = exception;
            }

            if (error != null)
            {
                var errorMessage = failureHandler(error);
                await DisplayAlert("Runtime Error", errorMessage, "OK").ConfigureAwait(true);
            }
        }
    }
}