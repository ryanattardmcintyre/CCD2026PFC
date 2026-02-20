using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login";   // where you start auth
        options.LogoutPath = "/account/logout";
    })
    .AddGoogle(options =>
    {
        options.ClientId = "";
        options.ClientSecret = "";

        // Make sure we end up with a Name claim so User.Identity.Name is populated
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

        // Optional: if "name" isn’t present in some cases, fallback to email
        options.Events.OnCreatingTicket = ctx =>
        {
            var hasName = ctx.Identity?.Claims.Any(c => c.Type == ClaimTypes.Name && !string.IsNullOrWhiteSpace(c.Value)) == true;
            if (!hasName)
            {
                var email = ctx.Identity?.FindFirst(ClaimTypes.Email)?.Value;
                if (!string.IsNullOrWhiteSpace(email))
                    ctx.Identity!.AddClaim(new Claim(ClaimTypes.Name, email));
            }
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

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
