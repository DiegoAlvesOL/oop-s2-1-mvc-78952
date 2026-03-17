using Library.mvc.Data;
using Microsoft.AspNetCore.Mvc;

namespace Library.mvc.Controllers;

public class CustomersController : Controller
{
    
    private readonly LibraryDbContext _contextLibraryDbContext;
    
    
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }
}