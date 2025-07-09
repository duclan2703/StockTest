using Core;
using Microsoft.EntityFrameworkCore;
using Stock.Entity;
using Application.Extensions;
using Stock.Middleware;
using System.Text.Json;
using Stock.MapperProfile;
using Stock.Business;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMediator, Mediator>();
builder.Services.AddLogging();
builder.Services.AddMemoryCache();
builder.Services.AddRegistrationExtension();
builder.Services.AddDbContext<AdventureWorksDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultString")));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(MapperProfile.Initialize());
builder.Services.RegisterServices();

// Services
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Note API V1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
