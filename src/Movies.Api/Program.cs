using Movies.Api.Extensions;
using Movies.Api.Middlewares;
using Movies.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddPresentation()
    .AddInfrastructure()
    .AddDatabase(config["Database:ConnectionString"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();
