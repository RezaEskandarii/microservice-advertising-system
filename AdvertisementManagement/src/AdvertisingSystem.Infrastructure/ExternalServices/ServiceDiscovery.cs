using AdvertisingSystem.Contract.Interfaces;
using Consul;

namespace AdvertisingSystem.Infrastructure.ExternalServices;

public class ServiceDiscovery : IServiceDiscovery
{
    private readonly ConsulClient _consulClient;

    public ServiceDiscovery()
    {
        _consulClient = new ConsulClient();
    }

    // TODO: implement load balancer between services
    public async Task<ICollection<ServiceResponse>> DiscoverAsync(string serviceID)
    {
        var services = (await _consulClient.Agent.Services()).Response;
        var agentServices = services.Values.Where(s => s.ID == serviceID).ToList();
        if (agentServices == null)
        {
            throw new Exception($"Service '{serviceID}' not found in Consul.");
        }

        return agentServices.Select(x => new ServiceResponse()
            {
                Address = x.Address,
                Port = x.Port,
                Meta = x.Meta,
                ID = x.ID,
                Service = x.Service,
                Tags = x.Tags
            })
            .ToList();
    }
}