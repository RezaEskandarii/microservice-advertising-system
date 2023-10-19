using AdvertisingSystem.Identity.Shared.ViewModels;

namespace AdvertisingSystem.Identity.Shared.Interfaces;

public interface IDistributedTracer
{
    void LogTraces(TracingRequest tracingRequest);
}