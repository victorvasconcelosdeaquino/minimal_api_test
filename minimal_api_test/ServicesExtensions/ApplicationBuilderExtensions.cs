namespace minimal_api_test.ServicesExtensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app,
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment()) app.UseDeveloperExceptionPage();

        return app;
    }

    public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
    {
        app.UseCors(c =>
        {
            c.AllowAnyOrigin();
            c.WithMethods("GET");
            c.AllowAnyHeader();
        });

        return app;
    }

    public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
