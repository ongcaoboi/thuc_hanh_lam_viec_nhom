using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using web_bh.Models;
using Microsoft.EntityFrameworkCore;

namespace web_bh.Controllers;

public class MyOrderController : Controller
{
    private readonly ILogger<MyOrderController> _logger;

    private readonly web_bhContext _context;
    public MyOrderController(ILogger<MyOrderController> logger, web_bhContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return RedirectToAction("Index", "Home");
        }else
        {
            List<Order> orders = _context.Orders.Where(q => q.IdUser == idUser).Include(x => x.OrderDetails.OrderBy(p => p.Id)).OrderByDescending(c => c.Id).ToList();
            ViewData["status"] = _context.Statuses.ToList();
            ViewData["product"] = _context.Products.ToList(); 
            return View(orders);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
