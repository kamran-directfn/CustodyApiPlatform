using Directfn.Custody.ApiFramework.Extensions;
using Directfn.Custody.SampleApi.Security;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Sessions;
var builder = WebApplication.CreateBuilder(args);

builder.AddDirectfnCustodyLogging();

builder.Services.AddDirectfnCustodyApiFramework(builder.Configuration);
builder.Services.AddScoped<IEntitlementService, FakeEntitlementService>();
builder.Services.AddScoped<IAuthSessionService, FakeAuthSessionService>();

var app = builder.Build();

app.UseDirectfnCustodyApiFramework();

app.MapControllers();

app.Run();