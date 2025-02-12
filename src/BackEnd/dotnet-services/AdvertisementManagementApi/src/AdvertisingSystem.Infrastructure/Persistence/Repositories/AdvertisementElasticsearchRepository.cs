using System.Text;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;

namespace AdvertisingSystem.Infrastructure.Persistence.Repositories;

public class AdvertisementElasticsearchRepository : IElasticsearchRepository<Advertisement>
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<AdvertisementElasticsearchRepository> _logger;

    public AdvertisementElasticsearchRepository(IConfiguration configuration, ILogger<AdvertisementElasticsearchRepository> logger)
    {
        _logger = logger;
        var settings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Url"] ??
                                                      throw new InvalidOperationException("Elasticsearch:Url is null"))).DefaultIndex("advertisements");

        var debugMode = bool.Parse(configuration["Elasticsearch:DebugMode"] ??
                                   throw new InvalidOperationException("can not read Elasticsearch:DebugMode ad cast to boolean"));

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
                querySelector = q => q.MatchAll();
            }
            else
            {
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
                Console.WriteLine($"Elasticsearch Error: {searchResponse.DebugInformation}");
                throw new Exception("Error occurred while querying Elasticsearch.");
            }

            return searchResponse.Documents.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}