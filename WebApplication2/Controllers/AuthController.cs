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
        var props = new AuthenticationProperties { RedirectUri = returnUrl ?? "/" };
        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    [Authorize]
    [HttpPost("/auth/logout")]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }
}
