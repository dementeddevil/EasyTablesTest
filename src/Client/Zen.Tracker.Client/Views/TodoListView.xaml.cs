using System;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Xamarin.Forms;
using Zen.Tracker.Client.Entities;
using Zen.Tracker.Client.Services;
using Zen.Tracker.Client.ViewModels;

namespace Zen.Tracker.Client.Views
{
    public partial class TodoListView : ContentPage
    {
        private class ActivityIndicatorScope : IDisposable
        {
            private readonly bool _showIndicator;
            private readonly ActivityIndicator _indicator;
            private readonly Task _indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                _indicator = indicator;
                _showIndicator = showIndicator;

                if (showIndicator)
                {
                    _indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    _indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                _indicator.IsVisible = isActive;
                _indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (_showIndicator)
                {
                    _indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private readonly ITodoItemManager _todoItemManager;

        public TodoListView()
        {
            InitializeComponent();
            InitializeToolBars();

            _todoItemManager = ServiceLocator.Current.GetInstance<ITodoItemManager>();
        }

        private void InitializeToolBars()
        {
            string addIcon = null;
            string refreshIcon = null;
            if (Device.OS == TargetPlatform.WinPhone)
            {
                addIcon = "Toolkit.Content/ApplicationBar.Add.png";
                refreshIcon = "Toolkit.Content/ApplicationBar.Refresh.png";
            }
            else if (Device.OS == TargetPlatform.Android)
            {
                addIcon = "ic_add_black_36dp.png";
                refreshIcon = "ic_refresh_black_36dp.png";
            }

            var addToolButton =
                new ToolbarItem(
                    "Add",
                    addIcon,
                    () =>
                    {
                        var todoItem = new TodoItem();
                        var todoPage = new TodoListView();
                        //todoPage.BindingContext = todoItem;
                        Navigation.PushAsync(todoPage);
                    });

            var refreshToolButton =
                new ToolbarItem(
                    "Refresh",
                    refreshIcon,
                    () =>
                    {
                        OnAppearing();
                    });

            ToolbarItems.Add(addToolButton);
            ToolbarItems.Add(refreshToolButton);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Set syncItems to true in order to synchronize the data on startup when running in offline mode
            await RefreshItems(true, syncItems: false).ConfigureAwait(true);
        }

        // Open item page only when the label is clicked.
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var todo = e.SelectedItem as TodoItem;
            if (Device.OS != TargetPlatform.iOS && todo != null)
            {
                // Not iOS - the swipe-to-delete is discoverable there
                if (Device.OS == TargetPlatform.Android)
                {
                    await DisplayAlert(todo.Title, "Press-and-hold to complete task " + todo.Title, "Got it!").ConfigureAwait(true);
                }
                else
                {
                    // Windows, not all platforms support the Context Actions yet
                    if (await DisplayAlert("Mark completed?", "Do you wish to complete " + todo.Title + "?", "Complete", "Cancel").ConfigureAwait(true))
                    {
                        await CompleteItem(todo).ConfigureAwait(true);
                    }
                }
            }

            // prevents background getting highlighted
            todoList.SelectedItem = null;
        }

        private async void OnCheckBoxChanged(object sender, bool isChecked)
        {
            var checkBox = ((CheckBox)sender);

            var item = (TodoItem)checkBox.BindingContext;

            if (item.Complete != isChecked)
            {
                item.Complete = isChecked;
                if (item.Complete)
                {
                    item.CompletedAt = DateTimeOffset.UtcNow;
                }

                await _todoItemManager.SaveTaskAsync(item).ConfigureAwait(true);
            }
        }

        #region Get size of device

        bool _firstTime = true;
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (_firstTime)
            {
                _firstTime = false;
                //TrackerApplication.TodoManager.Device.DeviceWidth = width;
                //TrackerApplication.TodoManager.Device.DeviceHeight = height;
            }
        }

        #endregion

        private async void OnAdd(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(newItemName.Text))
            {
                var todo = new TodoItem { Title = newItemName.Text };
                await AddItem(todo).ConfigureAwait(true);

                newItemName.Text = string.Empty;
                newItemName.Unfocus();
            }
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false, true).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK")
                    .ConfigureAwait(false);
            }
        }

        private async void OnComplete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var todo = mi.CommandParameter as TodoItem;
            await CompleteItem(todo).ConfigureAwait(true);
        }

        private async Task AddItem(TodoItem item)
        {
            await _todoItemManager.SaveTaskAsync(item).ConfigureAwait(true);
            todoList.ItemsSource = await _todoItemManager.GetTodoItemsAsync().ConfigureAwait(true);
        }

        private async Task CompleteItem(TodoItem item)
        {
            item.Complete = true;
            item.CompletedAt = DateTimeOffset.UtcNow;
            await _todoItemManager.SaveTaskAsync(item).ConfigureAwait(true);
            todoList.ItemsSource = await _todoItemManager.GetTodoItemsAsync().ConfigureAwait(true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                todoList.ItemsSource = await _todoItemManager
                    .GetTodoItemsAsync(syncItems)
                    .ConfigureAwait(true);
            }
        }
    }
}
