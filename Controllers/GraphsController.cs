﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CytoscapeDijkstra2.Controllers
{
    public class GraphsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
