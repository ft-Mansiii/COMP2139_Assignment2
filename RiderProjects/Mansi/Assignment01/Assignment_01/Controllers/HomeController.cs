using System.Diagnostics;
using Assignment01.Areas.Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Assignment01.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {
        var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Anonymous";
        var endpoint = _httpContextAccessor.HttpContext?.Request?.Path;
        _logger.LogInformation("User {User} accessed endpoint {Endpoint} at {Time}",
            user, endpoint, DateTime.UtcNow);
        return View();
    }

    public IActionResult Privacy()
    {
        var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Anonymous";
        var endpoint = _httpContextAccessor.HttpContext?.Request?.Path;
        Log.Information("User {User} accessed endpoint {Endpoint} at {Time}",
            user, endpoint, DateTime.UtcNow);
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    [Route("Home/Error")]
    public IActionResult Error(int? statusCode = null)
    {
        if (statusCode == 404)
            ViewBag.ErrorMessage = "Sorry, the page you are looking for could not be found.";
        else if (statusCode == 500)
            ViewBag.ErrorMessage = "Sorry, something went wrong on our end. Please try again later.";
        else
            ViewBag.ErrorMessage = "An unexpected error occurred.";

        return View();
    }
}