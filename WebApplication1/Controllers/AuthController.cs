using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    [HttpGet("/auth/login")]
    public IActionResult Login(string? returnUrl = "/")
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(Callback), new { returnUrl })!
        };
        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("/auth/callback")]
    public IActionResult Callback(string? returnUrl = "/")
        => LocalRedirect(returnUrl ?? "/");

    [Authorize]
    [HttpPost("/auth/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}
