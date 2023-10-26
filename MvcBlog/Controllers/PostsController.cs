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
    private List<PostModel>? postList;

    public PostsController(ILogger<PostsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult NewPost()
    {
        return View();
    }

    internal PostViewModel GetAllPosts()
    {
        List<PostModel> postsList = new();

        using (SqliteConnection connection = new SqliteConnection(_configuration.GetConnectionString("JapanWandererContext")))
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Posts";
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        postsList.Add(
                            new PostModel
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Content = reader.GetString(2),
                                Created = reader.GetDateTime(4),
                                Updated = reader.GetDateTime(5),
                            }
                        );
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
        using (SqliteConnection connection = new SqliteConnection(_configuration.GetConnectionString("JapanWandererContext")))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "INSERT INTO Posts (Title, Content, Created, Updated) VALUES ('{post.Title}', '{post.Content}', '{post.Created}', '{post.Updated}')";

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
