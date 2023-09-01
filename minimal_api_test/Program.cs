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

app.MapGet("/", () => "Product Catalog").ExcludeFromDescription();

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

app.MapPut("/categories/{id:int}", async (int id, Category category, AppDbContext context) =>
{
    if (category.Id != id) return Results.BadRequest();

    var cat = await context.Categories.FindAsync(id);

    if (cat is null) return Results.NotFound();

    cat.Name = category.Name;
    cat.Description = category.Description;
    await context.SaveChangesAsync();

    return Results.Ok(cat);
});

app.MapDelete("/categories/{id:int}", async (int id, AppDbContext context) =>
{
    var cat = await context.Categories.FindAsync(id);

    if (cat is null) return Results.NotFound();

    context.Remove(cat);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

//products endpoint
app.MapPost("/products", async (Product product, AppDbContext context) =>
{
    context.Products.Add(product);
    await context.SaveChangesAsync();

    return Results.Created($"/products/{product.Id}", product);
});

app.MapGet("/products", async (AppDbContext context) => await context.Products.ToListAsync());

app.MapGet("/products/{id:int}", async (int id, AppDbContext context) =>
{
    return await context.Products.FindAsync(id)
        is Product category ? Results.Ok(category) : Results.NotFound();
});

app.MapPut("/products/{id:int}", async (int id, Product product, AppDbContext context) =>
{
    if (product.Id != id) return Results.BadRequest();

    var cat = await context.Products.FindAsync(id);

    if (cat is null) return Results.NotFound();

    cat.Name = product.Name;
    cat.Description = product.Description;
    cat.Price = product.Price;
    cat.CategoryId = product.CategoryId;
    await context.SaveChangesAsync();

    return Results.Ok(cat);
});

app.MapDelete("/products/{id:int}", async (int id, AppDbContext context) =>
{
    var cat = await context.Products.FindAsync(id);

    if (cat is null) return Results.NotFound();

    context.Remove(cat);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();