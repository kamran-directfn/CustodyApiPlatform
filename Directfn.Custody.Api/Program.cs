using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Extensions;
using Directfn.Custody.Api.Security;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddDirectfnCustodyLogging();

builder.Services.AddDirectfnCustodyApiFramework(builder.Configuration);
builder.Services.AddScoped<IEntitlementService, FakeEntitlementService>();

WebApplication app = builder.Build();

app.UseDirectfnCustodyApiFramework();

app.MapControllers();

app.Run();
