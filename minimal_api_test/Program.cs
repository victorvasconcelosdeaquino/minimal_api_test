using Microsoft.EntityFrameworkCore;
using minimal_api_test.Context;
using minimal_api_test.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("Connection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connection, ServerVersion.AutoDetect(connection)));

var app = builder.Build();

app.MapGet("/", () => "Product Catalog");

app.MapPost("/categories", async (Category category, AppDbContext context) =>
{
    context.Categories.Add(category);
    await context.SaveChangesAsync();

    return Results.Created($"/categories/{category.Id}", category);
});

app.MapGet("/categories", async (AppDbContext context) => await context.Categories.ToListAsync());

app.MapGet("/categories/{id:int}", async (int id, AppDbContext context) =>
{
    return await context.Categories.FindAsync(id) 
        is Category category ? Results.Ok(category) : Results.NotFound();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();