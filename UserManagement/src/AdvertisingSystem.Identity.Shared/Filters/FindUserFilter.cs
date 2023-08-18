namespace AdvertisingSystem.Identity.Shared.Filters;

public class FindUserFilter : BaseFilter
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Id { get; set; }
}