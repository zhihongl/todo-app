using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp.Api;
using Xunit;

namespace TodoApp.Api.Tests;

public class TodoEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TodoEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyArray_OnFreshServer()
    {
        var response = await _client.GetAsync("/api/todos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<TodoItem[]>();
        Assert.NotNull(items);
    }

    [Fact]
    public async Task Create_ReturnsCreatedItem_WithNonEmptyTitle()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "Walk dog" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var item = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(item);
        Assert.Equal("Walk dog", item!.Title);
        Assert.False(item.IsDone);
        Assert.NotEqual(Guid.Empty, item.Id);
    }

    [Fact]
    public async Task Create_ReturnsValidationProblem_OnEmptyTitle()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new { title = "   " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_ForExistingItem()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new { title = "Throw out" });
        var item = await created.Content.ReadFromJsonAsync<TodoItem>();

        var response = await _client.DeleteAsync($"/api/todos/{item!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_ForMissingItem()
    {
        var response = await _client.DeleteAsync($"/api/todos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Patch_UpdatesIsDone_ForExistingItem()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new { title = "Read book" });
        var item = await created.Content.ReadFromJsonAsync<TodoItem>();

        var response = await _client.PatchAsJsonAsync($"/api/todos/{item!.Id}", new { isDone = true });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.True(updated!.IsDone);
    }

    [Fact]
    public async Task Patch_ReturnsNotFound_ForMissingItem()
    {
        var response = await _client.PatchAsJsonAsync(
            $"/api/todos/{Guid.NewGuid()}",
            new { isDone = true });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
