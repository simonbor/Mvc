using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace MvcSubAreaSample.Web.Areas.Restaurant.Hours.Controllers
{
    [Area("Restaurant")]
    [SubArea("Hours")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
