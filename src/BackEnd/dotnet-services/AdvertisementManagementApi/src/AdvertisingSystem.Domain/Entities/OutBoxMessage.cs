namespace AdvertisingSystem.Domain.Entities;

public class OutBoxMessage
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public bool Processed { get; set; }
    public DateTime? ProcessedOn { get; set; }
}