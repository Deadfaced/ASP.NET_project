using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MvcBlog.Models;
using MvcBlog.Models.ViewModels;

namespace MvcBlog.Controllers;

public class AboutController : Controller{

    
    public IActionResult Index(){
        return View();
    }
}