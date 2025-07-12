using Cosmetics.Server.EntityFrameworkCore;
using Cosmetics.Server.Services;
using Cosmetics.Server.Managers;
using Cosmetics.Server.Repository;
using Cosmetics.Server.Models;
using Microsoft.EntityFrameworkCore;
using Cosmetics.Server.Managers.Images;
using Cosmetics.Server.Managers.Brands;
using Cosmetics.Server.Managers.Categories;
using Cosmetics.Server.Managers.Users;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add controllers with JSON loop handling
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Configure image storage service
var storageProvider = builder.Configuration["Storage:Provider"] ?? "Cloudinary";
if (string.Equals(storageProvider, "Local", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();
    builder.Services.AddDirectoryBrowser();
}
else
{
    builder.Services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
}

// PostgreSQL DbContext setup
builder.Services.AddDbContext<AMSDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories and Managers
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IBrandManager, BrandManager>();
builder.Services.AddScoped<ICategoryManager, CategoryManager>();
builder.Services.AddScoped<IImageManager, ImageManager>();
builder.Services.AddScoped<IUserManager, UserManager>();

builder.Services.AddHttpContextAccessor();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                throw new InvalidOperationException("JWT key is not configured.")))
    };
});

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Swagger with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Optional: only if you have this operation filter in your project
    // c.OperationFilter<AuthResponsesOperationFilter>();
});

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// CORS setup
string[] allowedOrigins;
if (builder.Environment.IsProduction())
{
    // Use environment variables for Cloudinary in production
    builder.Configuration["Cloudinary:CloudName"] = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
    builder.Configuration["Cloudinary:ApiKey"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
    builder.Configuration["Cloudinary:ApiSecret"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

    allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',') ??
                     new[] { "https://victorious-ground-01db06200.2.azurestaticapps.net" };
}
else
{
    allowedOrigins = new[] { "http://localhost:4200" };
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Serve static files if using local storage
if (string.Equals(storageProvider, "Local", StringComparison.OrdinalIgnoreCase))
{
    app.UseStaticFiles();

    if (app.Environment.IsDevelopment())
    {
        app.UseDirectoryBrowser();
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();

app.MapControllers();

app.Run();
