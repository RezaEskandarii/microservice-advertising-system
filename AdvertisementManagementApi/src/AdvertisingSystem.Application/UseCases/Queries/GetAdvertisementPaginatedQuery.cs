using MediatR;

namespace AdvertisingSystem.Application.UseCases.Queries;

public class GetAdvertisementPaginatedQuery : IRequest
{
    public string? UserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public long? Id { get; set; }
    public string? Filter { get; set; }
    public Dictionary<string,string>? Properties { get; set; }
}