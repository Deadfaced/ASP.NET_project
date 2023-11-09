using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MvcBlog.Areas.Identity.Data;
using MvcBlog.Data;
using MvcBlog.Models;
using MvcBlog.Models.ViewModels;
namespace MvcBlog.Controllers;

public class MostLikedViewComponent : ViewComponent
{
    private readonly BlogDbContext _context;

    public MostLikedViewComponent(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var posts = await _context.Posts
            .OrderByDescending(p => p.Likes)
            .Take(3)
            .ToListAsync();

        return View(posts);
    }
}