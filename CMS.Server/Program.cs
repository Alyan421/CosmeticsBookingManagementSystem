using CMS.Server.EntityFrameworkCore;
using CMS.Server.Services;
using CMS.Server.Managers;
using CMS.Server.Repository;
using CMS.Server.Models;
using Microsoft.EntityFrameworkCore;
using CMS.Server.Managers.Images;
using CMS.Server.Managers.Cloths;
using CMS.Server.Managers.Colors;
using CMS.Server.Managers.Users;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Configure image storage service based on configuration
var storageProvider = builder.Configuration["Storage:Provider"] ?? "Cloudinary";
if (string.Equals(storageProvider, "Local", StringComparison.OrdinalIgnoreCase))
{
    // Register local file storage
    builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();

    // Enable static files for serving local images
    builder.Services.AddDirectoryBrowser();
}
else
{
    // Register Cloudinary storage (default)
    builder.Services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
}

// Configure DbContext with SQL Server
builder.Services.AddDbContext<AMSDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repository and Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Register ImageManager
builder.Services.AddScoped<IClothManager, ClothManager>();
builder.Services.AddScoped<IColorManager, ColorManager>();
builder.Services.AddScoped<IImageManager, ImageManager>();
builder.Services.AddScoped<IUserManager, UserManager>();

builder.Services.AddHttpContextAccessor();

// Configure JWT Authentication
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

// Rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configure Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });

    // Add JWT Authentication to Swagger
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
    c.OperationFilter<AuthResponsesOperationFilter>();
});

// Register Automappers
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Handle environment-specific configurations
if (builder.Environment.IsProduction())
{
    // Override Cloudinary configuration with environment variables
    builder.Configuration["Cloudinary:CloudName"] = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
    builder.Configuration["Cloudinary:ApiKey"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
    builder.Configuration["Cloudinary:ApiSecret"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

    var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',') ?? new[] { "https://green-tree-0e8213e00.2.azurestaticapps.net" };
    // Add CORS services with environment-based configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins(allowedOrigins) // This allows requests from any origin
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });
}
else
{
    // Development CORS configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
}

var app = builder.Build();

// Enable static files for local image storage if using local provider
if (string.Equals(storageProvider, "Local", StringComparison.OrdinalIgnoreCase))
{
    app.UseStaticFiles(); // Enable serving static files

    // Optional: Enable directory browsing for debugging purposes
    if (app.Environment.IsDevelopment())
    {
        app.UseDirectoryBrowser();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS before other middleware
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();

app.MapControllers();

app.Run();