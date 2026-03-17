using Microsoft.AspNetCore.Mvc;

namespace Library.mvc.Controllers;

public class ProductsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }
}