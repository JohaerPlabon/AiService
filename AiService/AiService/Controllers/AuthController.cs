using AiService.AccountManager.Repository.DTO;
using AiService.AccountManager.Repository.Interfaces;
using AiService.Domains.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AiService.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IUserRepository _user;
        private static RegisterRequest _registerRequest;

        public AuthController(IAuthService auth, IUserRepository user)
        {
            _auth = auth;
            _user = user;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn(string? returnUrl = null)
        {
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
            return RedirectToLocal("Home/Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp() => View(new AccountManager.Repository.DTO.RegisterRequest());

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(AccountManager.Repository.DTO.RegisterRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            _registerRequest = request;

            await _auth.SendVerificationEmailAsync(request);
            ViewBag.Email = request.Email;

            return View("VerifyEmailNotice", new ApplicationUser());
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
            return RedirectToAction("SignIn");
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

        private async Task<AuthResult> RegisterVerifiedAccount()
        {
            var result = await _auth.RegisterAsync();

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed");
                return result;
            }

            //await SignInAsync(result.UserId!.Value, result.UserName!, isGuest: false, persistent: true);
            return AuthResult.Success(result.UserId!.Value, result.UserName!, false);
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var response = await RegisterVerifiedAccount();
            bool result = false;
            if (response.Succeeded)
            {
                result = await _auth.UpdateGmailVerificationStatus(token);
            }

            await Task.Delay(100);
            var info = await _user.GetByEmailAsync(_registerRequest.Email);
            return View("VerifyEmailNotice", info);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginWithGoogle(string returnUrl = "/")
        {
            var props = new AuthenticationProperties { RedirectUri = Url.Action("GoogleCallback", new { returnUrl }) };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(string returnUrl = "/")
        {
            // Get the Google principal from the cookie (the Google handler signs into Cookie scheme)
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = authResult?.Principal ?? User;

            // In some setups the cookie might not carry Google claims yet; fall back:
            if (principal?.Identity is null || !principal.Identity.IsAuthenticated)
            {
                var googleTicket = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
                principal = googleTicket?.Principal;
            }

            var email = principal?.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return View("Info", "Unable to retrieve your Google account email.");

            // Upsert user and mark verified
            var user = await _auth.UpdateGoogleUserStatus(email, name);
            await SignInAppCookieAsync(user);

            return LocalRedirect(returnUrl);
        }

        private async Task SignInAppCookieAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.UserName) ? user.Email : user.UserName),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
