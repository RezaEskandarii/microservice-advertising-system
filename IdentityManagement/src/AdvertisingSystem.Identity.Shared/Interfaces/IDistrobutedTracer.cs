using AdvertisingSystem.Identity.Shared.ViewModels;

namespace AdvertisingSystem.Identity.Shared.Interfaces;

public interface IDistributedTracer
{
    Task TraceAsync(TracingRequest tracingRequest);
}