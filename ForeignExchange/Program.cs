using ForeignExchange.Application.Services;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Infrastructure.Data;
using ForeignExchange.Infrastructure.Repositories;
using ForeignExchange.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ForeignExchange.Application.EventHandlers;
using ForeignExchange.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Validations;

var builder = WebApplication.CreateBuilder(args);

// Set up the database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddFluentValidationAutoValidation();
// Register application services
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IForexProviderRepository, AlphaVantageRepository>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IValidationService, ValidationService>();

// Register application validations
builder.Services.AddScoped<IValidator<UserDTO>, UserValidator>();
builder.Services.AddScoped<IValidator<LoginDTO>, LoginValidator>();
builder.Services.AddScoped<IValidator<string>, StringValidator>();
builder.Services.AddScoped<IValidator<ExchangeRateDTO>, ExchangeRateValidator>();


// Register MessageService and Event Handlers
builder.Services.AddScoped<IMessageService, AzureServiceBusService>(); 
builder.Services.AddScoped<IEventHandler<ExchangeRateUpdatedEvent>, ExchangeRateUpdatedEventHandler>();

// Configure JWT authentication
builder.Services.AddScoped<IAuthService, AuthService>();

// Register controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ForeignExchange API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \n\n" +
                      "Enter 'Bearer' [space] and then your token in the text input below.\n\n" +
                      "Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.EnableAnnotations();
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
            new string[] { } }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
        ClockSkew = TimeSpan.Zero,
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError($"Authentication failed: {context.Exception.Message}");
            logger.LogError($"Authentication failed: {context.Exception.StackTrace}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validated successfully.");
            return Task.CompletedTask;
        },
        OnChallenge = context => {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError($"Authentication challenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }

    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Start Event Listener
using (var scope = app.Services.CreateScope())
{
    var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
    var eventHandler = scope.ServiceProvider.GetRequiredService<IEventHandler<ExchangeRateUpdatedEvent>>();

    // Subscribe to the event handler
    messageService.Subscribe(eventHandler);

    // Start listening for messages
    await Task.Run(() => messageService.StartListeningAsync());
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ForeignExchange API v1"));
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
