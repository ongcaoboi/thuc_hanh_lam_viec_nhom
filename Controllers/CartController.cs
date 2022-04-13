using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_bh.Models;

namespace web_bh.Controllers;

public class CartController : Controller
{
    private readonly ILogger<CartController> _logger;
    private readonly CartContext _cartContext;
    private readonly web_bhContext _context;

    public CartController(ILogger<CartController> logger, CartContext cartContext, web_bhContext context)
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

            List<Product> list = new List<Product>();
            var itemCart = _cartContext.cart.carts.Where(p => p.id_user == idUser).FirstOrDefault();
            if(itemCart != null)
            {
                var listCart = itemCart.cartItems.ToList();
                foreach (var item in listCart)
                {
                    Product _tmp = _context.Products.Where(p => p.Id == item.id_product).FirstOrDefault();
                    if(_tmp != null)
                    {
                        list.Add(_tmp);
                    }
                }
                ViewData["cart"] = listCart;
            }else
            {
                ViewData["cart"] = null;
            }
            return View(list);
        }
    }
    [HttpPost]
    public async Task<JsonResult> RemoveProduct(int idProduct, int sl)
    {
        Product product = await _context.Products.Where(p => p.Id.Equals(idProduct)).FirstOrDefaultAsync();
        if(product == null)
        {
            return Json(new {
                message = "Không tìm thấy sản phẩm!",
                position = "0"
            });
        }
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return Json(new {
                message = "Bạn phải đăng nhập mới sử dụng được chức năng này!",
                position = "0"
            });
        }else
        {
            CartData item = new CartData()
            {
                id_user = (int)idUser,
                cartItems = new List<CartItem>()
                {
                    new CartItem()
                    {
                        id_product = idProduct,
                        quantity = sl
                    }
                }
            };
            _cartContext.Remove(item);
            if(_cartContext.WriteData())
            {
                return Json(new {
                    message = "Xoá sản phẩm khỏi giỏ hàng thành công!",
                    position = "1"
                });
            }else
            {
                return Json(new {
                    message = "Không thể xoá sản phẩm khỏi giỏ hàng",
                    position = "0"
                });
            }
            
        }
    }
    
    [HttpPost]
    public async Task<JsonResult> SubProduct(int idProduct)
    {
        Product product = await _context.Products.Where(p => p.Id.Equals(idProduct)).FirstOrDefaultAsync();
        if(product == null)
        {
            return Json(new {
                message = "Không tìm thấy sản phẩm!",
                position = "0"
            });
        }
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return Json(new {
                message = "Bạn phải đăng nhập mới sử dụng được chức năng này!",
                position = "0"
            });
        }else
        {
            CartData item = new CartData()
            {
                id_user = (int)idUser,
                cartItems = new List<CartItem>()
                {
                    new CartItem()
                    {
                        id_product = idProduct,
                        quantity = 1
                    }
                }
            };
            _cartContext.Remove(item);
            if(_cartContext.WriteData())
            {
                return Json(new {
                    message = "Bớt 1 sản phẩm!",
                    position = "1"
                });
            }else
            {
                return Json(new {
                    message = "Không thể bớt 1 sản phẩm",
                    position = "0"
                });
            }
            
        }
    }
    
    [HttpPost]
    public async Task<JsonResult> AddProduct(int idProduct)
    {
        Product product = await _context.Products.Where(p => p.Id.Equals(idProduct)).FirstOrDefaultAsync();
        if(product == null)
        {
            return Json(new {
                message = "Không tìm thấy sản phẩm!",
                position = "0"
            });
        }
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return Json(new {
                message = "Bạn phải đăng nhập mới sử dụng được chức năng này!",
                position = "0"
            });
        }else
        {
            CartData item = new CartData()
            {
                id_user = (int)idUser,
                cartItems = new List<CartItem>()
                {
                    new CartItem()
                    {
                        id_product = idProduct,
                        quantity = 1
                    }
                }
            };
            _cartContext.Add(item);
            if(_cartContext.WriteData())
            {
                return Json(new {
                    message = "Thêm 1 sản phẩm!",
                    position = "1"
                });
            }else
            {
                return Json(new {
                    message = "Không thể thêm 1 sản phẩm!",
                    position = "0"
                });
            }
            
        }
    }
    
    [HttpPost]
    public async Task<JsonResult> addToCart(int idProduct, int sl = 1)
    {
        Product product = await _context.Products.Where(p => p.Id.Equals(idProduct)).FirstOrDefaultAsync();
        if(product == null)
        {
            return Json(new {
                message = "Không tìm thấy sản phẩm!",
                position = "0"
            });
        }
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return Json(new {
                message = "Bạn phải đăng nhập mới sử dụng được chức năng này!",
                position = "0"
            });
        }else
        {
            int sl_ = _cartContext.getQuantity((int)idUser, idProduct) + sl;
            if( sl_ > product.Quantity)
            {
                return Json(new {
                message = "Số lượng sản phẩm bạn đặt đã đạt tối đa!",
                position = "0"
            });
            }

            CartData item = new CartData()
            {
                id_user = (int)idUser,
                cartItems = new List<CartItem>()
                {
                    new CartItem()
                    {
                        id_product = idProduct,
                        quantity = sl
                    }
                }
            };
            _cartContext.Add(item);
            if(_cartContext.WriteData())
            {
                return Json(new {
                    message = "Thêm sản phẩm vào giỏ hàng thành công!",
                    position = "1"
                });
            }else
            {
                return Json(new {
                    message = "Không thể thêm sản phẩm vào giỏ hàng",
                    position = "0"
                });
            }
        }
    } 

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
