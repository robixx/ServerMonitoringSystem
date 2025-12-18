using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SysMonitor.Infrastructure;
using SysMonitor.Infrastructure.Services;
using System.Net;
using System.Security.Policy;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------ DB Context ------------------
builder.Services.AddDbContext<DatabaseConnection>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ------------------ Authentication ------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is missing.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// ------------------ Controllers ------------------
builder.Services.AddControllers();

// ------------------ CORS ------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("Content-Disposition"));
});

// ------------------ OpenAPI (ASP.NET Core 10) ------------------
builder.Services.AddOpenApi();
builder.Services.InjectService();
builder.Services.AddSignalR();
var app = builder.Build();

// ------------------ Middleware ------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// ✅ Built-in OpenAPI endpoints

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapOpenApi();              // /openapi/v1.json
app.MapScalarApiReference();
app.MapHub<SystemMonitorHub>("/systemMonitorHub");
// Bind to LAN IP + HTTPS port
var lanIP = "192.168.11.148";
var httpsPort = 7149;

app.Urls.Clear();
app.Urls.Add($"https://{lanIP}:{httpsPort}");
app.Run();