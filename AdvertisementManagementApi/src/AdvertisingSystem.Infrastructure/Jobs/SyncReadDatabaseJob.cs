using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Quartz;

namespace AdvertisingSystem.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class SyncReadDatabaseJob : IJob
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<SyncReadDatabaseJob> _logger;

    public SyncReadDatabaseJob(IConfiguration configuration, ApplicationDbContext dbContext, IElasticClient elasticClient, ILogger<SyncReadDatabaseJob> logger)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _elasticClient = elasticClient;
        _logger = logger;
    }

    /// <summary>
    ///  This method executes the job asynchronously. It fetches unsynced advertisements from the database,
    /// processes them in batches, inserts them into Elasticsearch, and marks them as synced in the database.
    /// </summary>
    /// <param name="context"></param>
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var pageSize = 200;

            var unsyncedAdvertisements = GetUnSyncedAdvertisements(_dbContext);

            if (!unsyncedAdvertisements.Any()) return;

            for (var pageNumber = 0; pageNumber * pageSize < await unsyncedAdvertisements.CountAsync(); pageNumber++)
            {
                // Extract a batch of advertisements to process.
                var advertisementsToProcess = unsyncedAdvertisements
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToList();

                var insertResult = await InsertToElasticsearch(advertisementsToProcess);

                if (insertResult)
                {
                    await MarkAsSynced(_dbContext, advertisementsToProcess);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }


    /// <summary>
    /// Inserts a collection of advertisements into Elasticsearch.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private async Task<bool> InsertToElasticsearch(ICollection<Advertisement> data)
    {
        var indexName = "advertisements";

        var bulkDescriptor = new BulkDescriptor();

        // Iterate over each advertisement and add it to the bulk descriptor.
        foreach (var entity in data)
        {
            bulkDescriptor.Index<Advertisement>(i => i
                .Index(indexName)
                .Document(entity));
        }

        var rsp = await _elasticClient.IndexManyAsync(data, indexName);

        var response = await _elasticClient.BulkAsync(bulkDescriptor);
        return response.IsValid;
    }

    /// <summary>
    /// Marks a list of advertisements as synced in the database.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="data"></param>
    private async Task MarkAsSynced(ApplicationDbContext dbContext, List<Advertisement> data)
    {
        // Update the IsSyncedInReadDb property of each advertisement to true.
        foreach (var entity in data)
        {
            entity.UpdateIsSyncedInReadDb(true);
        }

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves unsynced advertisements from the database.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    private IQueryable<Advertisement> GetUnSyncedAdvertisements(ApplicationDbContext dbContext)
    {
        // Query the Advertisements table for records that have not been synced.
        return dbContext
            .Advertisements
            .Where(x => !x.IsSyncedInReadDb)
            .AsQueryable();
    }
}