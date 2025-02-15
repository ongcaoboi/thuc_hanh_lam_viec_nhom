using Microsoft.AspNetCore.Mvc;
using web_bh.Models;
using Microsoft.EntityFrameworkCore;

namespace web_bh.Areas_Admin_Controllers
{
    [Area("Admin")]
    public class OrderManager : Controller
    {
        private web_bhContext _context;
        
        public OrderManager(web_bhContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Order> orders = _context.Orders.Include(x => x.OrderDetails.OrderBy(p => p.Id)).OrderByDescending(c => c.Id).ToList();
            ViewData["status"] = _context.Statuses.ToList();
            ViewData["product"] = _context.Products.ToList(); 
            return View(orders);
        }
        [HttpPost]
        public async Task<JsonResult> ChangeStatus(int idOrder, int idStatus)
        {
            bool isAdmin = HttpContext.Session.GetInt32("role") == 1? true: false;
            if(!isAdmin)
            {
                return Json(new {
                    message = "Bạn phải là admin mới sử dụng được chức năng này!",
                    position = "0"
                });
            }else
            {
                Status status = _context.Statuses.Where(p => p.Id == idStatus).FirstOrDefault();
                if(status == null)
                {
                    return Json(new {
                        message = "Trạng thái không tồn tại!",
                        position = "0"
                    });
                }
                Order order = _context.Orders.Where(q => q.Id == idOrder).Include(x => x.OrderDetails.OrderBy(p => p.Id)).First();
                if(order == null)
                {
                    return Json(new {
                        message = "Đơn hàng không tồn tại!",
                        position = "0"
                    });
                }
                if(order.IdStatus == 4){
                    return Json(new {
                        message = "Đơn hàng đã bị hủy rồi!",
                        position ="0"
                    });
                }
                if(idStatus == 4){
                    bool isError = false;
                    foreach (OrderDetail item in order.OrderDetails)
                    {
                        Product product = _context.Products.Where(p => p.Id == item.IdProduct).First();
                        if(product == null){
                            isError = true; 
                            break;  
                        }
                        product.Quantity += Int32.Parse(item.Num.ToString());
                        _context.Update(product);
                    }
                    if(isError){
                        return Json(new {
                            message = "Có lỗi xảy ra, vui lòng thử lại sau!",
                            position = "0"
                        });
                    }
                }
                order.IdStatus = status.Id;
                _context.Update(order);
                await _context.SaveChangesAsync();
                return Json(new {
                    message = "Thay đổi trạng thái thành công!",
                    position ="1"
                });
            }
        }
    }
}