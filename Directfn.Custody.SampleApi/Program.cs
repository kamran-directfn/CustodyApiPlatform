using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Extensions;
using Directfn.Custody.ApiFramework.Sessions;
using Directfn.Custody.SampleApi.Security;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddDirectfnCustodyLogging();

builder.Services.AddDirectfnCustodyApiFramework(builder.Configuration);
builder.Services.AddScoped<IEntitlementService, FakeEntitlementService>();
builder.Services.AddScoped<IAuthSessionService, FakeAuthSessionService>();

WebApplication app = builder.Build();

app.UseDirectfnCustodyApiFramework();

app.MapControllers();

app.Run();