﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebCllient.Controllers
{
    public class DepartmensController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}