using Microsoft.AspNetCore.Mvc;
using MLS_UI.Interfaces;
using MLS_UI.Models;

namespace MLS_UI.Controllers
{
    public class AuthentController : Controller
    {
        private readonly IAuthentInterface _authentService;

        public AuthentController(IAuthentInterface authentService)
        {
            _authentService = authentService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authentService.RegisterAsync(model);
            TempData["msg"] = result.StatusMessage;

            return RedirectToAction(nameof(Register));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authentService.LoginAsync(model);

            //var jwtToken = result.Token;

            //if (result.StatusCode == 1 && jwtToken != string.Empty)
            if (result.StatusCode == 1 && !string.IsNullOrEmpty(result.Token))
            {
                //return RedirectToAction("Index", "Home", new { token = jwtToken });
                //return RedirectToAction("Index", "Home", new { token = result.Token });
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(15), // Le token expire après 15 minutes.
                };

                Response.Cookies.Append("jwtToken", result.Token, cookieOptions);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["msg"] = result.StatusMessage;

                return RedirectToAction(nameof(Login));
            }
        }

        //[Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authentService.LogoutAsync();

            return RedirectToAction(nameof(Login));
        }
    }
}
