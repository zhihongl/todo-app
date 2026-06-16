namespace TodoApp.Api.Services;

public interface ITodoService
{
    IReadOnlyList<TodoItem> GetAll();
    TodoItem Add(string title);
    bool Delete(Guid id);
    TodoItem? Toggle(Guid id, bool isDone);
}
