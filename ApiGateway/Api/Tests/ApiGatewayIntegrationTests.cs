using System.Net;
using Api.Tests.Models;
using Bogus;
using Xunit;

namespace Api.Tests;

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