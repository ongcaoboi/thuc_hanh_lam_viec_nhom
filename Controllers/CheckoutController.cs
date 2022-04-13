using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using web_bh.Models;

namespace web_bh.Controllers;

public class CheckoutController : Controller
{
    private readonly ILogger<CheckoutController> _logger;
    private readonly CartContext _cartContext;
    private readonly web_bhContext _context;

    public CheckoutController(ILogger<CheckoutController> logger, CartContext cartContext, web_bhContext context)
    {
        _logger = logger;
        _cartContext = cartContext;
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
            var itemCart = _cartContext.cart.carts.Where(p => p.id_user == idUser).FirstOrDefault();
            int price = 0;
            if(itemCart != null)
            {
                var listCart = itemCart.cartItems.ToList();
                if(!listCart.Any())
                {
                    return RedirectToAction("Index", "Home");
                }
                foreach (var item in listCart)
                {
                    Product _tmp = _context.Products.Where(p => p.Id == item.id_product).FirstOrDefault();
                    if(_tmp != null)
                    {
                        price += _tmp.Price * item.quantity;
                    }
                }
            }else
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["price"] = price;
            return View();
        }
    }
    public async Task<JsonResult> CreateOrder(string fullName, string phone, string address, string note)
    {
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return Json(new {
                message = "Bạn phải đăng nhập mới sử dụng được chức năng này!",
                position = "0"
            });
        }else
        {
            var itemCart = _cartContext.cart.carts.Where(p => p.id_user == idUser).FirstOrDefault();
            if(itemCart == null || itemCart.cartItems == null)
            {
                return Json(new {
                    message = "Giỏ hàng trống không thể đặt hàng!",
                    position = "0"
                });
            }else
            {
                Order order = new Order();
                order.IdStatus = 1;
                order.IdUser = idUser;
                order.FullName = fullName;
                order.PhoneNumber = phone;
                order.Address = address;
                order.Note = note;
                order.OrderDate = DateTime.Now;

                _context.Add(order);
                await _context.SaveChangesAsync();

                var order_ = _context.Orders.Where(p => p == order).FirstOrDefault();
                if(order_ == null)
                {
                    return Json(new {
                        message = "Đã có lỗi xảy ra! Vui lòng thử lại!",
                        position = "0"
                    });
                }else
                {
                    foreach (var item in itemCart.cartItems.ToList())
                    {
                        Product prd = _context.Products.Where(p => p.Id == item.id_product).FirstOrDefault();
                        if(prd == null)
                        {
                            return Json(new {
                                message = "Lỗi sản phẩm không tồn tại! Vui lòng thử lại!",
                                position = "0"
                            });
                        }
                        prd.Quantity -= item.quantity;
                        _context.Update(prd);
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.IdOrder = order_.Id;
                        orderDetail.IdProduct = item.id_product;
                        orderDetail.Num = item.quantity;
                        orderDetail.TotalMoney = prd.Price * orderDetail.Num;
                        _context.Add(orderDetail);
                        await _context.SaveChangesAsync();
                    }
                    _cartContext.Remove(itemCart);
                    _cartContext.WriteData();
                    return Json(new {
                        message = "Đặt hàng thành công!",
                        position = "1"
                    });
                }
            }
        }
    }
    public IActionResult Complate()
    {
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return RedirectToAction("Index", "Home");
        }else
        {
            return View("Complate");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
