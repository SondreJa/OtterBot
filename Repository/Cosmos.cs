using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace OtterBot.Repository
{

    public interface ICosmos<T> where T : class, IEntity, new()
    {
        Task<T> Get(string id);
        Task Upsert(T entity);
        Task Delete(string id);
    }

    public class Cosmos<T> : ICosmos<T> where T : class, IEntity, new()
    {
        private CosmosClient client;
        private readonly Database database;
        private readonly Container container;
        private readonly IMemoryCache cache;

        public Cosmos(IConfigurationRoot config)
        {
            this.cache = new MemoryCache(new MemoryCacheOptions());
            client = new CosmosClient(config["CosmosConnectionString"]);
            database = client.CreateDatabaseIfNotExistsAsync("MainDatabase").GetAwaiter().GetResult();
            container = database.CreateContainerIfNotExistsAsync(typeof(T).Name, "/id").GetAwaiter().GetResult();
            LoadCache().GetAwaiter().GetResult();
        }

        public async Task Delete(string id)
        {
            await container.DeleteItemAsync<T>(id, new PartitionKey(id));
            cache.Remove(id);
        }

        public async Task<T> Get(string id)
        {
            try
            {
                T entity;
                if (!cache.TryGetValue(id, out entity))
                {
                    entity = await container.ReadItemAsync<T>(id, new PartitionKey(id));
                    cache.Set(id, entity);
                }
                return entity;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task Upsert(T entity)
        {
            await container.UpsertItemAsync(entity);
            cache.Set(entity.Id, entity);
        }

        public async Task LoadCache()
        {
            var iterator = container.GetItemLinqQueryable<T>().ToFeedIterator();
            var results = await iterator.ReadNextAsync();
            foreach (var result in results)
            {
                cache.Set(result.Id, result);
            }
        }
    }

    public interface IEntity
    {
        string Id { get; }
    }
}