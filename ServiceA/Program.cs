using Config.Data.Ef;

var builder = WebApplication.CreateBuilder(args);

var appName = builder.Configuration["Config:ApplicationName"] ?? "SERVICE-A";
var cs = builder.Configuration["Config:ConnectionString"]!;
var refresh = int.TryParse(builder.Configuration["Config:RefreshIntervalMs"], out var ms) ? ms : 5000;

var store = new EfConfigStore(cs);
var reader = new ConfigurationReader.ConfigurationReader(appName, refresh, store);

var app = builder.Build();
app.Lifetime.ApplicationStopping.Register(() => reader.Dispose());

app.MapGet("/", () => $"ServiceA up. ApplicationName={appName}");

app.MapGet("/config/{key}", (string key) =>
{
    try
    {
        var value = reader.GetValue(key);
        return Results.Ok(new { key, value });
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { key, error = ex.Message });
    }
});

app.Run();
