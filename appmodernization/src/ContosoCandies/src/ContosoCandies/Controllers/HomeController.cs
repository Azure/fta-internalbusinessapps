using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ContosoCandies.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Contact Shareef @ reachumar@hotmail.com";

            return View();
        }



        public IActionResult Error()
        {
            return View();
        }
    }
}
