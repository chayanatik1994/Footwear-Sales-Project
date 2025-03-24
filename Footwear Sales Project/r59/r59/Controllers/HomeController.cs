using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using r59.Models;

namespace r59.Controllers
{
    public class HomeController(ProductDbContext db) : Controller
    {
        
        public IActionResult Index()
        {
            if (!db.Database.CanConnect())
            { 
            db.Database.EnsureCreated();
            }
            return View();
        }
    }
}
