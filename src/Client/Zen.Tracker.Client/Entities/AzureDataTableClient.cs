using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppServiceHelpers.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Zen.Tracker.Client.Entities
{
    public class AzureDataTableClient : IAzureDataTableClient
    {
        private readonly MobileServiceSQLiteStore _store;
        private readonly IDictionary<string, AzureDataTable> _tables = new Dictionary<string, AzureDataTable>();
        private bool _isFinialized;

        public AzureDataTableClient(string url)
        {
            Url = url;
            _store = new MobileServiceSQLiteStore("app.db");
            MobileService =
                new MobileServiceClient(Url)
                {
                    SerializerSettings =
                        new MobileServiceJsonSerializerSettings
                        {
                            CamelCasePropertyNames = true
                        }
                };
        }

        public string Url { get; }

        public MobileServiceClient MobileService { get; }

        public void RegisterTable<TEntity>()
            where TEntity : EntityData
        {
            ThrowIfFinalized();
            _store.DefineTable<TEntity>();
            var table = new AzureDataTable<TEntity>();
            table.Initialize(this);
            _tables.Add(typeof(TEntity).Name, table);
        }

        public void RegisterTable<TEntity, TTable>()
            where TEntity : EntityData
            where TTable : AzureDataTable<TEntity>, new()
        {
            ThrowIfFinalized();
            _store.DefineTable<TEntity>();
            var table = new TTable();
            table.Initialize(this);
            _tables.Add(typeof(TEntity).Name, table);
        }

        public async Task FinalizeSchemaAsync()
        {
            if (!_isFinialized)
            {
                _isFinialized = true;
                await MobileService.SyncContext
                    .InitializeAsync(
                        _store,
                        new MobileServiceSyncHandler(),
                        StoreTrackingOptions.AllNotifications)
                    .ConfigureAwait(false);
            }
        }

        public IAzureDataTable<T> Table<T>() where T : EntityData
        {
            return _tables[typeof(T).Name] as AzureDataTable<T>;
        }

        private void ThrowIfFinalized()
        {
            if (_isFinialized)
            {
                throw new NotSupportedException("Table methods cannot be called after schema has been finalized.");
            }
        }
    }
}