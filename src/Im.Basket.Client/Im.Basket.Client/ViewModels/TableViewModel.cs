using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using AppServiceHelpers.Models;
using Xamarin.Forms;
using Im.Basket.Client.Entities;

namespace Im.Basket.Client.ViewModels
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
        private string _icon;
        private bool _isBusy;

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

        public Command RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new Command(async () => await ExecuteRefreshCommand()));
            }
        }

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
            set
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
            return _table.AddAsync(item);
        }

        public Task DeleteItemAsync(TEntity item)
        {
            return _table.DeleteAsync(item);
        }

        public Task UpdateItemAsync(TEntity item)
        {
            return _table.UpdateAsync(item);
        }

        private async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Exception exceptionToDisplay = null;
            try
            {
                var items = await _table.GetAllAsync();
                Items.Clear();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
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
