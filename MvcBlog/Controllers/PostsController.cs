using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MvcBlog.Models;
using MvcBlog.Models.ViewModels;

namespace MvcBlog.Controllers;

public class PostsController : Controller
{
    private readonly ILogger<PostsController> _logger;

    private readonly IConfiguration _configuration;

    string[] formats = { "dd/MM/yyyy HH:mm:ss", "MM/dd/yyyy HH:mm:ss" , "MM/dd/yyyy H:mm:ss" };

    public PostsController(ILogger<PostsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    ////////////////////////////////   Actions   ////////////////////////////////

    public IActionResult Index()
    {
        var postListViewModel = GetAllPosts();
        return View(postListViewModel);
    }

    public IActionResult NewPost()
    {
        return View();
    }

    public IActionResult ViewPost(int id)
    {
        var post = GetPostById(id);
        var postViewModel = new PostViewModel();
        postViewModel.Post = post;
        return View(postViewModel);
    }

    public ActionResult EditPost(int id)
    {
        var post = GetPostById(id);
        var postViewModel = new PostViewModel();
        postViewModel.Post = post;
        return View(postViewModel);
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

    public ActionResult Update(PostModel post)
    {
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
                    $"UPDATE post SET title = '{post.Title}', content = '{post.Content}', updatedat = '{post.UpdatedAt}' WHERE Id = '{post.Id}'";

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
                            DateTime parsedCreateDate;
                            DateTime parsedUpdateDate;
                            DateTime.TryParseExact(reader.GetString(3), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedCreateDate);
                            DateTime.TryParseExact(reader.GetString(4), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedUpdateDate);

                            postList.Add(
                                new PostModel
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Content = reader.GetString(2),
                                    CreatedAt = parsedCreateDate,
                                    UpdatedAt = parsedUpdateDate
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
                        while (reader.Read())
                        {
                            DateTime parsedCreateDate;
                            DateTime parsedUpdateDate;
                            DateTime.TryParseExact(reader.GetString(3), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedCreateDate);
                            DateTime.TryParseExact(reader.GetString(4), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedUpdateDate);

                            post.Id = reader.GetInt32(0);
                            post.Title = reader.GetString(1);
                            post.Content = reader.GetString(2);
                            post.CreatedAt = parsedCreateDate;
                            post.UpdatedAt = parsedUpdateDate;
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

        return Json(new Object{});
    }

    ////////////////////////////////   Methods   ////////////////////////////////

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
