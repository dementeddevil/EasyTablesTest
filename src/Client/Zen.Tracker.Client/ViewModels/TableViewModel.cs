﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AppServiceHelpers.Models;
using Xamarin.Forms;
using Zen.Tracker.Client.Entities;

namespace Zen.Tracker.Client.ViewModels
{
    public class TableViewModel<TEntity>
        where TEntity : EntityData
    {
        public const string TitlePropertyName = "Title";
        public const string SubtitlePropertyName = "Subtitle";
        public const string IconPropertyName = "Icon";
        public const string IsBusyPropertyName = "IsBusy";
        public const string CanLoadMorePropertyName = "CanLoadMore";

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

        public event PropertyChangedEventHandler PropertyChanged;

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
                OnPropertyChanged("items");
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
                SetProperty(ref _title, value, "Title");
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
                SetProperty(ref _subTitle, value, "Subtitle");
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
                SetProperty(ref _icon, value, "Icon");
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
                SetProperty(ref _isBusy, value, "IsBusy");
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
                SetProperty(ref _canLoadMore, value, "CanLoadMore");
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

        protected void SetProperty<TPropertyType>(ref TPropertyType backingStore, TPropertyType value, string propertyName, Action onChanged = null)
        {
            // Skip if unchanged
            if (EqualityComparer<TPropertyType>.Default.Equals(backingStore, value))
            {
                return;
            }

            // Update backing store and raise events
            backingStore = value;
            if (onChanged != null)
            {
                onChanged();
            }
            OnPropertyChanged(propertyName);
        }

        public void OnPropertyChanged(string propertyName)
        {
            // ISSUE: reference to a compiler-generated field
            if (PropertyChanged == null)
                return;
            // ISSUE: reference to a compiler-generated field
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}