using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nest;
using Quartz;

namespace AdvertisingSystem.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class SyncReadDatabaseJob : IJob
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;

    public SyncReadDatabaseJob(IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var elasticClient = new ElasticClient(new Uri(_configuration["Elasticsearch:Url"]));

        var pageSize = 200;

        var unsyncedAdvertisements = GetUnSyncedAdvertisements(_dbContext);

        if (!unsyncedAdvertisements.Any()) return;

        for (var pageNumber = 0; pageNumber * pageSize < await unsyncedAdvertisements.CountAsync(); pageNumber++)
        {
            var advertisementsToProcess = unsyncedAdvertisements
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToList();

            var insertResult = await InsertToElasticsearch(elasticClient, advertisementsToProcess);
            if (insertResult)
            {
                await MarkAsSynced(_dbContext, advertisementsToProcess);
            }
        }
    }

    #region Private

    private async Task<bool> InsertToElasticsearch(ElasticClient elasticClient, ICollection<Advertisement> data)
    {
        var indexName = "advertisements";
        var bulkDescriptor = new BulkDescriptor();

        foreach (var entity in data)
        {
            bulkDescriptor.Index<Advertisement>(i => i
                .Index(indexName)
                .Document(entity));
        }

        var response = await elasticClient.BulkAsync(bulkDescriptor);

        return response.IsValid;
    }

    private async Task MarkAsSynced(ApplicationDbContext dbContext, List<Advertisement> data)
    {
        foreach (var entity in data)
        {
            entity.UpdateIsSyncedInReadDb(true);
        }

        await dbContext.SaveChangesAsync();
    }


    private IQueryable<Advertisement> GetUnSyncedAdvertisements(ApplicationDbContext dbContext)
    {
        return dbContext
            .Advertisements
            .Where(x => !x.IsSyncedInReadDb)
            .AsQueryable();
    }

    #endregion
}