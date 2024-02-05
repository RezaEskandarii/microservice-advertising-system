using System.Text;
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

        var debugMode = bool.Parse(configuration["Elasticsearch:DebugMode"]);

        if (debugMode)
        {
            settings = settings.EnableDebugMode(response =>
            {
                if (response.RequestBodyInBytes != null)
                    Console.WriteLine($"Request:\n{Encoding.UTF8.GetString(response.RequestBodyInBytes)}");

                if (response.ResponseBodyInBytes != null)
                    Console.WriteLine($"Response:\n{Encoding.UTF8.GetString(response.ResponseBodyInBytes)}");
            });
        }


        _elasticClient = new ElasticClient(settings);
    }

    public async Task<List<Advertisement>> SearchAsync(string? searchText, int? page, int? pageSize)
    {
        try
        {
            Func<QueryContainerDescriptor<Advertisement>, QueryContainer> querySelector;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // If searchText is null or empty, match all documents
                querySelector = q => q.MatchAll();
            }
            else
            {
                // Use wildcard query for "like" functionality
                querySelector = q => q
                    .Bool(b => b
                        .Should(sh => sh
                            .MultiMatch(m => m
                                .Fields(fs => fs
                                )
                                .Query($"*{searchText}*")
                            )
                        )
                    );
            }
 
            var searchResponse = await _elasticClient.SearchAsync<Advertisement>(s => s
                .Query(q => querySelector(q))
                .From((page - 1) * pageSize)
                .Size(pageSize)
            );

            if (!searchResponse.IsValid)
            {
                // Log Elasticsearch error
                Console.WriteLine($"Elasticsearch Error: {searchResponse.DebugInformation}");
                throw new Exception("Error occurred while querying Elasticsearch.");
            }

            return searchResponse.Documents.ToList();
        }
        catch (Exception ex)
        {
            // Log and handle the exception
            Console.WriteLine($"Exception occurred: {ex.Message}");
            throw;
        }
    }
}