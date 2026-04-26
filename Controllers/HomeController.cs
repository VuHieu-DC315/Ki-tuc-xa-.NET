using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kitucxa.Models;
using Microsoft.AspNetCore.Authorization;

namespace kitucxa.Controllers;

public class HomeController : Controller
{
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        return View();
    }
    
    [Authorize(Roles = "Admin")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
