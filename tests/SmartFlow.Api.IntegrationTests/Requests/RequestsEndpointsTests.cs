using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SmartFlow.Api.IntegrationTests.Infrastructure;
using SmartFlow.Application.Common.Security;
using SmartFlow.Infrastructure.Identity;

namespace SmartFlow.Api.IntegrationTests.Requests;

[Collection("Integration tests")]
public sealed class RequestsEndpointsTests(
    SmartFlowApiFactory factory)
    : IClassFixture<SmartFlowApiFactory>
{
    [Fact]
    public async Task CreateThenGet_ValidRequest_ReturnsCreatedDraft()
    {
        await factory.ResetDatabaseAsync();

        var creator = await CreateAuthenticatedUserAsync(
            Roles.Collaborateur);

        using var client = creator.Client;

        var requestId = await CreateDraftAsync(
            client,
            "Purchase a laptop");

        var request = await GetRequestAsync(client, requestId);

        Assert.Equal("Purchase a laptop", request.Title);
        Assert.Equal("draft", request.Status);
        Assert.Equal(creator.UserId, request.CreatorId);
        Assert.True(request.Version > 0);
        Assert.Empty(request.Comments);
        Assert.Empty(request.Attachments);
    }

    [Fact]
    public async Task CompleteWorkflow_AssignedManager_CanApproveRequest()
    {
        await factory.ResetDatabaseAsync();

        var creator = await CreateAuthenticatedUserAsync(
            Roles.Collaborateur);

        var manager = await CreateAuthenticatedUserAsync(
            Roles.Manager);

        var administrator = await CreateAuthenticatedUserAsync(
            Roles.Administrateur);

        using var creatorClient = creator.Client;
        using var managerClient = manager.Client;
        using var administratorClient = administrator.Client;

        var requestId = await CreateDraftAsync(
            creatorClient,
            "Purchase a laptop");

        var request = await GetRequestAsync(
            creatorClient,
            requestId);

        using var submitResponse = await PostJsonAsync(
            creatorClient,
            $"/api/requests/{requestId}/submit",
            $$"""
            {
              "version": {{request.Version}}
            }
            """);

        Assert.Equal(HttpStatusCode.NoContent, submitResponse.StatusCode);

        request = await GetRequestAsync(creatorClient, requestId);

        using var assignResponse = await PostJsonAsync(
            administratorClient,
            $"/api/requests/{requestId}/assign-manager",
            $$"""
            {
              "managerId": "{{manager.UserId}}",
              "version": {{request.Version}}
            }
            """);

        Assert.Equal(HttpStatusCode.NoContent, assignResponse.StatusCode);

        request = await GetRequestAsync(creatorClient, requestId);

        using var startReviewResponse = await PostJsonAsync(
            managerClient,
            $"/api/requests/{requestId}/start-review",
            $$"""
            {
              "version": {{request.Version}}
            }
            """);

        Assert.Equal(
            HttpStatusCode.NoContent,
            startReviewResponse.StatusCode);

        request = await GetRequestAsync(creatorClient, requestId);

        using var approveResponse = await PostJsonAsync(
            managerClient,
            $"/api/requests/{requestId}/approve",
            $$"""
            {
              "decisionComment": "All requirements are met.",
              "version": {{request.Version}}
            }
            """);

        Assert.Equal(HttpStatusCode.NoContent, approveResponse.StatusCode);

        request = await GetRequestAsync(creatorClient, requestId);

        Assert.Equal("approved", request.Status);
        Assert.NotNull(request.DecisionAtUtc);
        Assert.Equal(3, request.ApprovalHistories.Count);
    }

    [Fact]
    public async Task Update_WithOldVersion_ReturnsConflict()
    {
        await factory.ResetDatabaseAsync();

        var creator = await CreateAuthenticatedUserAsync(
            Roles.Collaborateur);

        using var client = creator.Client;

        var requestId = await CreateDraftAsync(
            client,
            "Original title");

        var request = await GetRequestAsync(client, requestId);

        var oldVersion = request.Version;

        using var firstUpdateResponse = await PutJsonAsync(
            client,
            $"/api/requests/{requestId}",
            $$"""
            {
              "title": "First update",
              "description": "A laptop is required for the development team.",
              "priority": "high",
              "dueDate": "2026-12-01T12:00:00Z",
              "version": {{oldVersion}}
            }
            """);

        Assert.Equal(
            HttpStatusCode.NoContent,
            firstUpdateResponse.StatusCode);

        using var outdatedUpdateResponse = await PutJsonAsync(
            client,
            $"/api/requests/{requestId}",
            $$"""
            {
              "title": "Outdated update",
              "description": "A laptop is required for the development team.",
              "priority": "normal",
              "dueDate": "2026-12-01T12:00:00Z",
              "version": {{oldVersion}}
            }
            """);

        Assert.Equal(
            HttpStatusCode.Conflict,
            outdatedUpdateResponse.StatusCode);
    }

    [Fact]
    public async Task Comments_AuthorCanAddUpdateAndRemoveComment()
    {
        await factory.ResetDatabaseAsync();

        var creator = await CreateAuthenticatedUserAsync(
            Roles.Collaborateur);

        using var client = creator.Client;

        var requestId = await CreateDraftAsync(
            client,
            "Comment test");

        var request = await GetRequestAsync(client, requestId);

        using var addResponse = await PostJsonAsync(
            client,
            $"/api/requests/{requestId}/comments",
            $$"""
            {
              "content": "Please attach the invoice.",
              "version": {{request.Version}}
            }
            """);

        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);

        var commentId = await GetResponseIdAsync(addResponse);

        request = await GetRequestAsync(client, requestId);

        Assert.Single(request.Comments);

        using var updateResponse = await PutJsonAsync(
            client,
            $"/api/requests/{requestId}/comments/{commentId}",
            $$"""
            {
              "content": "Please attach the invoice and approval email.",
              "version": {{request.Version}}
            }
            """);

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        request = await GetRequestAsync(client, requestId);

        Assert.Equal(
            "Please attach the invoice and approval email.",
            request.Comments.Single().Content);

        using var deleteResponse = await DeleteJsonAsync(
            client,
            $"/api/requests/{requestId}/comments/{commentId}",
            $$"""
            {
              "version": {{request.Version}}
            }
            """);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        request = await GetRequestAsync(client, requestId);

        Assert.Empty(request.Comments);
    }

    [Fact]
    public async Task Attachments_UploaderCanUploadDownloadAndRemoveFile()
    {
        await factory.ResetDatabaseAsync();

        var creator = await CreateAuthenticatedUserAsync(
            Roles.Collaborateur);

        using var client = creator.Client;

        var requestId = await CreateDraftAsync(
            client,
            "Attachment test");

        var request = await GetRequestAsync(client, requestId);

        using var form = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent(
            Encoding.UTF8.GetBytes("SmartFlow attachment content."));

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("text/plain");

        form.Add(fileContent, "file", "note.txt");

        form.Add(
            new StringContent(request.Version.ToString()),
            "version");

        using var uploadResponse = await client.PostAsync(
            $"/api/requests/{requestId}/attachments",
            form);

        Assert.Equal(HttpStatusCode.Created, uploadResponse.StatusCode);

        var attachmentId = await GetResponseIdAsync(uploadResponse);

        request = await GetRequestAsync(client, requestId);

        Assert.Single(request.Attachments);

        using var downloadResponse = await client.GetAsync(
            $"/api/requests/{requestId}/attachments/{attachmentId}/download");

        Assert.Equal(HttpStatusCode.OK, downloadResponse.StatusCode);

        var content = await downloadResponse.Content.ReadAsStringAsync();

        Assert.Equal("SmartFlow attachment content.", content);

        using var deleteResponse = await DeleteJsonAsync(
            client,
            $"/api/requests/{requestId}/attachments/{attachmentId}",
            $$"""
            {
              "version": {{request.Version}}
            }
            """);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        request = await GetRequestAsync(client, requestId);

        Assert.Empty(request.Attachments);
    }

    [Fact]
    public async Task GetList_WithSearchAndPagination_ReturnsFilteredPage()
    {
        await factory.ResetDatabaseAsync();

        var creator = await CreateAuthenticatedUserAsync(
            Roles.Collaborateur);

        using var client = creator.Client;

        await CreateDraftAsync(client, "Laptop purchase");
        await CreateDraftAsync(client, "Laptop renewal");
        await CreateDraftAsync(client, "Travel expenses");

        using var response = await client.GetAsync(
            "/api/requests?search=laptop&page=1&pageSize=1&sortBy=title&sortDirection=asc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var root = document.RootElement;

        Assert.Equal(1, root.GetProperty("page").GetInt32());
        Assert.Equal(1, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(2, root.GetProperty("totalCount").GetInt32());
        Assert.Equal(2, root.GetProperty("totalPages").GetInt32());
        Assert.Single(root.GetProperty("items").EnumerateArray());
    }

    private async Task<TestUser> CreateAuthenticatedUserAsync(string role)
    {
        var client = factory.CreateClient();

        var uniqueValue = Guid.NewGuid().ToString("N");
        var email = $"user-{uniqueValue}@smartflow.test";

        using var registerResponse = await PostJsonAsync(
            client,
            "/api/auth/register",
            $$"""
            {
              "fullName": "Integration Test User",
              "email": "{{email}}",
              "password": "SmartFlow2026"
            }
            """);

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        using var registerDocument = JsonDocument.Parse(
            await registerResponse.Content.ReadAsStringAsync());

        var userId = registerDocument.RootElement
            .GetProperty("userId")
            .GetGuid();

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(email);

            Assert.NotNull(user);

            var currentRoles = await userManager.GetRolesAsync(user);

            if (currentRoles.Count > 0)
            {
                var removeResult = await userManager.RemoveFromRolesAsync(
                    user,
                    currentRoles);

                Assert.True(removeResult.Succeeded);
            }

            var addResult = await userManager.AddToRoleAsync(user, role);

            Assert.True(addResult.Succeeded);
        }

        using var loginResponse = await PostJsonAsync(
            client,
            "/api/auth/login",
            $$"""
            {
              "email": "{{email}}",
              "password": "SmartFlow2026"
            }
            """);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        using var loginDocument = JsonDocument.Parse(
            await loginResponse.Content.ReadAsStringAsync());

        var accessToken = loginDocument.RootElement
            .GetProperty("accessToken")
            .GetString();

        Assert.False(string.IsNullOrWhiteSpace(accessToken));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        return new TestUser(client, userId);
    }

    private static async Task<Guid> CreateDraftAsync(
        HttpClient client,
        string title)
    {
        using var response = await PostJsonAsync(
            client,
            "/api/requests",
            $$"""
            {
              "title": "{{title}}",
              "description": "Business request created for integration testing.",
              "priority": "normal",
              "dueDate": "2026-12-01T12:00:00Z"
            }
            """);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return await GetResponseIdAsync(response);
    }

    private static async Task<RequestSnapshot> GetRequestAsync(
        HttpClient client,
        Guid requestId)
    {
        using var response = await client.GetAsync(
            $"/api/requests/{requestId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var root = document.RootElement;

        var comments = root.GetProperty("comments")
            .EnumerateArray()
            .Select(item => new CommentSnapshot(
                item.GetProperty("id").GetGuid(),
                item.GetProperty("content").GetString()!))
            .ToList();

        var attachments = root.GetProperty("attachments")
            .EnumerateArray()
            .Select(item => item.GetProperty("id").GetGuid())
            .ToList();

        var histories = root.GetProperty("approvalHistories")
            .EnumerateArray()
            .Select(item => item.GetProperty("id").GetGuid())
            .ToList();

        return new RequestSnapshot(
            root.GetProperty("id").GetGuid(),
            root.GetProperty("title").GetString()!,
            root.GetProperty("status").GetString()!,
            root.GetProperty("version").GetUInt32(),
            root.GetProperty("creatorId").GetGuid(),
            root.GetProperty("decisionAtUtc").ValueKind ==
                JsonValueKind.Null
                ? null
                : root.GetProperty("decisionAtUtc").GetDateTime(),
            comments,
            attachments,
            histories);
    }

    private static async Task<Guid> GetResponseIdAsync(
        HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        return document.RootElement
            .GetProperty("id")
            .GetGuid();
    }

    private static Task<HttpResponseMessage> PostJsonAsync(
        HttpClient client,
        string path,
        string content)
    {
        return client.PostAsync(
            path,
            new StringContent(
                content,
                Encoding.UTF8,
                "application/json"));
    }

    private static Task<HttpResponseMessage> PutJsonAsync(
        HttpClient client,
        string path,
        string content)
    {
        return client.PutAsync(
            path,
            new StringContent(
                content,
                Encoding.UTF8,
                "application/json"));
    }

    private static Task<HttpResponseMessage> DeleteJsonAsync(
        HttpClient client,
        string path,
        string content)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Delete,
            path)
        {
            Content = new StringContent(
                content,
                Encoding.UTF8,
                "application/json")
        };

        return client.SendAsync(request);
    }

    private sealed record TestUser(
        HttpClient Client,
        Guid UserId);

    private sealed record RequestSnapshot(
        Guid Id,
        string Title,
        string Status,
        uint Version,
        Guid CreatorId,
        DateTime? DecisionAtUtc,
        IReadOnlyCollection<CommentSnapshot> Comments,
        IReadOnlyCollection<Guid> Attachments,
        IReadOnlyCollection<Guid> ApprovalHistories);

    private sealed record CommentSnapshot(
        Guid Id,
        string Content);
}