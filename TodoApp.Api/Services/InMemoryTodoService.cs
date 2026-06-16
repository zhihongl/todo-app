using System.Collections.Concurrent;

namespace TodoApp.Api.Services;

public sealed class InMemoryTodoService : ITodoService
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();
    private readonly List<Guid> _order = new();
    private readonly object _orderLock = new();

    public IReadOnlyList<TodoItem> GetAll()
    {
        lock (_orderLock)
        {
            return _order
                .Select(id => _items[id])
                .ToList();
        }
    }

    public TodoItem Add(string title)
    {
        var item = new TodoItem(
            Guid.NewGuid(),
            title,
            IsDone: false,
            CreatedAt: DateTimeOffset.UtcNow);

        if (_items.TryAdd(item.Id, item))
        {
            lock (_orderLock)
            {
                _order.Add(item.Id);
            }
        }

        return item;
    }

    public bool Delete(Guid id)
    {
        var removed = _items.TryRemove(id, out _);
        if (removed)
        {
            lock (_orderLock)
            {
                _order.Remove(id);
            }
        }
        return removed;
    }

    public TodoItem? Toggle(Guid id, bool isDone)
    {
        if (!_items.TryGetValue(id, out var existing))
        {
            return null;
        }

        var updated = existing with { IsDone = isDone };
        _items[id] = updated;
        return updated;
    }
}
