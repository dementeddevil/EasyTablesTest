using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppServiceHelpers.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Im.Basket.Client.Entities
{
    public abstract class AzureDataTable
    {
        protected AzureDataTable()
        {
        }
    }

    public class AzureDataTable<TEntity> : AzureDataTable, IAzureDataTable<TEntity>
        where TEntity : EntityData
    {
        private IAzureDataTableClient _tableClient;
        private IMobileServiceSyncTable<TEntity> _table;

        protected IMobileServiceSyncTable<TEntity> Table => _table ?? (_table = _tableClient.MobileService.GetSyncTable<TEntity>());

        public void Initialize(IAzureDataTableClient tableClient)
        {
            _tableClient = tableClient;
        }

        public Task SyncAsync()
        {
            return SyncAsync(CancellationToken.None);
        }

        public async Task SyncAsync(CancellationToken cancellationToken)
        {
            var queryId = $"allitems-{nameof(TEntity)}".ToLower();

            // Pull changes from the server
            await Table
                .PullAsync(
                    queryId,
                    Table.CreateQuery(),
                    false,
                    cancellationToken,
                    new PullOptions
                    {
                        MaxPageSize = 100
                    })
                .ConfigureAwait(false);

            // Push changes up to server
            await _tableClient.MobileService.SyncContext
                .PushAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return GetAllAsync(CancellationToken.None);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            await SyncAsync(cancellationToken).ConfigureAwait(false);
            return await GetAllOrderBy(Table).ToEnumerableAsync().ConfigureAwait(false);
        }

        public Task<TEntity> GetAsync(string id)
        {
            return GetAsync(id, CancellationToken.None);
        }

        public async Task<TEntity> GetAsync(string id, CancellationToken cancellationToken)
        {
            await SyncAsync(cancellationToken).ConfigureAwait(false);
            return await Table.LookupAsync(id).ConfigureAwait(false);
        }

        public Task AddAsync(TEntity entity)
        {
            return AddAsync(entity, CancellationToken.None);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            // Setup created/updated date
            entity.CreatedAt = entity.UpdatedAt = DateTimeOffset.UtcNow;

            // Insert item
            await Table.InsertAsync(entity).ConfigureAwait(false);

            // Sync changes
            await SyncAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task UpdateAsync(TEntity entity)
        {
            return UpdateAsync(entity, CancellationToken.None);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            // Setup updated date
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            // Update item
            await Table.UpdateAsync(entity).ConfigureAwait(false);

            // Sync changes
            await SyncAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task DeleteAsync(TEntity entity)
        {
            return DeleteAsync(entity, CancellationToken.None);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            // Setup updated date
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            // Delete item
            await Table.DeleteAsync(entity).ConfigureAwait(false);

            // Sync changes
            await SyncAsync(cancellationToken).ConfigureAwait(false);
        }

        protected virtual IMobileServiceTableQuery<TEntity> GetAllOrderBy(IMobileServiceSyncTable<TEntity> table)
        {
            return table.OrderBy(c => c.CreatedAt);
        }
    }
}