using Autofac;

namespace Zen.Tracker.Client.Entities
{
    public static class AzureDataTableExtensions
    {
        public static AzureDataTableClientBuilder WithTableClient(this ContainerBuilder containerBuilder, string url)
        {
            var client = new AzureDataTableClient(url);
            containerBuilder.RegisterInstance(client).As<IAzureDataTableClient>();
            return new AzureDataTableClientBuilder(client);
        }
    }
}