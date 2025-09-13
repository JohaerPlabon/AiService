using Microsoft.AspNetCore.Mvc;

namespace AiService.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult TextGenerator()
        {
            return View();
        }
    }
}
