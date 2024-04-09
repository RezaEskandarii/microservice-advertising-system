using System.Net;
using Api.Tests.Models;
using Bogus;
using Xunit;

namespace Api.Tests.IntegrationTests;

public class ApiGatewayTests : IClassFixture<DockerFixture>
{
    private readonly TestClient _client;
    private readonly DockerFixture _dockerFixture;
    private readonly Faker _faker = new Faker();

    public ApiGatewayTests(DockerFixture dockerFixture)
    {
        _dockerFixture = dockerFixture;
        _client = new TestClient();
    }

    [Fact]
    public async Task Pos_SignUp_ReturnsOkResponse()
    {
        var signupObj = GetSignUpObj();
        var response = await _client.PostAsync("/api/v1/Identity/SignUp", signupObj);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_SignIn_ReturnsOkResponse()
    {
        var signupObj = GetSignUpObj();
        var signupResponse = await _client.PostAsync("/api/v1/Identity/SignUp", signupObj);
        Assert.Equal(HttpStatusCode.OK, signupResponse.StatusCode);

        var response = await _client.PostAsync("/api/v1/Identity/SignIn", new
        {
            username = signupObj.Email,
            password = signupObj.Password
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_GetCategories_ReturnsOkResponse()
    {
        var response = await _client.GetAsync("/api/v1/categories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreateAdvertisement_ReturnsOkResponse()
    {
        var requestBody = new MultipartFormDataContent
        {
            { new StringContent("test"), "Title" },
            { new StringContent("test"), "Description" },
            { new StringContent("2023-12-29"), "ExpiresAt" },
            { new StringContent("[\"test1\",\"test2\"]"), "Tags" },
            { new StringContent("2"), "categoryId" },
            { new StringContent("Make"), "properties[0].name" },
            { new StringContent("Toyota"), "properties[0].value" },
            { new StringContent("Model"), "properties[1].name" },
            { new StringContent("Camry"), "properties[1].value" },
            { new StringContent("Year"), "properties[2].name" },
            { new StringContent("2022"), "properties[2].value" },
            { new StringContent("Mileage"), "properties[3].name" },
            { new StringContent("50000"), "properties[3].value" },
            { new StringContent("Condition"), "properties[4].name" },
            { new StringContent("Used"), "properties[4].value" },
            { new StringContent("Fuel Type"), "properties[5].name" },
            { new StringContent("Gasoline"), "properties[5].value" },
            { new StringContent("Transmission"), "properties[6].name" },
            { new StringContent("Automatic"), "properties[6].value" },
            { new StringContent("Price"), "properties[7].name" },
            { new StringContent("25000.99"), "properties[7].value" }
        };

        var response = await _client.PostAsync("/api/v1/Advertisement", requestBody);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    //
    // [Fact]
    // public async Task Get_SearchAdvertisement_ReturnsOkResponse()
    // {
    //     var response = await _client.GetAsync("/api/v1/Advertisement/Search");
    //     Assert.Equal("Search Advertisement Success", response);
    // }
    //
    // [Fact]
    // public async Task Get_GetAdvertisementById_ReturnsOkResponse()
    // {
    //     var response = await _client.GetAsync("/api/v1/Advertisement/123");
    //     Assert.Equal("Get Advertisement By Id Success", response);
    // }
    //
    // [Fact]
    // public async Task Delete_RemoveAdvertisement_ReturnsOkResponse()
    // {
    //     var response = await _client.DeleteAsync("/api/v1/Advertisement/Remove/123");
    //     Assert.Equal("Remove Advertisement Success", response);
    // }
    //
    // [Fact]
    // public async Task Put_UpdateAdvertisement_ReturnsOkResponse()
    // {
    //     var response = await _client.PutAsync("/api/v1/Advertisement/Update/123", new
    //     {
    //         /* Your Advertisement data */
    //     });
    //     Assert.Equal("Update Advertisement Success", response);
    // }
    //
    // [Fact]
    // public async Task Get_AdvertisementHealthCheck_ReturnsOkResponse()
    // {
    //     var response = await _client.GetAsync("/api/v1/Advertisement/healthcheck");
    //     Assert.Equal("Advertisement Health Check Success", response);
    // }
    //
    // [Fact]
    // public async Task Get_GetLocations_ReturnsOkResponse()
    // {
    //     var response = await _client.GetAsync("/api/v1/locations");
    //     Assert.Equal("Get Locations Success", response);
    // }

    private SignUpRequest GetSignUpObj()
    {
        return new SignUpRequest()
        {
            FirstName = _faker.Person.FirstName,
            LastName = _faker.Person.LastName,
            CellNumber = _faker.Phone.PhoneNumber("+#"),
            Address = new Address()
            {
                Street = _faker.Address.StreetAddress(),
                City = _faker.Address.City(),
                State = _faker.Address.State(),
                PostalCode = _faker.Address.ZipCode(),
                Country = _faker.Address.Country()
            },
            Email = _faker.Internet.Email(),
            PhoneNumber = _faker.Phone.PhoneNumber("+#"),
            Password = "Test@1234",
            ConfirmPassword = "Test@1234"
        };
    }
}