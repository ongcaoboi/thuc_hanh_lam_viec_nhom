using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using web_bh.Models;

namespace web_bh.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly web_bhContext _context;

    public ProductController(ILogger<ProductController> logger, web_bhContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(int? id, int p = 1)
    {
        int size = 5;
        ViewData["IsCate"] = 0;
        if(id == null && id != 0)
        {   
            ViewData["Title"] = "Sản phẩm";
            var list = from a in _context.Products
                    join b in _context.Categories on a.IdCategory equals b.Id
                    select new {
                        id = a.Id,
                        idCate = b.Id,
                        nameCate = b.Name,
                        title = a.Title,
                        price = a.Price,
                        thumb = a.Thumbnail,
                        description = a.Description,
                        quantity = a.Quantity 
                    };
            ViewData["sl"] = (int)list.Count();
            int pages = (int)Math.Ceiling((double)list.Count() / size);
            ViewData["num_page"] = pages;
            ViewData["page"] = p;
            ViewData["ls_product"] = list.Skip((p - 1) * size).Take(size);
        }
        else
        {
            var rs = (from a in _context.Categories where a.Id == id select a).FirstOrDefault();
            if(rs == null)
                return NotFound();
            ViewData["Title"] = rs?.Name;
            ViewData["IsCate"] = rs?.Id;
            var list = from a in _context.Products
                    join b in _context.Categories on a.IdCategory equals b.Id
                    where b.Id == id
                    select new {
                        id = a.Id,
                        idCate = b.Id,
                        nameCate = b.Name,
                        title = a.Title,
                        price = a.Price,
                        thumb = a.Thumbnail,
                        description = a.Description,
                        quantity = a.Quantity 
                    };
            ViewData["sl"] = (int)list.Count();
            int pages = (int)Math.Ceiling((double)list.Count() / size);
            ViewData["num_page"] = pages;
            ViewData["page"] = p;
            ViewData["ls_product"] = list.Skip((p - 1) * size).Take(size);
        }
        return View();
    }

    public IActionResult Details(int? id)
    {
        if(id == null )
        {
            return NotFound();
        }
        else
        {
            var product = (from a in _context.Products where a.Id == id
                                    select a).FirstOrDefault();
            if(product == null)
                return NotFound();
            var rs = from a in _context.Categories where a.Id == product.IdCategory select a;
            if(!rs.Any())
                return NotFound();
            Category? category = rs.FirstOrDefault();
            ViewData["cate"] = category;
            return View(product);
        }
    }

    public IActionResult Search(string? q, int p = 1)
    {
        int size = 5;
        if(q == null)
        {
            ViewData["Title"] = "Tất cả";
            ViewData["IsCate"] = 0;
            var list = from a in _context.Products
                        join b in _context.Categories on a.IdCategory equals b.Id
                        select new {
                            id = a.Id,
                            idCate = b.Id,
                            nameCate = b.Name,
                            title = a.Title,
                            price = a.Price,
                            thumb = a.Thumbnail,
                            description = a.Description,
                            quantity = a.Quantity 
                        };
            ViewData["sl"] = (int)list.Count();
            int pages = (int)Math.Ceiling((double)list.Count() / size);
            ViewData["num_page"] = pages;
            ViewData["page"] = p;
            ViewData["ls_product"] = list.Skip((p - 1) * size).Take(size);
        }
        else
        {
            ViewData["Title"] = q;
            ViewData["IsCate"] = 1;
            ViewData["IsSearch"] = 1;
            var list = from a in _context.Products
                        join b in _context.Categories on a.IdCategory equals b.Id
                        where a.Title.ToLower().Contains(q.ToLower())
                        select new {
                            id = a.Id,
                            idCate = b.Id,
                            nameCate = b.Name,
                            title = a.Title,
                            price = a.Price,
                            thumb = a.Thumbnail,
                            description = a.Description,
                            quantity = a.Quantity 
                        };
            ViewData["sl"] = (int)list.Count();
                int pages = (int)Math.Ceiling((double)list.Count() / size);
                ViewData["num_page"] = pages;
                ViewData["page"] = p;
                ViewData["ls_product"] = list.Skip((p - 1) * size).Take(size);
        }
        return View("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
