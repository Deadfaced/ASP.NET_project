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

public class PostsController : Controller
{
    private readonly ILogger<PostsController> _logger;

    private readonly IConfiguration _configuration;

    private readonly UserManager<MvcBlogUser> _userManager;

    private readonly BlogDbContext _context;

    string[] formats =
    {
        "dd/MM/yyyy HH:mm:ss",
        "MM/dd/yyyy HH:mm:ss",
        "MM/dd/yyyy H:mm:ss",
        "dd/MM/yyyy HH:mm",
        "yyyy/MM/dd HH:mm:ss",
        "MM/dd/yyyy h:mm:ss tt",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy/MM/dd HH:mm",
        "dd-MM-yyyy HH:mm:ss",
        "dd-MM-yyyy H:mm:ss",
        "dd-MM-yyyy HH:mm",
        "yyyy-MM-dd HH:mm:ss",
        "MM-dd-yyyy HH:mm:ss",
        "MM-dd-yyyy H:mm:ss",
        "MM-dd-yyyy HH:mm",
        "yy/MM/dd HH:mm:ss",
        "yy/MM/dd H:mm:ss",
        "yy/MM/dd HH:mm"
    };

    public PostsController(
        ILogger<PostsController> logger,
        IConfiguration configuration,
        UserManager<MvcBlogUser> userManager,
        BlogDbContext context
    )
    {
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _context = context;
    }

    ////////////////////////////////   Actions   ////////////////////////////////

////////////
    public IActionResult Index()
    {
        var postListViewModel = GetAllPosts();
        return View(postListViewModel);
    }

////////////
    public IActionResult NewPost()
    {
        return View();
    }

////////////
    public async Task<IActionResult> ViewPost(int id)
    {
        var post = GetPostById(id);
        if (post == null)
        {
            return NotFound();
        }

        var comments = _context.Comments.Where(c => c.PostId == id).ToList();

        var postViewModel = new PostViewModel { Post = post, Comments = comments };

        // Create a new Comment object and set the Author property to the first name of the currently logged-in user
        var newComment = new Comment();
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            newComment.Author = user.FirstName; // Assuming your ApplicationUser has a FirstName property
        }

        // Add the new Comment object to your view model
        postViewModel.NewComment = newComment;

        return View(postViewModel);
    }

////////////
    public ActionResult EditPost(int id)
    {
        var post = GetPostById(id);
        var postViewModel = new PostViewModel();
        postViewModel.Post = post;
        return View(postViewModel);
    }

////////////
    public ActionResult Insert(PostModel post)
    {
        post.CreatedAt = DateTime.Now;
        post.UpdatedAt = DateTime.Now;

        using (
            SqliteConnection connection = new SqliteConnection(
                _configuration.GetConnectionString("JapanWandererContext")
            )
        )
        {
            connection.Open();
            using (
                var command = new SqliteCommand(
                    "INSERT INTO post (title, content, createdat, updatedat) VALUES (@title, @content, @createdAt, @updatedAt)",
                    connection
                )
            )
            {
                command.Parameters.AddWithValue("@title", post.Title);
                command.Parameters.AddWithValue("@content", post.Content);
                command.Parameters.AddWithValue("@createdAt", post.CreatedAt);
                command.Parameters.AddWithValue("@updatedAt", post.UpdatedAt);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        return RedirectToAction(nameof(Index));
    }

////////////
    public ActionResult Update(PostModel post)
    {
        post.UpdatedAt = DateTime.Now;

        using (
            SqliteConnection connection = new SqliteConnection(
                _configuration.GetConnectionString("JapanWandererContext")
            )
        )
        {
            connection.Open();
            using (
                var command = new SqliteCommand(
                    "UPDATE post SET title = @title, content = @content, updatedat = @updatedAt WHERE Id = @id",
                    connection
                )
            )
            {
                command.Parameters.AddWithValue("@title", post.Title);
                command.Parameters.AddWithValue("@content", post.Content);
                command.Parameters.AddWithValue("@updatedAt", post.UpdatedAt);
                command.Parameters.AddWithValue("@id", post.Id);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        return RedirectToAction(nameof(Index));
    }

////////////
    // This method toggles a like or dislike for a post.
    public async Task<IActionResult> ToggleLike(int postId, bool isLike)
    {
        // Get the current user.
        var user = await _userManager.GetUserAsync(User);
        // If the user is not authenticated, return an Unauthorized result.
        if (user == null)
        {
            return Unauthorized();
        }

        // Find an existing like or dislike by the user for the post.
        var userLikes = await _context.UserLikes
            .Where(ul => ul.UserId == user.Id && ul.PostId == postId)
            .ToListAsync();

        UserLike? userLike = null;
        
        if (userLikes.Any())
        {
            userLike = userLikes.First();
        }

        // If a like or dislike exists...
        if (userLike != null)
        {
            // If the existing like/dislike is the same as the new one, remove it.
            // Otherwise, update it.
            if (userLike.IsLike == isLike)
            {
                _context.UserLikes.Remove(userLike);
            }
            else
            {
                userLike.IsLike = isLike;
            }
        }
        else
        {
            // If a like or dislike does not exist, create a new one.
            userLike = new UserLike
            {
                UserId = user.Id,
                PostId = postId,
                IsLike = isLike
            };
            _context.UserLikes.Add(userLike);
        }

        // Save the changes to the database.
        await _context.SaveChangesAsync();

        // Find the post that the like or dislike is for.
        var post = await _context.Posts.FindAsync(postId);
        // Update the like count for the post.
        post.Likes = await _context.UserLikes.CountAsync(ul => ul.PostId == postId && ul.IsLike);
        // Update the dislike count for the post.
        post.Dislikes = await _context.UserLikes.CountAsync(
            ul => ul.PostId == postId && !ul.IsLike
        );

        // Update the post in the database.
        _context.Update(post);
        // Save the changes to the database.
        await _context.SaveChangesAsync();

        // Redirect the user to the view post page.
        return RedirectToAction("ViewPost", new { id = postId });
    }

////////////
    private void UpdatePost(PostModel post)
    {
        using (
            SqliteConnection connection = new SqliteConnection(
                _configuration.GetConnectionString("JapanWandererContext")
            )
        )
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText =
                    $"UPDATE post SET Likes = {post.Likes}, Dislikes = {post.Dislikes} WHERE Id = {post.Id}";

                command.ExecuteNonQuery();
            }
        }
    }
////////////

    ////////////////////////////////   Actions   ////////////////////////////////

    ////////////////////////////////   Methods   ////////////////////////////////

    internal PostViewModel GetAllPosts()
    {
        List<PostModel> postList = new();

        using (
            SqliteConnection connection = new SqliteConnection(
                _configuration.GetConnectionString("JapanWandererContext")
            )
        )
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"SELECT * FROM post";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DateTime parsedCreateDate = DateTime.Parse(reader.GetString(3));
                            DateTime parsedUpdateDate = DateTime.Parse(reader.GetString(4));

                            string formattedCreateDate = parsedCreateDate.ToString(
                                "yyyy-MM-dd HH:mm:ss"
                            );
                            string formattedUpdateDate = parsedUpdateDate.ToString(
                                "yyyy-MM-dd HH:mm:ss"
                            );

                            postList.Add(
                                new PostModel
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Content = reader.GetString(2),
                                    CreatedAt = DateTime.Parse(formattedCreateDate),
                                    UpdatedAt = DateTime.Parse(formattedUpdateDate)
                                }
                            );
                        }
                    }
                    else
                    {
                        return new PostViewModel { PostList = postList };
                    }
                }
            }
        }

        return new PostViewModel { PostList = postList };
    }

    internal PostModel GetPostById(int id)
    {
        PostModel post = new();

        using (
            SqliteConnection connection = new SqliteConnection(
                _configuration.GetConnectionString("JapanWandererContext")
            )
        )
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"SELECT * FROM post WHERE Id = '{id}'";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            DateTime parsedCreateDate = DateTime.Parse(reader.GetString(3));
                            DateTime parsedUpdateDate = DateTime.Parse(reader.GetString(4));

                            string formattedCreateDate = parsedCreateDate.ToString(
                                "yyyy-MM-dd HH:mm:ss"
                            );
                            string formattedUpdateDate = parsedUpdateDate.ToString(
                                "yyyy-MM-dd HH:mm:ss"
                            );

                            post.Id = reader.GetInt32(0);
                            post.Title = reader.GetString(1);
                            post.Content = reader.GetString(2);
                            post.CreatedAt = DateTime.Parse(formattedCreateDate);
                            post.UpdatedAt = DateTime.Parse(formattedUpdateDate);
                            post.Likes = reader.GetInt32(5);
                            post.Dislikes = reader.GetInt32(6);
                        }
                    }
                    else
                    {
                        return post;
                    }
                }
            }
        }

        return post;
    }

    [HttpPost]
    public JsonResult Delete(int id)
    {
        using (
            SqliteConnection connection = new SqliteConnection(
                _configuration.GetConnectionString("JapanWandererContext")
            )
        )
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"DELETE FROM post WHERE Id = '{id}'";

                command.ExecuteNonQuery();
            }
        }

        return Json(new Object { });
    }

    ////////////////////////////////   Methods   ////////////////////////////////

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }

    public IActionResult searchByKeyword(string keyword)
    {
        IQueryable<PostModel> query = _context.Posts;

        if (!string.IsNullOrEmpty(keyword))
        {
            var lowerCaseKeyword = keyword.ToLower();
            query = query.Where(p => p.Title.ToLower().Contains(lowerCaseKeyword));
        }

        var posts = query.ToList();

        var model = new PostViewModel
        {
            PostList = posts
        };

        return View("Index", model);
    }

    public IActionResult SearchByYear(int year)
    {
        var posts = _context.Posts.Where(p => p.CreatedAt.Year == year).ToList();

        // Assuming PostViewModel has a property List<Post> PostList
        var model = new PostViewModel
        {
            PostList = posts
        };

        return View("Index", model);
    }

    public IActionResult SearchByMonth(int year, int month)
    {
        var posts = _context.Posts.Where(p => p.CreatedAt.Year == year && p.CreatedAt.Month == month).ToList();

        // Assuming PostViewModel has a property List<Post> PostList
        var model = new PostViewModel
        {
            PostList = posts
        };

        return View("Index", model);
    }

}
