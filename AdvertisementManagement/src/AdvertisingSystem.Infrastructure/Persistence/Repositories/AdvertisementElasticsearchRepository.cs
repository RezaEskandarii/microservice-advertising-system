using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Nest;

namespace AdvertisingSystem.Infrastructure.Persistence.Repositories;

public class AdvertisementElasticsearchRepository : IElasticsearchRepository<Advertisement>
{
    private readonly IElasticClient _elasticClient;

    public AdvertisementElasticsearchRepository(IConfiguration configuration)
    {
        var settings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Url"]))
            .DefaultIndex("advertisements");

        _elasticClient = new ElasticClient(settings);
    }

    public async Task<List<Advertisement>> SearchAsync(string searchText, int page, int pageSize)
    {
        var searchResponse = await _elasticClient.SearchAsync<Advertisement>(s => s
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(fs => fs
                        .Field(f => f.Title)
                        .Field(f => f.Description)
                    )
                    .Query(searchText)
                )
            )
            .From((page - 1) * pageSize)
            .Size(pageSize)
        );

        if (!searchResponse.IsValid)
        {
            throw new Exception("Error occurred while querying Elasticsearch.");
        }

        return searchResponse.Documents.ToList();
    }
}