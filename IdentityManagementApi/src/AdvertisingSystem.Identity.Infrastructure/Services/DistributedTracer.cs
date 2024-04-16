using AdvertisingSystem.Identity.Shared.Interfaces;
using AdvertisingSystem.Identity.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;
using OpenTracing.Util;

namespace AdvertisingSystem.Identity.Infrastructure.Services;

public class DistributedTracer : IDistributedTracer
{
    private readonly ITracer _tracer;

    public DistributedTracer(ITracer tracer)
    {
        _tracer = tracer;
    }

    public void LogTraces(TracingRequest tracingRequest)
    {

        var actionName = "RTTY";
    
        using var scope = _tracer.BuildSpan(actionName).StartActive(true);

        scope.Span.Log($"Add user log username: {tracingRequest.RequestID}");
        
        var spanBuilder = _tracer.BuildSpan(tracingRequest.Route);
     //   using var scope = spanBuilder.StartActive(true);
        
        scope.Span.SetTag("application_name", "identity_management");
        scope.Span.SetTag("request_id", tracingRequest.RequestID);
        
        if (!string.IsNullOrWhiteSpace(tracingRequest.ControllerName))
        {
            scope.Span.SetTag("controller_name", tracingRequest.ControllerName);
        }
        
        if (!string.IsNullOrWhiteSpace(tracingRequest.ActionName))
        {
            scope.Span.SetTag("action_name", tracingRequest.ActionName);
        }
        
        if (!string.IsNullOrWhiteSpace(tracingRequest.IpAddress))
        {
            scope.Span.SetTag("ip_address", tracingRequest.IpAddress);
        }
        
        if (!string.IsNullOrWhiteSpace(tracingRequest.QueryParams))
        {
            scope.Span.SetTag("query_params", tracingRequest.QueryParams);
        }
        
        if (!string.IsNullOrWhiteSpace(tracingRequest.Payload))
        {
            scope.Span.SetTag("payload", tracingRequest.Payload);
        }
        
        scope.Span.Log("jafargholiiiiiiiiiiiiiiiiiiiiiiiiiiiiii");
    }
}