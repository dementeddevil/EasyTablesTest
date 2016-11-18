using System.Threading.Tasks;
using AppServiceHelpers.Models;
using Microsoft.WindowsAzure.MobileServices;

namespace Im.Basket.Client.Entities
{
    public interface IAzureDataTableClient
    {
        MobileServiceClient MobileService { get; }

        string Url { get; }

        Task FinalizeSchemaAsync();

        void RegisterTable<TEntity>() where TEntity : EntityData;

        void RegisterTable<TEntity, TTable>()
            where TEntity : EntityData
            where TTable : AzureDataTable<TEntity>, new();

        IAzureDataTable<T> Table<T>() where T : EntityData;
    }
}