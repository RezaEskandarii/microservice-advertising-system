namespace AdvertisingSystem.Identity.Shared.ViewModels;

public class TracingRequest
{
    public string? RequestID { get; set; }
    public string? IpAddress { get; set; }
    public string? Route { get; set; }
    public string? QueryParams { get; set; }
    public string? Payload { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
}