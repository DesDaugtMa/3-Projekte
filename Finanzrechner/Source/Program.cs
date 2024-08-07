using Finanzrechner.Config;
using Finanzrechner.Database;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

IHostBuilder hostBuilder = builder.Host.ConfigureAppConfiguration(configurationBuilder =>
{
    configurationBuilder.Sources.Clear();
    configurationBuilder.AddJsonFile($"Config/appsettings.json", false);
    configurationBuilder.AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", true);
    configurationBuilder.AddEnvironmentVariables();
    configurationBuilder.AddUserSecrets<Program>(true);
});

AppSettings appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var currentThreadCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
    currentThreadCulture.NumberFormat = NumberFormatInfo.InvariantInfo;

    Thread.CurrentThread.CurrentCulture = currentThreadCulture;
    Thread.CurrentThread.CurrentUICulture = currentThreadCulture;

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transaction}/{action=Index}/{id?}");

app.Run();
