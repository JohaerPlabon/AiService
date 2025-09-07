# Project layout (solution)

```
MyCompany.AuthSample/
├─ src/
│  ├─ AiService.Domain/                # Enterprise core (no EF/MVC deps) In Core, you define interfaces for repositories, but you don’t use EF Core or MVC directly.
│  ├─ AiService.Application/           # Use cases + contracts (no EF/MVC deps)
│  ├─ AiService.Infrastructure/        # EF Core, Identity, external services
│  └─ AiService.Web/                   # ASP.NET Core MVC (UI) or Web API
└─ tests/
   └─ AiService.Tests/                 # Unit tests (Application & Domain)
```

---

# 1) Domain (pure C#)

`src/AiService.Domain/Entities/ApplicationUser.cs`

```csharp
namespace AiService.Domain.Entities;

// Domain should not depend on EF/MVC; it's just your core model.
public class Profile // separate from Identity user to keep domain clean
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = "";
    public string EmailHash { get; set; }
    public string? PasswordHash { get; set; }
}
```

---

# 2) Application (use cases + contracts)

`src/AiService.Application/Common/Interfaces/IEmailSender.cs`

```csharp
namespace AiService.Application.Common.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
}
```

`src/AiService.Application/Auth/Dtos.cs`

```csharp
namespace AiService.Application.Auth;

public record RegisterCommand(string Email, string Password, string FullName);
public record LoginCommand(string Email, string Password, bool RememberMe);
public record ConfirmEmailCommand(string UserId, string Token);
public record AuthResult(bool Success, string? Error);
```

`src/AiService.Application/Auth/IAuthService.cs`

```csharp
namespace AiService.Application.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterCommand cmd, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(LoginCommand cmd, CancellationToken ct = default);
    Task<AuthResult> ConfirmEmailAsync(ConfirmEmailCommand cmd, CancellationToken ct = default);
    Task LogoutAsync(CancellationToken ct = default);
}
```

*(Application layer is framework-agnostic: only interfaces, DTOs, and business rules.)*

---

# 3) Infrastructure (EF Core + Identity + external services)

## 3.1 Identity user + DbContext

`src/AiService.Infrastructure/Identity/ApplicationUser.cs`

```csharp
using Microsoft.AspNetCore.Identity;

namespace AiService.Infrastructure.Identity;

// Extend IdentityUser to keep security features; map to Domain Profile if needed.
public class ApplicationUser : IdentityUser
{
    public Guid ProfileId { get; set; }
}
```

`src/AiService.Infrastructure/Persistence/AppDbContext.cs`

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AiService.Domain.Entities;
using AiService.Infrastructure.Identity;

namespace AiService.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Profile> Profiles => Set<Profile>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Profile>(cfg =>
        {
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        });
    }
}
```

## 3.2 Email sender (e.g., SMTP or a provider like SendGrid)

`src/AiService.Infrastructure/Email/SmtpEmailSender.cs`

```csharp
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using AiService.Application.Common.Interfaces;

namespace AiService.Infrastructure.Email;

public class SmtpSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public string From { get; set; } = "";
    public bool EnableSsl { get; set; } = true;
}

public class SmtpEmailSender : IEmailSender
{
    #Already Implemented
}
```

## 3.3 AuthService implementation (bridging Identity to Application contracts)

`src/AiService.Infrastructure/Auth/AuthService.cs`

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using AiService.Application.Auth;
using AiService.Application.Common.Interfaces;
using AiService.Domain.Entities;
using AiService.Infrastructure.Identity;
using AiService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiService.Infrastructure.Auth;

public class AuthService : IAuthService
{
    #Already implemented
}
```

## 3.4 Dependency injection registration

`src/AiService.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AiService.Application.Auth;
using AiService.Application.Common.Interfaces;
using AiService.Infrastructure.Auth;
using AiService.Infrastructure.Email;
using AiService.Infrastructure.Identity;
using AiService.Infrastructure.Persistence;

namespace AiService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SmtpSettings>(config.GetSection("Smtp"));
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("Default")));

        services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
        {
            opt.Password.RequiredLength = 8;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Lockout.MaxFailedAccessAttempts = 5;
            opt.SignIn.RequireConfirmedEmail = true;
            opt.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromHours(24));

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
```

---

# 4) Web (MVC UI)

## 4.1 Program.cs

`src/AiService.Web/Program.cs`

```csharp
using AiService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpsRedirection(o => o.HttpsPort = 443);
builder.Services.AddHsts(o => { o.MaxAge = TimeSpan.FromDays(120); o.IncludeSubDomains = true; });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Global security headers (basic hardening)
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    ctx.Response.Headers["X-XSS-Protection"] = "0";
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

## 4.2 AuthController (thin, delegates to Application)

`src/AiService.Web/Controllers/AuthController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiService.Application.Auth;

namespace AiService.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpGet, AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterCommand cmd)
    {
        if (!ModelState.IsValid) return View(cmd);

        var result = await _auth.RegisterAsync(cmd);
        if (result.Success)
        {
            TempData["Info"] = "Registration successful. Please confirm your email.";
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed.");
        return View(cmd);
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginCommand cmd, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(cmd);
        var result = await _auth.LoginAsync(cmd);
        if (result.Success)
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl!) : RedirectToAction("Index", "Home");

        ModelState.AddModelError(string.Empty, result.Error ?? "Login failed.");
        return View(cmd);
    }

    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var res = await _auth.ConfirmEmailAsync(new ConfirmEmailCommand(userId, token));
        TempData[res.Success ? "Info" : "Error"] = res.Success ? "Email confirmed." : res.Error;
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _auth.LogoutAsync();
        return RedirectToAction(nameof(Login));
    }
}
```

*(Views: create simple Razor forms for Register and Login using the DTO properties.)*

---

# 5) appsettings.json (securely set via environment variables in prod)

`src/AiService.Web/appsettings.json`

```json
{
  "ConnectionStrings": {
    "Default": "Server=.;Database=MyAppDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Smtp": {
    "Host": "smtp.yourprovider.com",
    "Port": 587,
    "User": "apikey_or_user",
    "Password": "secret_here",
    "From": "noreply@yourdomain.com",
    "EnableSsl": true
  },
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*"
}
```

*In production, override ConnectionStrings + Smtp via **environment variables**, not hardcoded.*

---

# 6) Database & migrations
```
DB already migrated
```
---

# 7) Security hardening checklist (production)

* Enforce **HTTPS** + **HSTS** (already in Program.cs).
* **Require confirmed email** before sign-in (configured).
* **Lockout** after repeated failures (configured).
* **Cookie security**: set `Cookie.SameSite = Strict`, `Cookie.SecurePolicy = Always` (Identity sets sensible defaults, you can further tune via `ConfigureApplicationCookie`).
* **Data Protection keys** persisted (for load-balanced servers):

  * Use a shared store (e.g., Azure Blob/KeyVault or a network share).
* **Secrets** via environment variables or secret manager; never commit secrets.
* **Logging & monitoring** (Serilog, App Insights).
* **Input validation & anti-forgery** (ValidateAntiForgeryToken on POST).
* **CSP** (Content-Security-Policy header) if you want stronger front-end protection.
* **Backups** for DB; **automated migrations** during deployment with care.

Example cookie config (optional):

---


---

# 9) Unit tests (example)

`tests/AiService.Tests/AuthServiceTests.cs`

```csharp
using Xunit;

public class AuthServiceTests
{
    [Fact]
    public void Placeholder() => Assert.True(true);
}
```

*(In real tests, mock `UserManager`, `SignInManager`, `IEmailSender` or use an in-memory approach.)*

---

# 10) Deployment quick guide

* Will discuss later

---
