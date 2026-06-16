using TodoApp.Api.Endpoints;
using TodoApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITodoService, InMemoryTodoService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapTodoEndpoints();

app.Run();

public partial class Program;
