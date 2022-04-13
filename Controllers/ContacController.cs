using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using web_bh.Models;
using Newtonsoft.Json;

namespace web_bh.Controllers
{
    public class ContacController : Controller
    {
        private IWebHostEnvironment _hostEnvironment;
        private CartContext _cartContext;

        public ContacController (IWebHostEnvironment hostEnvironment, CartContext cartContext)
        {
            _hostEnvironment = hostEnvironment;
            _cartContext = cartContext;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}