using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Services;

namespace TodoApp.Api.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todos").WithTags("Todos");

        group.MapGet("/", GetAll);
        group.MapPost("/", Create);
        group.MapDelete("/{id:guid}", Delete);
        group.MapPatch("/{id:guid}", Update);
    }

    public static Ok<IReadOnlyList<TodoItem>> GetAll(ITodoService service)
        => TypedResults.Ok(service.GetAll());

    public static Results<Created<TodoItem>, ValidationProblem> Create(
        [FromBody] CreateTodoRequest request,
        ITodoService service)
    {
        var title = (request.Title ?? string.Empty).Trim();
        if (title.Length == 0)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["title"] = new[] { "Title is required." }
            });
        }

        var item = service.Add(title);
        return TypedResults.Created($"/api/todos/{item.Id}", item);
    }

    public static Results<NoContent, NotFound> Delete(Guid id, ITodoService service)
        => service.Delete(id)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();

    public static Results<Ok<TodoItem>, NotFound> Update(
        Guid id,
        [FromBody] UpdateTodoRequest request,
        ITodoService service)
    {
        var updated = service.Toggle(id, request.IsDone);
        return updated is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(updated);
    }
}

public sealed record CreateTodoRequest(string? Title);

public sealed record UpdateTodoRequest(bool IsDone);
