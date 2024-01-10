using System;
using System.Linq;
using System.Text;
using Core.Filter; 
using Core.Interface;
using Core.Profiles;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Seed;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder => policyBuilder
            .WithOrigins("http://127.0.0.1:5500") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());   
});

// Add services to the container.
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var redisConnectionString = builder.Configuration.GetConnectionString("RedisCacheUrl");

builder.Services.AddDbContext<DataContext>(conf=> conf.UseNpgsql(dbConnectionString));
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnectionString; });
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<CacheService>();
builder.Services.AddScoped<IStudentService,StudentService>();
builder.Services.AddScoped(typeof(CacheService),typeof(CacheService));
builder.Services.AddAutoMapper(typeof(InfrastructureProfile));
builder.Services.AddMemoryCache();
builder.Services.AddControllers();  

#region Identity & JWT
builder.Services.AddIdentity<IdentityUser<Guid>,IdentityRole<Guid>>(config =>
    {
        config.Password.RequiredLength = 6;
        config.Password.RequireDigit = false; // must have at least one digit
        config.Password.RequireNonAlphanumeric = false; // must have at least one non-alphanumeric character
        config.Password.RequireUppercase = false; // must have at least one uppercase character
        config.Password.RequireLowercase = false; // must have at least one lowercase character
    

    })
//for registering usermanager and signinmanger
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

  
//JWT Config
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    string? jwtKey = builder.Configuration["JWT:Key"];
    byte[]? key = null;
    if (jwtKey != null)
    {
       key = Encoding.ASCII.GetBytes(jwtKey);
    }
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


//Swagger Authentication Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AC REGISTOR WEB API",
        Version = "v1",
        Description = "Sample API Services.",
        Contact = new OpenApiContact
        {
            Name = "John Doe"
        },
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
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
            new string[] {}
        }
    });
});

#endregion
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#region Seed configurations
// Seed
try
{
    var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser<Guid>>>();
    var context = services.GetRequiredService<DataContext>();
    context.Database.Migrate();
    SeedData.Seed(context, userManager);
}
catch (Exception ex)
{
    app.Logger.LogError(ex.Message);
}
#endregion

using (var scope = app.Services.CreateScope())
{
    var cacheService = scope.ServiceProvider.GetRequiredService<CacheService>();
    await cacheService.SetCache();
}
app.UseHttpsRedirection();
app.UseRouting(); 
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers(); 

app.Run();
