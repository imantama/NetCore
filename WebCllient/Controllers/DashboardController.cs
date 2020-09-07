using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebCllient.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.IsAvailable)
            {
                return View();
            }
            //if (HttpContext.Session.GetString("lvl") == "Sales")
            //{
            //    return View();
            //}
            return RedirectToAction("Login", "Auth");
        }
    }
}