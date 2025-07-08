using Hj.Examples.Website.Stubs.Api;
using Hj.ServiceStub;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Configures the Service Stub.
builder.Services.ConfigureStub(builder.Configuration, options =>
{
  options.JsonPath = Path.Combine(builder.Environment.ContentRootPath, "Stubs", "Json");
});

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseRouting();

// Add the middleware that serves stubs.
app.UseStub();

// Add an API that manages the "active" stubs collection. See the Stub.http file.
app.UseStubApi("stub");

// Map stubs in a Mimimal API-like fashion.
app.MapStub("/hello-from-api", HttpMethod.Get, HelloFromApi.GetJsonAsync);
app.MapStub("/hello-from-api", HttpMethod.Get, "my-other-collection", HelloFromApi.GetJsonFromMyOtherCollectionAsync);
app.MapStub("/hello-from-svg", HttpMethod.Get, HelloFromApi.GetSvgAsync);

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");

await app.RunAsync();
