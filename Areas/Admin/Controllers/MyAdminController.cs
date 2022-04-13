#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web_bh.Models;
using Newtonsoft.Json;

namespace web_bh.Areas_Admin_Controllers
{
    [Area("Admin")]
    public class MyAdminController : Controller
    {
        private readonly web_bhContext _context;

        public MyAdminController(web_bhContext context)
        {
            _context = context;
        }
        public IActionResult Index(){
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> layThongKe(int thang, int nam)
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
                List<Order> orders = _context.Orders.Include(x => x.OrderDetails.OrderBy(p => p.Id)).ToList();
                List<Order> order = new List<Order>();
                foreach (Order item in orders)
                {
                    if(item.OrderDate.Year == nam){
                        if(item.OrderDate.Month == thang){
                            order.Add(item);
                        }
                    }
                }
                int soNgay = (int)DateTime.DaysInMonth(nam, thang);

                int[] arr = new int[soNgay];
                int[] doanhThu = new int[soNgay];

                for(int i = 0; i < soNgay ; i++){
                    arr[i] = 0;
                    doanhThu[i] = 0;
                    foreach (Order item in order)
                    {
                        if(item.OrderDate.Day == i+1){
                            arr[i]++;
                            foreach (var item_1 in item.OrderDetails.ToList())
                            {
                                doanhThu[i] += (int)item_1.TotalMoney;
                            }
                        }
                    }
                }
                return Json(new {
                    songay = soNgay,
                    donhang = arr,
                    doanhthu = doanhThu
                });
                

            }

        }
    }
   

}
