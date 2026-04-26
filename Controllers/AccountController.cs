using System.Security.Claims;
using kitucxa.Data;
using kitucxa.Models;
using kitucxa.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace kitucxa.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _authService.Login(username, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };
            if (user.StudentId != null)
            {
                claims.Add(new Claim("StudentId", user.StudentId.Value.ToString()));
            }
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            else if (user.Role == "User")
            {
                if (user.StudentId == null)
                {
                    ModelState.AddModelError("", "Tài khoản này chưa được liên kết với sinh viên.");
                    return View();
                }
                return RedirectToAction("Dashboard", "Student", new { id = user.StudentId });
            }
            return RedirectToAction("AccessDenied", "Account");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVm user)
        {
            if (ModelState.IsValid)
            {
                _authService.Register(user);
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công. Vui lòng đăng nhập.";
                return View();
            }
            return View(user);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}