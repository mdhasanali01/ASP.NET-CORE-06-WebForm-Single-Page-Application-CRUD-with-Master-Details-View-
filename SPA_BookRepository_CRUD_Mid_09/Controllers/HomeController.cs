using Microsoft.AspNetCore.Mvc;

namespace SPA_BookRepository_CRUD_Mid_09.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
