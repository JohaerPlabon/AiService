using AiService.AccountManager.Repository.DTO;
using AiService.AccountManager.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AiService.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginRequest());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(LoginRequest request, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(request);

            var result = await _auth.LoginAsync(request);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Login failed");
                return View(request);
            }

            await SignInAsync(result.UserId!.Value, result.UserName!, isGuest: false, request.RememberMe);
            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp() => View(new RegisterRequest());

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(RegisterRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            var result = await _auth.RegisterAsync(request);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed");
                return View(request);
            }

            await SignInAsync(result.UserId!.Value, result.UserName!, isGuest: false, persistent: true);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Guest()
        {
            var result = await _auth.LoginAsGuestAsync();
            await SignInAsync(result.UserId!.Value, result.UserName!, isGuest: true, persistent: false);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        private async Task SignInAsync(Guid userId, string userName, bool isGuest, bool persistent)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, isGuest ? "Guest" : "User"),
            new Claim("is_guest", isGuest ? "true" : "false")
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = persistent });
        }

        private IActionResult RedirectToLocal(string? returnUrl)
            => Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl!) : RedirectToAction("Index", "Home");

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var result = await _auth.VerifyGmailAccount(token);
            return View("Info", result ? "Email verified!" : "Invalid or expired token.");
        }
    }
}
