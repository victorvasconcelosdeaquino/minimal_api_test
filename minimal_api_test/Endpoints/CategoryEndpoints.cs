using Microsoft.EntityFrameworkCore;
using minimal_api_test.Context;
using minimal_api_test.Entities;

namespace minimal_api_test.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        app.MapPost("/categories", async (Category category, AppDbContext context) =>
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return Results.Created($"/categories/{category.Id}", category);
        });

        app.MapGet("/categories", async (AppDbContext context) =>
            await context.Categories.ToListAsync()).RequireAuthorization();

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
    }
}
