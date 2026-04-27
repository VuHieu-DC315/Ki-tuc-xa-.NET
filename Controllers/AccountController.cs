using System.Security.Claims;
using kitucxa.Models;
using kitucxa.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace kitucxa.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILoginAttemptService _loginAttemptService;

        public AccountController(IAuthService authService, ILoginAttemptService loginAttemptService)
        {
            _authService = authService;
            _loginAttemptService = loginAttemptService;
        }

        public IActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                bool isBlocked = await _loginAttemptService.IsBlockedAsync(username, ipAddress);

                if (isBlocked)
                {
                    ModelState.AddModelError("", "Bạn nhập sai quá nhiều lần. Vui lòng thử lại sau 5 phút.");
                    return View();
                }

                var user = _authService.Login(username, password);

                if (user == null)
                {
                    int failedCount = await _loginAttemptService.RecordFailedAttemptAsync(username, ipAddress);

                    if (failedCount >= 5)
                    {
                        ModelState.AddModelError("", "Bạn đã nhập sai quá nhiều lần. Tài khoản bị khóa tạm thời 5 phút.");
                        return View();
                    }
                    ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu.");
                    return View();
                }

                await _loginAttemptService.ResetFailedAttemptsAsync(username, ipAddress);

                if (user.Role == "User" && user.StudentId == null)
                {
                    ModelState.AddModelError("", "Tài khoản này chưa được liên kết với sinh viên.");
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

                if (user.Role == "User")
                {
                    return RedirectToAction("Dashboard", "Student");
                }

                return RedirectToAction("AccessDenied", "Account");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng nhập: " + ex.Message);
                return View();
            }
        }

        public IActionResult Register()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public IActionResult Register(RegisterVm user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _authService.Register(user);
                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công. Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }

                return View(user);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký tài khoản: " + ex.Message);
                return View(user);
            }
        }

        public IActionResult AccessDenied()
        {
            try
            {
                return View();
            }
            catch
            {
                return Content("Bạn không có quyền truy cập trang này.");
            }
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Login");
            }
        }
    }
}