namespace AdvertisingSystem.Domain.Entities;

public class OutBoxMessage : IEntity<long>
{
    public long Id { get; set; }
    public string Payload { get; set; }
}