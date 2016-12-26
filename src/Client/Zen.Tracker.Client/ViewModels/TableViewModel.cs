using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AppServiceHelpers.Models;
using GalaSoft.MvvmLight;
using Xamarin.Forms;
using Zen.Tracker.Client.Entities;

namespace Zen.Tracker.Client.ViewModels
{
    public class TableViewModel<TEntity> : ViewModelBase
        where TEntity : EntityData
    {
        private readonly IAzureDataTableClient _client;
        private readonly IAzureDataTable<TEntity> _table;
        private ObservableCollection<TEntity> _items = new ObservableCollection<TEntity>();
        private string _title = string.Empty;
        private string _subTitle = string.Empty;
        private bool _canLoadMore = true;
        private Command _refreshCommand;
        private Command _cancelCommand;
        private string _icon;
        private bool _isBusy;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public TableViewModel(IAzureDataTableClient client)
        {
            _client = client;
            _table = client.Table<TEntity>();
        }

        public ObservableCollection<TEntity> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }

        public Command RefreshCommand => _refreshCommand ?? (_refreshCommand = new Command(async () => await ExecuteRefreshCommand()));

        public Command CancelCommand => _cancelCommand ?? (_cancelCommand = new Command(ExecuteCancelCommand));

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                Set(ref _title, value);
            }
        }

        public string Subtitle
        {
            get
            {
                return _subTitle;
            }
            set
            {
                Set(ref _subTitle, value);
            }
        }

        public string Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                Set(ref _icon, value);
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            private set
            {
                Set(ref _isBusy, value);
            }
        }

        public bool CanLoadMore
        {
            get
            {
                return _canLoadMore;
            }
            set
            {
                Set(ref _canLoadMore, value);
            }
        }

        public Task AddItemAsync(TEntity item)
        {
            return _table.AddAsync(item, _cancellationTokenSource.Token);
        }

        public Task DeleteItemAsync(TEntity item)
        {
            return _table.DeleteAsync(item, _cancellationTokenSource.Token);
        }

        public Task UpdateItemAsync(TEntity item)
        {
            return _table.UpdateAsync(item, _cancellationTokenSource.Token);
        }

        private async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Exception exceptionToDisplay = null;
            try
            {
                var cancellationToken = _cancellationTokenSource.Token;
                var items = await _table.GetAllAsync(cancellationToken).ConfigureAwait(true);
                if (!cancellationToken.IsCancellationRequested)
                {
                    Items.Clear();
                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                exceptionToDisplay = ex;
            }
            finally
            {
                IsBusy = false;
            }

            if (exceptionToDisplay != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", exceptionToDisplay.Message, "OK");
            }
        }

        private void ExecuteCancelCommand()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}
