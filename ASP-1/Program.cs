using ASP_1.Data;
using ASP_1.Middleware;
using ASP_1.Services;
using ASP_1.Services.Email;
using ASP_1.Services.Hash;
using ASP_1.Services.Kdf;
using ASP_1.Services.Random;
using ASP_1.Services.Transliterate;
using ASP_1.Services.Validation;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<DateService>();
builder.Services.AddScoped<TimeService>();
builder.Services.AddSingleton<StampService>();

builder.Services.AddSingleton<IHashService, Md5HashService>();

builder.Services.AddSingleton<IRandomService, RandomServiceV1>();
builder.Services.AddSingleton<IKdfService, HashKdfService>();

builder.Services.AddSingleton<IValidationService, ValidationServiceV1>();
builder.Services.AddSingleton<IEmailService, GmailService>();
builder.Services.AddSingleton<ITransliterationService, TransliterationServiceUkr>();


String? connectionString = builder.Configuration.GetConnectionString("MySqlDb");
MySqlConnection connection = new MySqlConnection(connectionString);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connection, ServerVersion.AutoDetect(connection))
    );



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


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

app.UseSession();

//app.UseMiddleware<SessionAuthMiddleware>();
app.UseSessionAuth();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
