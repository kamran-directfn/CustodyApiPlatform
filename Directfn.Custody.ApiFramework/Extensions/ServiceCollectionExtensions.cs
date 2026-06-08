using System.Text;
using Asp.Versioning;
using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Directfn.Custody.ApiFramework.Passwords;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Repositories.User;
namespace Directfn.Custody.ApiFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public const string DefaultCorsPolicyName = "DirectfnCustodyCorsPolicy";

    public static IServiceCollection AddDirectfnCustodyApiFramework(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<EntitlementActionFilter>();

        services.AddControllers(options =>
        {
            options.Filters.Add<EntitlementActionFilter>();
        });

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });


        services.AddHttpContextAccessor();
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));
        services.AddScoped<IDbConnectionFactory, OracleConnectionFactory>();
        services.AddScoped<IOracleDbManager, OracleDbManager>();
        services.AddScoped<IOracleDbManagerAsync, OracleDbManagerAsync>();


        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<ITokenFingerprintService, TokenFingerprintService>();
        services.AddSingleton<IPasswordHashService, AspNetPasswordHashService>();
        services.AddSingleton<ILegacyPasswordService, TripleDesLegacyPasswordService>();

        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddCors(options =>
        {
            options.AddPolicy(DefaultCorsPolicyName, policy =>
            {
                var allowedOrigins = configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? [];

                if (allowedOrigins.Length > 0)
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });

        services.AddEndpointsApiExplorer();

        AddJwtAuthentication(services, configuration);

        services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = configuration["Swagger:Title"] ?? "Directfn Custody API",
        Version = configuration["Swagger:Version"] ?? "v1",
        Description = configuration["Swagger:Description"] ?? "Directfn Custody API"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Description = "Enter full JWT header value. Example: Bearer eyJhbGciOiJIUzI1NiIs...",
        Name = "Authorization",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Type = Microsoft.OpenApi.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

        return services;
    }
    private static void AddJwtAuthentication(
    IServiceCollection services,
    IConfiguration configuration)
    {
        var authOptions = configuration
            .GetSection(AuthOptions.SectionName)
            .Get<AuthOptions>() ?? new AuthOptions();

        if (!authOptions.Enabled)
        {
            services.AddAuthorization();
            return;
        }

        if (string.IsNullOrWhiteSpace(authOptions.Issuer))
        {
            throw new InvalidOperationException("Authentication:Issuer is missing.");
        }

        if (string.IsNullOrWhiteSpace(authOptions.Audience))
        {
            throw new InvalidOperationException("Authentication:Audience is missing.");
        }

        if (string.IsNullOrWhiteSpace(authOptions.SigningKey))
        {
            throw new InvalidOperationException("Authentication:SigningKey is missing for development JWT validation.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(authOptions.SigningKey);

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException("Authentication:SigningKey must be at least 32 bytes.");
        }

        var signingKey = new SymmetricSecurityKey(keyBytes);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = authOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();
    }
}