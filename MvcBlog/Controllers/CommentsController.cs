using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MvcBlog.Data;
using MvcBlog.Models;
using MvcBlog.Models.ViewModels;

namespace MvcBlog.Controllers;

public class CommentsController : Controller
{
    private readonly CommentsDbContext _context;

    public CommentsController(CommentsDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComment(int postId, string author, string text)
    {
        if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(text))
        {
            return BadRequest();
        }

        var comment = new Comment
        {
            PostId = postId,
            Author = author,
            Text = text
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction("ViewPost", "Posts", new { id = postId });
    }
}
