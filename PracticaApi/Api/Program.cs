using Api.Modules;
using Api.Services.UserProvider;
using Application;
using Application.Common.Interfaces;
using Application.Middlewares;
using DataAccessLayer;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserProvider, UserProvider>();

builder.Services.AddApplication(builder);
builder.Services.AddInfrastructure(builder);
builder.Services.SetupServices();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options
    .WithOrigins(new[] { "http://localhost:5173" })
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

app.UseAuthentication();
app.UseAuthorization();

await app.InitialiseDb();
app.MapControllers();

var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "data/images");

if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

app.UseMiddleware<MiddlewareExceptionHandling>();


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/images"
});

// app.SeedData();

app.Run();

namespace Api
{
    public partial class Program;
}