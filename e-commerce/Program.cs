using e_commerce.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var connectionString = $"server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"user={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

builder.Services.AddDbContext<ECommerceDbContext>(opt =>
    opt.UseMySQL(connectionString));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();