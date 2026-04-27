using kitucxa.Data;
using kitucxa.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackExchange.Redis;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

if (OperatingSystem.IsWindows())
{
    var startRedis = new ProcessStartInfo
    {
        FileName = "wsl.exe",
        Arguments = "-d Ubuntu -u root -- service redis-server start",
        UseShellExecute = false,
        CreateNoWindow = true
    };

    Process.Start(startRedis)?.WaitForExit();

    Thread.Sleep(1500);
}

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");

    if (string.IsNullOrEmpty(configuration))
    {
        configuration = "127.0.0.1:6379,abortConnect=false,connectRetry=5,connectTimeout=10000,syncTimeout=10000";
    }

    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReport_RoomService, Report_RoomService>();
builder.Services.AddScoped<ILoginAttemptService, LoginAttemptService>();
builder.Services.AddScoped<IRoomPermissionService, RoomPermissionService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();