using Microsoft.EntityFrameworkCore;
using minimal_api_test.Context;
using minimal_api_test.Entities;

namespace minimal_api_test.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        app.MapPost("/products", async (Product product, AppDbContext context) =>
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Results.Created($"/products/{product.Id}", product);
        });

        app.MapGet("/products", async (AppDbContext context) =>
            await context.Products.ToListAsync()).RequireAuthorization();

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
    }
}
