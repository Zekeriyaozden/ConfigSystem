using Config.Abstractions;
using Config.Data.Ef;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("ConfigDb")
         ?? throw new InvalidOperationException("ConnectionStrings:ConfigDb missing");

builder.Services.AddDbContext<ConfigDbContext>(opt => opt.UseSqlServer(cs));

builder.Services.AddScoped<IConfigRepository, ConfigRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Configurations}/{action=Index}/{id?}");

app.Run();
