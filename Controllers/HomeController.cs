using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using web_bh.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace web_bh.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    // private readonly <HomeController> _context;
    private readonly web_bhContext _context;
    public HomeController(ILogger<HomeController> logger, web_bhContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        ViewData["ls_prd_new"] = (from a in _context.Products
                    join b in _context.Categories on a.IdCategory equals b.Id
                    orderby a.Id descending
                    select new {
                        id = a.Id,
                        idCate = b.Id,
                        nameCate = b.Name,
                        title = a.Title,
                        price = a.Price,
                        thumb = a.Thumbnail,
                        description = a.Description,
                        quantity = a.Quantity 
                    }).Take(5);
        ViewData["ls_cate"] = _context.Categories.Include(x => x.Products.OrderByDescending(x => x.Id).Take(5));
        return View();
    }
    [HttpPost]
    public async Task<JsonResult> Login(string login_name, string login_pass, bool login_checkSave)
    {
        var account = await _context.Users.Where(p => p.UserName.Contains(login_name)).FirstOrDefaultAsync();
        if(account == null)
        {
            return Json(new {
                message = "Tài khoản không tồn tại!",
                position = "0"
            });
        }
        if(account.Password != login_pass)
        {
            return Json(new {
                message = "Mật khẩu không chính xác!",
                position = "0"
            });
        }
        HttpContext.Session.SetInt32("user", account.Id);
        HttpContext.Session.SetInt32("role", account.IdRole);

        // string user = JsonConvert.SerializeObject(new {
        //     id = a.Id,
        //     idRole = a.IdRole,
        //     userName = a.UserName,
        //     disName = a.DisName,
        //     pass = a.Password
        // });
        // HttpContext.Session.SetString("user", user);
        var rs = new { 
            message = "Đăng nhập thành công",
            position = "1"    
        };
        return Json(rs);
    }
    [HttpPost]
    public async Task<JsonResult> Repass(string old_pass, string new_pass)
    {
        if(HttpContext.Session.GetInt32("user") != null)
        {
            int idUser = (int)HttpContext.Session.GetInt32("user");
            User user = await _context.Users.Where(p => p.Id.Equals(idUser)).FirstOrDefaultAsync();
            if(user == null)
            {
                return Json(new {
                    message = "Không tìm thấy tài khoản!",
                    position = "0"
                });
            }
            if(user.Password != old_pass)
            {
                return Json(new {
                    message = "Mật khẩu không chĩnh xác!",
                    position = "0"
                });
            }
            user.Password = new_pass;
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                var rs = new { 
                    message = "Đổi mật khẩu thành công",
                    position = "1"
                };
                return Json(rs);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new {
                    message = "Lỗi update mk!",
                    position = "0"
                });
            }
        }else
        {
            return Json(new {
                message = "Có lỗi xảy ra!",
                position = "0"
            });
        }
    }
    [HttpPost]
    public async Task<JsonResult> Register(string name, string pass, string fullname, string sdt, string email)
    {
        if(HttpContext.Session.GetInt32("user") != null)
        {
            return Json(new {
                message = "Có lỗi xảy ra!",
                position = "0"
            });
        }else
        {
            var a = await _context.Users.Where(p => p.Email.Contains(email)).FirstOrDefaultAsync();
            if(a != null)
            {
                return Json(new {
                    message = "Email đã được đăng ký!",
                    position = "0"
                });
            }else
            {
                var b = await _context.Users.Where(p => p.PhoneNumber.Contains(sdt)).FirstOrDefaultAsync();
                if(b != null)
                {
                    return Json(new {
                        message = "Số điện thoại đã được đăng ký!",
                        position = "0"
                    });
                }else
                {
                    User user = new User();
                    user.IdRole = 3;
                    user.UserName = name;
                    user.Password = pass;
                    user.DisName = fullname;
                    user.PhoneNumber = sdt;
                    user.Email = email;

                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return Json(new {
                        message = "Đăng ký thành công, hãy đăng nhập để sử dụng!",
                        position = "1"
                    });
                }
            }
        }
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("user");
        HttpContext.Session.Remove("role");
        return RedirectToAction("Index", "Home");
    }
    // public IActionResult Index1()
    // {
    //     var item = _context.Categories.Include(a => a.Children.Select(c => c.ChildRelationshipType));
    //     return View();
    // }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
