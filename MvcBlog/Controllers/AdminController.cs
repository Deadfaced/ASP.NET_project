using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MvcBlog.Areas.Identity.Data;
using MvcBlog.Models;
using MvcBlog.Models.ViewModels;

namespace MvcBlog.Controllers;

public class AdminController : Controller
{
    private readonly UserManager<MvcBlogUser> _userManager;

    public AdminController(UserManager<MvcBlogUser> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin")]
public async Task<IActionResult> Manage()
{
    var allUsers = _userManager.Users.ToList();
    var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

    var nonAdminUsers = allUsers.Except(adminUsers).ToList();

    return View(nonAdminUsers);
}

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("Manage");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(user);
        }
    }
}
