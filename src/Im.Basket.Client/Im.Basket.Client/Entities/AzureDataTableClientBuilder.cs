using System.Threading.Tasks;
using AppServiceHelpers.Models;

namespace Im.Basket.Client.Entities
{
    public class AzureDataTableClientBuilder
    {
        private readonly AzureDataTableClient _client;

        public AzureDataTableClientBuilder(AzureDataTableClient client)
        {
            _client = client;
        }

        public AzureDataTableClientBuilder AddTable<TEntity>()
            where TEntity : EntityData
        {
            _client.RegisterTable<TEntity>();
            return this;
        }

        public AzureDataTableClientBuilder AddTable<TEntity, TTable>()
            where TEntity : EntityData
            where TTable : AzureDataTable<TEntity>, new()
        {
            _client.RegisterTable<TEntity, TTable>();
            return this;
        }

        public Task BuildAsync()
        {
            return _client.FinalizeSchemaAsync();
        }
    }
}