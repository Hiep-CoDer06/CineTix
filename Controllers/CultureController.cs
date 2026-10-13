using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CineTix.Controllers;

public sealed class CultureController : Controller
{
    private static readonly string[] SupportedCultures = ["vi-VN", "en-US"];

    [HttpGet]
    public IActionResult Set(string culture, string? returnUrl = null)
    {
        if (!SupportedCultures.Contains(culture, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(365),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });

        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
    }
}
