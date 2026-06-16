using TodoApp.Api.Services;
using Xunit;

namespace TodoApp.Api.Tests;

public class InMemoryTodoServiceTests
{
    [Fact]
    public void Add_PersistsItem_AndAssignsId()
    {
        var service = new InMemoryTodoService();

        var item = service.Add("Buy milk");

        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal("Buy milk", item.Title);
        Assert.False(item.IsDone);
        Assert.True(item.CreatedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void GetAll_PreservesInsertionOrder()
    {
        var service = new InMemoryTodoService();
        var first = service.Add("first");
        var second = service.Add("second");
        var third = service.Add("third");

        var items = service.GetAll();

        Assert.Equal(new[] { first.Id, second.Id, third.Id }, items.Select(i => i.Id));
    }

    [Fact]
    public void Delete_RemovesItem_AndReturnsTrue()
    {
        var service = new InMemoryTodoService();
        var item = service.Add("Buy milk");

        var result = service.Delete(item.Id);

        Assert.True(result);
        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void Delete_ReturnsFalse_WhenItemMissing()
    {
        var service = new InMemoryTodoService();

        var result = service.Delete(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public void Toggle_UpdatesIsDone_AndReturnsNewValue()
    {
        var service = new InMemoryTodoService();
        var item = service.Add("Buy milk");

        var updated = service.Toggle(item.Id, true);

        Assert.NotNull(updated);
        Assert.True(updated!.IsDone);
        Assert.True(service.GetAll().Single().IsDone);
    }

    [Fact]
    public void Toggle_ReturnsNull_WhenItemMissing()
    {
        var service = new InMemoryTodoService();

        var updated = service.Toggle(Guid.NewGuid(), true);

        Assert.Null(updated);
    }
}
