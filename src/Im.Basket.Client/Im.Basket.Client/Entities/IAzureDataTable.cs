using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppServiceHelpers.Models;

namespace Im.Basket.Client.Entities
{
    public interface IAzureDataTable<TEntity> where TEntity : EntityData
    {
        void Initialize(IAzureDataTableClient tableClient);

        Task SyncAsync();

        Task SyncAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);

        Task<TEntity> GetAsync(string id);

        Task<TEntity> GetAsync(string id, CancellationToken cancellationToken);

        Task AddAsync(TEntity entity);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task DeleteAsync(TEntity entity);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}