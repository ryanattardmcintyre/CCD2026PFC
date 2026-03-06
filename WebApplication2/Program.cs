using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using WebApplication2.Repositories;

var builder = WebApplication.CreateBuilder(args);

Environment.SetEnvironmentVariable
    ("GOOGLE_APPLICATION_CREDENTIALS", 
    "C:\\Users\\attar\\Source\\Repos\\CCD2026PFCv2\\WebApplication2\\ccd63a2026-7af4d41f03a9.json");


// MVC (views + controllers)
builder.Services.AddControllersWithViews();

// Auth: Cookies + Google
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        // Where unauthenticated users get redirected (your controller should Challenge Google)
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";

        // Optional: cookie settings
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
    })
    .AddGoogle(options =>
    {
        // Prefer config, but leaving hardcoded since you currently have it that way
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        // Optional: ensure email is included in claims
        options.Scope.Add("email");
        options.Scope.Add("profile");
    });

//add scoped firestorerepository passing the project id: ccd63a2026, reading the project id from configuration
builder.Services.AddScoped<FirestoreRepository>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var projectId = configuration["Firestore:ProjectId"];
    return new FirestoreRepository(projectId);
});

builder.Services.AddScoped<BucketsRepository>();


var app = builder.Build();

// Pipeline
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

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
