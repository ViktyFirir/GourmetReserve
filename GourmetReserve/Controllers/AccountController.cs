using System.Security.Claims;
using GourmetReserve.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GourmetReserve.Controllers
{
    public class AccountController : Controller
    {
        private readonly AdminCredentialsOptions _adminCredentials;
        private readonly PasswordHasher<object> _passwordHasher = new();

        public AccountController(IOptions<AdminCredentialsOptions> adminOptions)
        {
            _adminCredentials = adminOptions.Value;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isValidUser = !string.IsNullOrEmpty(_adminCredentials.Username)
                && string.Equals(model.Username, _adminCredentials.Username, StringComparison.OrdinalIgnoreCase);

            var passwordResult = isValidUser
                ? _passwordHasher.VerifyHashedPassword(this, _adminCredentials.PasswordHash, model.Password)
                : PasswordVerificationResult.Failed;

            if (!isValidUser || passwordResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, _adminCredentials.Username),
                new(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
