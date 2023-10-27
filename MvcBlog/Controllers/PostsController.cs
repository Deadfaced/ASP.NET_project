using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MvcBlog.Models;
using MvcBlog.Models.ViewModels;

namespace MvcBlog.Controllers;

public class PostsController : Controller
{
    private readonly ILogger<PostsController> _logger;

    private readonly IConfiguration _configuration;

    public PostsController(ILogger<PostsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        var postListViewModel = GetAllPosts();
        return View(postListViewModel);
    }

    public IActionResult NewPost()
    {
        return View();
    }

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
                            postList.Add(
                                new PostModel
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Content = reader.GetString(2),
                                    CreatedAt = DateTime.Parse(reader.GetString(3)),
                                    UpdatedAt = DateTime.Parse(reader.GetString(4))
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
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText =
                    $"INSERT INTO post (title, content, createdat, updatedat) VALUES ('{post.Title}', '{post.Content}', '{post.CreatedAt}', '{post.UpdatedAt}')";

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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
