using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Domain.ValueObjects;
using AdvertisingSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nest;
using Quartz;

namespace AdvertisingSystem.Infrastructure.Jobs;

//[DisallowConcurrentExecution]
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

            var insertResult =
                await InsertToElasticsearch(elasticClient, MapToAdvertisementDto(advertisementsToProcess));
            if (insertResult)
            {
                await MarkAsSynced(_dbContext, advertisementsToProcess);
            }
        }
    }

    #region Private

    private async Task<bool> InsertToElasticsearch(ElasticClient elasticClient, ICollection<AdvertisementDto> data)
    {
        var indexName = "advertisements";
        var bulkDescriptor = new BulkDescriptor();

        foreach (var entity in data)
        {
            bulkDescriptor.Index<AdvertisementDto>(i => i
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

    private ICollection<AdvertisementDto> MapToAdvertisementDto(List<Advertisement> data)
    {
        return data.Select(advertisement => new AdvertisementDto
            {
                Id = advertisement.Id,
                Title = advertisement.Title,
                UserId = advertisement.UserId,
                Description = advertisement.Description,
                Price = advertisement.Price?.Amount,
                ExpiresAt = advertisement.ExpiresAt.Value,
                CreatedAt = advertisement.CreatedAt.Value,
                UpdatedAt = advertisement.UpdatedAt.Value,
                Address = advertisement.Address,
                CategoryId = advertisement.CategoryId,
                Thumbnails = advertisement.Thumbnails,
                Properties = advertisement.Properties.ToDictionary(x => x.Name, y => y.Value),
                Tags = advertisement.Tags
            })
            .ToList();
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

internal class AdvertisementDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public decimal? Price { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Address? Address { get; set; }
    public int CategoryId { get; set; }
    public ICollection<string> Thumbnails { get; set; } = new List<string>();
    public ICollection<KeyValuePair<string, string>>? Properties { get; set; }
    public string[]? Tags { get; set; }
}