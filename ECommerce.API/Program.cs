using ECommerce.API.Middleware;
using ECommerce.Application;
using ECommerce.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    //c.RoutePrefix = string.Empty; // Uncomment if you want Swagger UI at the root URL
});

app.UseStaticFiles();

app.UseSession();

app.UseCors("AllowFrontEnd");

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.UseMiddleware<AuthenticationHandlingMiddleware>();

app.MapControllers();

app.Run();
