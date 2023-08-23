using AdvertisingSystem.Domain.ValueObjects;

namespace AdvertisingSystem.Contract.Interfaces;

public interface IServiceDiscovery
{
    Task<ICollection<ServiceResponse>> DiscoverAsync(string serviceName);
}

public class ServiceResponse
{
    public string Address { get; set; }
    public int Port { get; set; }
    public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
    public string ID { get; set; }
    public string Service { get; set; }
    public string[] Tags { get; set; }
}