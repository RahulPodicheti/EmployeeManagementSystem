using EmployeeManagementSystem.Services;
using EmployeeManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EmployeeManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAuthService authService,
            ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _authService.LoginAsync(
                model.Username,
                model.Password);

            if (user == null)
            {
                ViewBag.Error =
                    "User does not exist or password is incorrect";

                _logger.LogWarning(
                    "Failed Login Attempt: {Username}",
                    model.Username);

                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal =
                new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            _logger.LogInformation(
                "User Logged In: {Username}",
                user.Username);

            TempData["Success"] =
                "Login successful.";

            return RedirectToAction(
                "Index",
                "Dashboard");
        }
        [HttpPost]
        public async Task<IActionResult> Register(
    RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _authService.UserExistsAsync(
                model.Username))
            {
                ModelState.AddModelError(
                    "",
                    "Username already exists.");

                return View(model);
            }

            await _authService.RegisterUserAsync(
                model.Username,
                model.Email,
                model.Password,
                "User");

            TempData["Success"] =
                "Registration successful. Please login.";

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation(
                "User Logged Out: {Username}",
                User.Identity?.Name);

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Success"] =
                "Logout successful.";

            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            _logger.LogWarning(
                "Access Denied: {Username}",
                User.Identity?.Name);

            return View();
        }
    }
}