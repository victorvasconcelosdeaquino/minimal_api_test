using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api_test.Context;
using minimal_api_test.Entities;
using minimal_api_test.Services;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "minimal_api_test", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the Bearer scheme.
                        Enter 'Bearer'[space] and then your token in the text input below.
                        Example: Bearer 12345abcdef",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
});

var connection = builder.Configuration.GetConnectionString("Connection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connection, ServerVersion.AutoDetect(connection)));

builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

//login endpoint
app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
{
    if (userModel is null) return Results.BadRequest("Invalid login");

    if (userModel.UserName == "vaquino" && userModel.Password == "americas123")
    {
        var tokenString = tokenService.GenerateToken(
            app.Configuration["Jwt:Key"],
            app.Configuration["Jwt:Issuer"],
            app.Configuration["Jwt:Audience"],
            userModel);

        return Results.Ok(new { token = tokenString });
    }
    else return Results.BadRequest("Invalid login");
}).Produces(StatusCodes.Status400BadRequest)
  .Produces(StatusCodes.Status200OK)
  .WithName("Login")
  .WithTags("Authentication");

//categories endpoint
app.MapGet("/", () => "Product Catalog").ExcludeFromDescription();

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

//products endpoint
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();