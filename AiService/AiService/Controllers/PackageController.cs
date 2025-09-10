using Microsoft.AspNetCore.Mvc;
using AiService.Domains.Entities;

namespace AiService.Controllers
{
    public class PackageController : Controller
    {
        public IActionResult Index()
        {
            var offers = new List<PackagePlans>
            {
                new () { Title = "50 GB + Hoichoi", Data = "50 GB + Hoichoi", Price = 618, ValidityDays = 30, Tag = "10 GB Bonus", ShowTimer = true, RemainingTime = new TimeSpan(12,19,35) },
                new () { Title = "120 GB", Data = "120 GB", Price = 798, ValidityDays = 30, ShowTimer = true, RemainingTime = new TimeSpan(12,19,35) },
                new () { Title = "8 GB + Hoichoi", Data = "8 GB + Hoichoi", Price = 118, ValidityDays = 3, Tag = "Send Gift", ShowTimer = true, RemainingTime = new TimeSpan(12,19,35) },
                new () { Title = "3 GB", Data = "3 GB", Price = 98, ValidityDays = 3, Tag = "Free Hoichoi" },
                new () { Title = "Unlimited Internet", Data = "Unlimited Internet", Price = 247, ValidityDays = 7, Tag = "New!" }
            };

            return View(offers);
        }

        public IActionResult BuyCredits()
        {
            return View("BuyCredits");
        }
    }
}
