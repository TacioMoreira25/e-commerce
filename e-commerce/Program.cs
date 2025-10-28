using System.Text;
using e_commerce.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using e_commerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new
        TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))
        });

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();