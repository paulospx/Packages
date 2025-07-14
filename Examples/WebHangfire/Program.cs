using Hangfire;
using Hangfire.Storage.SQLite;

var builder = WebApplication.CreateBuilder(args);

// Add Hangfire services with SQL lite storage.
builder.Services.AddHangfire(config =>
    config.UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSQLiteStorage("Data Source=hangfire.db;"));

// Add services to the container.
builder.Services.AddRazorPages();

// Add mvc controller with views
builder.Services.AddControllersWithViews();

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
// configure Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    // AppPath = null, // Set to null to disable the "Return to site" link
    // Authorization = new[] { new Hangfire.Dashboard.BasicAuthAuthorizationFilter(new Hangfire.Dashboard.BasicAuthAuthorizationFilterOptions
    // {
    //     SslRedirect = true,
    //     RequireSsl = true,
    //     LoginCaseSensitive = true,
    //     Users = new[]
    //     {
    //         new Hangfire.Dashboard.BasicAuthAuthorizationUser
    //         {
    //             Login = "admin",
    //             PasswordClear = "password"
    //         }
    //     }
    // }) }
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Scheduler}/{action=Index}/{id?}");
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
