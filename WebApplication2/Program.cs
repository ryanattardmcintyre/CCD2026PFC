using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using WebApplication2.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Works both locally and inside Docker
var credentialsPath = Path.Combine(AppContext.BaseDirectory, "ccd63a2026-7af4d41f03a9.json");

Environment.SetEnvironmentVariable(
    "GOOGLE_APPLICATION_CREDENTIALS",
    credentialsPath
);

SecretManagerRepository secretManagerRepository = new SecretManagerRepository();

string googleClientId = secretManagerRepository.GetSecret("ccd63a2026", "Authentication:Google:ClientId");
string googleClientSecret = secretManagerRepository.GetSecret("ccd63a2026", "Authentication:Google:ClientSecret");
string redisPassword = secretManagerRepository.GetSecret("ccd63a2026", "Redis:Password");

// MVC
builder.Services.AddControllersWithViews();

// Auth
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
    })
    .AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.Scope.Add("email");
        options.Scope.Add("profile");
    });

builder.Services.AddScoped<FirestoreRepository>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var projectId = configuration["Firestore:ProjectId"];
    return new FirestoreRepository(projectId);
});

builder.Services.AddScoped<PublisherRepository>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var projectId = configuration["Firestore:ProjectId"];
    return new PublisherRepository(projectId, "ccd63a2026");
});

builder.Services.AddScoped<BucketsRepository>();
builder.Services.AddScoped<VisionAPIRepository>();

builder.Services.AddScoped<CacheRepository>(provider =>
{
    return new CacheRepository(redisPassword);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();