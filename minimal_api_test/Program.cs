using minimal_api_test.Endpoints;
using minimal_api_test.ServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApiSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAuthenticationJwt();

var app = builder.Build();

//login endpoint
app.MapAuthenticationEndpoints();

//categories endpoint
app.MapCategoryEndpoints();

//products endpoint
app.MapProductEndpoints();

// Configure the HTTP request pipeline.
var environment = app.Environment;
app.UseExceptionHandler(environment)
   .UseSwaggerMiddleware()
   .UseAppCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();