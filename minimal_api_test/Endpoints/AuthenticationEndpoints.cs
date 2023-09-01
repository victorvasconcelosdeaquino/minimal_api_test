using Microsoft.AspNetCore.Authorization;
using minimal_api_test.Entities;
using minimal_api_test.Services;

namespace minimal_api_test.Endpoints;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
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

    }
}
