using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcBlog.Areas.Identity.Data;
using MvcBlog.Models;
using MvcBlog.Models.ViewModels;


namespace MvcBlog.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<MvcBlogUser> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<MvcBlogUser> userManager)
    {
        _logger = logger;
        this._userManager = userManager;
    }

    public IActionResult Index()
    {
        ViewData["UserID"] = _userManager.GetUserId(this.User);
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
