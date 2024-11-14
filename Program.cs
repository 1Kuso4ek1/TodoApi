using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) // Если приложение в разработке
{
    app.UseOpenApi();

    // Подключаем Swagger UI
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger"; // http://localhost:****/swagger - адрес, по которому можно получить доступ к интерфейсу
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

// Запрос GET                  база данных
app.MapGet("/todoitems", async (TodoDb db) => await db.Todos.ToListAsync());

// Запрос POST                   новый объект, база данных
app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo); // Добавление нового объекта в БД
    await db.SaveChangesAsync(); // Сохранение изменений

    return Results.Created($"/todoitems/{todo.Id}", todo); // Возврат созданного объекта в качестве результата
});

app.Run();
