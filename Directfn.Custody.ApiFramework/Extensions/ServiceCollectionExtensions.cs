using Asp.Versioning;
using Directfn.Custody.ApiFramework.Approvals;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Auditing.Oracle;
using Directfn.Custody.ApiFramework.Auditing.SQLite;
using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Authentication.TokenStore;
using Directfn.Custody.ApiFramework.Authentication.TokenStore.Oracle;
using Directfn.Custody.ApiFramework.Authentication.TokenStore.SQLite;
using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Menus;
using Directfn.Custody.ApiFramework.Passwords;
using Directfn.Custody.ApiFramework.Repositories.Operations;
using Directfn.Custody.ApiFramework.Repositories.User;
using Directfn.Custody.ApiFramework.Security;
using Directfn.Custody.ApiFramework.Sessions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json;

namespace Directfn.Custody.ApiFramework.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const string DefaultCorsPolicyName = "DirectfnCustodyCorsPolicy";

        public static IServiceCollection AddDirectfnCustodyApiFramework(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<EntitlementActionFilter>();

            services.AddControllers(options =>
            {
                options.Filters.Add<AuditActionFilter>();
                options.Filters.Add<EntitlementActionFilter>();
                options.Filters.Add<OperationApprovalActionFilter>();
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });


            services.AddHttpContextAccessor();
            services.AddDataProtection();
            services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));
            services.Configure<AuthTokenStoreOptions>(configuration.GetSection(AuthTokenStoreOptions.SectionName));
            var authTokenStoreOptions = configuration.GetSection(AuthTokenStoreOptions.SectionName).Get<AuthTokenStoreOptions>() ?? new AuthTokenStoreOptions();

            if (authTokenStoreOptions.Provider == AuthTokenStoreProvider.SQLite)
            {
                services.AddScoped<IAuthTokenStore, SQLiteAuthTokenStore>();
                services.AddSingleton<SQLiteAuthTokenStoreInitializer>();
            }
            else if (authTokenStoreOptions.Provider == AuthTokenStoreProvider.Oracle)
            {
                services.AddScoped<IAuthTokenStore, OracleAuthTokenStore>();
            }
            else
            {
                throw new InvalidOperationException($"Unsupported auth token store provider: {authTokenStoreOptions.Provider}");
            }

            services.AddScoped<IDbConnectionFactory, OracleConnectionFactory>();
            services.AddScoped<IOracleDbManager, OracleDbManager>();
            services.AddScoped<IOracleDbManagerAsync, OracleDbManagerAsync>();

            services.Configure<AuditOptions>(configuration.GetSection(AuditOptions.SectionName));

            var auditOptions = configuration.GetSection(AuditOptions.SectionName).Get<AuditOptions>() ?? new AuditOptions();

            if (!auditOptions.Enabled || auditOptions.Provider == AuditStoreProvider.None)
            {
                services.AddScoped<IAuditWriter, NullAuditWriter>();
            }
            else if (auditOptions.Provider == AuditStoreProvider.SQLite)
            {
                services.AddScoped<IAuditWriter, SQLiteAuditWriter>();
                services.AddSingleton<SQLiteAuditStoreInitializer>();
            }
            else if (auditOptions.Provider == AuditStoreProvider.Oracle)
            {
                services.AddScoped<IAuditWriter, OracleAuditWriter>();
            }
            else
            {
                throw new InvalidOperationException($"Unsupported audit store provider: {auditOptions.Provider}");
            }

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddSingleton<ITokenFingerprintService, TokenFingerprintService>();
            services.AddScoped<IAuthSessionService, AuthSessionService>();
            services.AddScoped<IRefreshTokenService, DataProtectionRefreshTokenService>();
            services.AddSingleton<IPasswordHashService, AspNetPasswordHashService>();
            services.AddSingleton<ILegacyPasswordService, TripleDesLegacyPasswordService>();

            services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ILeftMenuBuilder, LeftMenuBuilder>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOperationApprovalRepository, OperationApprovalRepository>();
            services.AddScoped<OperationApprovalActionFilter>();

            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, policy =>
                {
                    string[] allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

                    if (allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                    }
                });
            });

            services.AddEndpointsApiExplorer();

            AddJwtAuthentication(services, configuration);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = configuration["Swagger:Title"] ?? "Directfn Custody API", Version = configuration["Swagger:Version"] ?? "v1", Description = configuration["Swagger:Description"] ?? "Directfn Custody API" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter full JWT header value. Example: Bearer eyJhbGciOiJIUzI1NiIs...",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement { { new OpenApiSecuritySchemeReference("Bearer", document), new List<string>() } });
            });

            services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

            return services;
        }

        private static void AddJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            AuthOptions authOptions = configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>() ?? new AuthOptions();

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

            byte[] keyBytes = Encoding.UTF8.GetBytes(authOptions.SigningKey);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException("Authentication:SigningKey must be at least 32 bytes.");
            }

            SymmetricSecurityKey signingKey = new(keyBytes);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("JWT token validated successfully.");

                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"JWT authentication failed: {context.Exception.Message}");

                        return Task.CompletedTask;
                    },

                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        string authorizationHeader = context.Request.Headers.Authorization.ToString();

                        bool hasAuthorizationHeader = !string.IsNullOrWhiteSpace(authorizationHeader);

                        bool startsWithBearer = authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

                        string message = "Authentication failed.";

                        if (!string.IsNullOrWhiteSpace(context.ErrorDescription))
                        {
                            message = context.ErrorDescription;
                        }
                        else if (!string.IsNullOrWhiteSpace(context.Error))
                        {
                            message = context.Error;
                        }

                        var response = new
                        {
                            Success = false,
                            Errors = new[]
                            {
            new
            {
                Code = "AUTHENTICATION_FAILED",
                Message = message
            }
        },
                            Debug = new
                            {
                                HasAuthorizationHeader = hasAuthorizationHeader,
                                StartsWithBearer = startsWithBearer,
                                AuthorizationHeaderLength = authorizationHeader.Length
                            }
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    },

                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            Success = false,
                            Errors = new[]
                            {
                new
                {
                    Code = "FORBIDDEN",
                    Message = "You are authenticated, but you do not have permission to access this resource."
                }
            }
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                };

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
}
