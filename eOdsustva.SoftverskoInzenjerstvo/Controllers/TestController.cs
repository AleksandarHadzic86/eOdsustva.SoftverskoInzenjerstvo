using eOdsustva.SoftverskoInzenjerstvo.Models;
using Microsoft.AspNetCore.Mvc;

namespace eOdsustva.SoftverskoInzenjerstvo.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            var data = new TestViewModel
            {
                Name = "Student",
                DateOfBirth = new DateTime(2000, 1, 1)
            };
            return View(data);
        }
    }
}
