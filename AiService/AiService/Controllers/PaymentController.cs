using AiService.DataManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiService.Domains.Entities;
using System.Security.Claims;

[Authorize]
public class PaymentInfoController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _contextAccessor;

    public PaymentInfoController(ApplicationDbContext db, IHttpContextAccessor accessor)
    {
        _db = db;
        _contextAccessor = accessor;
    }

    [HttpPost]
    public IActionResult BkashPaymentInfo(string phoneNumber, string trxId, decimal amount)
    {
        string userEmail = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
        var PaymentInfo = new PaymentInfo
        {
            Email = userEmail == null ? string.Empty : userEmail,
            Method = "Bkash",
            Amount = amount,
            TransactionId = trxId,
            Status = "Pending"
        };

        _db.PaymentInfos.Add(PaymentInfo);
        _db.SaveChanges();

        // Here: Call bKash API to verify PaymentInfo (sandbox/live)
        // If success, update PaymentInfo.Status = "Success"

        TempData["Message"] = "Your bKash PaymentInfo is being verified!";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public IActionResult RocketPaymentInfo(string phoneNumber, string trxId, decimal amount)
    {
        string userEmail = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
        var PaymentInfo = new PaymentInfo
        {
            Email = userEmail == null ? string.Empty : userEmail,
            Method = "Bkash",
            Amount = amount,
            TransactionId = trxId,
            Status = "Pending"
        };

        _db.PaymentInfos.Add(PaymentInfo);
        _db.SaveChanges();

        TempData["Message"] = "Rocket PaymentInfo received. Verification pending.";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public IActionResult NagadPaymentInfo(string phoneNumber, string trxId, decimal amount)
    {
        string userEmail = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
        var PaymentInfo = new PaymentInfo
        {
            Email = userEmail == null ? string.Empty : userEmail,
            Method = "Bkash",
            Amount = amount,
            TransactionId = trxId,
            Status = "Pending"
        };

        _db.PaymentInfos.Add(PaymentInfo);
        _db.SaveChanges();

        TempData["Message"] = "Nagad PaymentInfo received. Verification pending.";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public IActionResult VisaPaymentInfo(string cardNumber, string expiry, string cvv, decimal amount)
    {
        string userEmail = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
        var PaymentInfo = new PaymentInfo
        {
            Email = userEmail == null ? string.Empty : userEmail,
            Method = "Bkash",
            Amount = amount,
            TransactionId = "transactionId",
            Status = "Pending"
        };

        _db.PaymentInfos.Add(PaymentInfo);
        _db.SaveChanges();

        // Here integrate with Stripe / SSLCOMMERZ / other gateway
        TempData["Message"] = "Visa PaymentInfo submitted successfully!";
        return RedirectToAction("Index", "Dashboard");
    }
}