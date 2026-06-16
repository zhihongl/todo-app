namespace TodoApp.Api;

public sealed record TodoItem(Guid Id, string Title, bool IsDone, DateTimeOffset CreatedAt);
