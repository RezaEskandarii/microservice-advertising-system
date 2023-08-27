namespace AdvertisingSystem.Identity.Application.UseCases.Queries;

public class GetUserCommand
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellNumber { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
}