using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SMSVotingSystem.API.Middleware;
using SMSVotingSystem.Application.Interfaces;
using SMSVotingSystem.Application.Services;
using SMSVotingSystem.Domain.Repositories;
using SMSVotingSystem.Infrastructure.Persistence;
using SMSVotingSystem.Infrastructure.Repositories;
using SMSVotingSystem.Infrastructure.Services;
using System.Text;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Mvc;
using SMSVotingSystem.Domain.Services;
using SMSVotingSystem.Infrastructure.HealthChecks;
using Scalar.AspNetCore;
using SMSVotingSystem.Application.Common;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured. Please add the DefaultConnection setting to your configuration.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);


builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("Security"));

// builder.Services.AddScoped<IIdentityService, IdentityService>();
// Repositories
builder.Services.AddScoped<IVoterRepository, VoterRepository>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IElectionRepository, ElectionRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<ISmsLogRepository, SmsLogRepository>();
builder.Services.AddScoped<ITOTPRepository, TOTPRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Services
builder.Services.AddScoped<IVoterService, VoterService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IElectionService, ElectionService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ISmsLogService, SmsLogService>();
builder.Services.AddScoped<ISMSStatusService, SMSStatusService>();
builder.Services.AddScoped<ISMSHelpService, SMSHelpService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITOTPService, TOTPService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

// Infrastructure Services
builder.Services.AddScoped<ISmsService, TwilioSmsService>();
// builder.Services.AddScoped<ISmsService, SmsServiceMock>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;

    var jwtSecret = builder.Configuration["JWT:Secret"];
    if (string.IsNullOrEmpty(jwtSecret))
    {
        throw new InvalidOperationException("JWT Secret is not configured. Please add the JWT:Secret setting to your configuration.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});


// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("Database", tags: new[] { "database" })
    .AddCheck<TwilioHealthCheck>("Twilio", tags: new[] { "sms" });

// builder.Services.AddHealthChecksUI()
//     .AddInMemoryStorage();

// Controllers
builder.Services.AddControllers();

// builder.Configuration["AllowedOrigins:Angular"]
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder
            .WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SMS Voting System API",
        Version = "v1",
        Description = "API for SMS-based voting system",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@smsvoting.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
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
            new string[] { }
        }
    });
});
builder.Services.AddOpenApi();
// Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "SMS Voting System API";
        options.Theme = ScalarTheme.Saturn;
        options.Layout = ScalarLayout.Classic;
    });
    // app.UseSwagger();
    // app.UseSwaggerUI();
    // app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Custom exception handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health Checks endpoints
// app.MapHealthChecks("/health", new HealthCheckOptions
// {
//     ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
// });

// app.MapHealthChecksUI(options =>
// {
//     options.UIPath = "/health-ui";
// });

app.Run();