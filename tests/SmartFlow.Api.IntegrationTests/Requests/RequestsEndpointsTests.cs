using System.Net;
using System.Text;
using System.Text.Json;
using SmartFlow.Api.IntegrationTests.Infrastructure;

namespace SmartFlow.Api.IntegrationTests.Requests;

[Collection("Integration tests")]
public sealed class RequestsEndpointsTests(
    SmartFlowApiFactory factory)
    : IClassFixture<SmartFlowApiFactory>
{
    [Fact]
    public async Task CreateThenGet_ValidRequest_ReturnsCreatedRequest()
    {
        await factory.ResetDatabaseAsync();

        using var client = factory.CreateClient();

        const string requestBody = """
            {
              "title": "Purchase a laptop",
              "description": "A laptop is required for the development team.",
              "priority": "normal",
              "dueDate": "2026-10-01T12:00:00Z",
              "creatorId": "11111111-1111-1111-1111-111111111111"
            }
            """;

        using var createResponse = await client.PostAsync(
            "/api/requests",
            new StringContent(
                requestBody,
                Encoding.UTF8,
                "application/json"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content
            .ReadAsStringAsync();

        using var createdDocument = JsonDocument.Parse(createContent);

        var requestId = createdDocument.RootElement
            .GetProperty("id")
            .GetGuid();

        using var getResponse = await client.GetAsync(
            $"/api/requests/{requestId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getContent = await getResponse.Content.ReadAsStringAsync();

        using var requestDocument = JsonDocument.Parse(getContent);

        Assert.Equal(
            requestId,
            requestDocument.RootElement
                .GetProperty("id")
                .GetGuid());

        Assert.Equal(
            "Purchase a laptop",
            requestDocument.RootElement
                .GetProperty("title")
                .GetString());

        Assert.Equal(
            "draft",
            requestDocument.RootElement
                .GetProperty("status")
                .GetString());

        Assert.Equal(
            "normal",
            requestDocument.RootElement
                .GetProperty("priority")
                .GetString());
    }

    [Fact]
    public async Task GetById_UnknownRequest_ReturnsNotFound()
    {
        await factory.ResetDatabaseAsync();

        using var client = factory.CreateClient();

        using var response = await client.GetAsync(
            $"/api/requests/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidRequest_ReturnsBadRequest()
    {
        await factory.ResetDatabaseAsync();

        using var client = factory.CreateClient();

        const string requestBody = """
            {
              "title": "",
              "description": "",
              "priority": "normal",
              "creatorId": "00000000-0000-0000-0000-000000000000"
            }
            """;

        using var response = await client.PostAsync(
            "/api/requests",
            new StringContent(
                requestBody,
                Encoding.UTF8,
                "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Contains("Validation failed", responseContent);
    }
}