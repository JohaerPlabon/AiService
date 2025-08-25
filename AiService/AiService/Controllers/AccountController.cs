using AiService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AiService.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(User model)
        {
            if (ModelState.IsValid)
            {
                if (model.Email == "test@example.com" && model.Password == "123456")
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult SignUp(User model)
        {
            if (ModelState.IsValid)
            {
                if (model.Email == "test@example.com" && model.Password == "123456")
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password");
            }

            return View(model);
        }
    }
}
