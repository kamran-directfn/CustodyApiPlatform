using Directfn.Custody.ApiFramework.Authentication.TokenStore.SQLite;
using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Directfn.Custody.ApiFramework.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UseDirectfnCustodyApiFramework(this WebApplication app)
        {

            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetService<SQLiteAuthTokenStoreInitializer>();

                if (initializer is not null)
                {
                    initializer.InitializeAsync().GetAwaiter().GetResult();
                }
            }

            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            });

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Directfn Custody API v1");
                options.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();

            app.UseCors(ServiceCollectionExtensions.DefaultCorsPolicyName);

            app.UseAuthentication();
            app.UseMiddleware<FingerprintValidationMiddleware>();
            app.UseAuthorization();

            app.MapHealthChecks("/health");

            return app;
        }
    }
}
