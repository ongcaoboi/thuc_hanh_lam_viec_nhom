using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using web_bh.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using OfficeOpenXml;

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
    public IActionResult ExportToCSV(int num)
    {
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return RedirectToAction("Index", "MyOrder");
        }
        Order order = _context.Orders.Where(q => q.Id == num).Include(x => x.OrderDetails.OrderBy(p => p.Id)).First();
        // Order order = _context.Orders.Where(q => q.Id == num).Include(od => od.OrderDetails.Select(odd => odd.IdOrder)).FirstOrDefault();
        if(order == null)
        {
            return RedirectToAction("Index", "MyOrder");
        }
        if(idUser != order.IdUser)
        {
            return RedirectToAction("Index", "MyOrder");
        }
        var buider = new StringBuilder();
        buider.AppendLine(", HÓA ĐƠN MUA HÀNG,,");
        buider.AppendLine(",,,");
        buider.AppendLine(",Người nhận,,");
        buider.AppendLine($",Tên,{order.FullName},");
        buider.AppendLine($",Địa chỉ,{order.Address},");
        buider.AppendLine($",SĐT,{order.PhoneNumber},");
        buider.AppendLine(",,,");
        buider.AppendLine(",Thanh toán khi nhận hàng,Giao hàng miễn phí,");
        buider.AppendLine($",Ghi chú,{order.Note},");
        buider.AppendLine(",,,");
        buider.AppendLine("STT,Tên sản phẩm,Số lượng,Tổng tiền");
        int stt = 1;
        int sum = 0;
        foreach(OrderDetail item in order.OrderDetails.ToList()){
            Product product = _context.Products.Where(x => x.Id == item.IdProduct)
                .First();
            buider.AppendLine($"{stt} , {product.Title}, {item.TotalMoney},");
            sum += Int32.Parse(item.TotalMoney.ToString());
        }
        buider.AppendLine(",,,");
        buider.AppendLine($",Ngày đặt hàng,{order.OrderDate},");
        buider.AppendLine($",Tổng tiền phải trả,,{sum}");
        return File(Encoding.UTF8.GetBytes(buider.ToString()), "text/csv", "DonHang.csv");
    }
    public async Task<IActionResult> CancelOrder(int idOrder)
    {
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return Json(new {
                message = "Hãy đăng nhập",
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
        if(order.IdStatus == 1)
        {
            if(order.IdUser != idUser)
            {
                return Json(new {
                    message = "Đơn hàng không tồn tại!",
                    position = "0"
                });
            }
            
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
            order.IdStatus = 4;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return Json(new {
                message = "Hủy đơn hàng thành công!",
                position ="1"
            });
        }
        else
        {
            return Json(new {
                message = "Không thể hủy đơn hàng!",
                position = "0"
            });
        }
        
    }

    public IActionResult DownloadExcel(int num)
    {
        int? idUser = HttpContext.Session.GetInt32("user");
        if(idUser == null)
        {
            return RedirectToAction("Index", "MyOrder");
        }
        Order order = _context.Orders.Where(q => q.Id == num).Include(x => x.OrderDetails.OrderBy(p => p.Id)).First();
        if(order == null)
        {
            return RedirectToAction("Index", "MyOrder");
        }
        if(idUser != order.IdUser)
        {
            return RedirectToAction("Index", "MyOrder");
        }
        var buider = new StringBuilder();

        List<Order> orders = _context.Orders.ToList();
        List<Product> products = _context.Products.ToList();

        var stream = new MemoryStream();

        ExcelPackage Ep = new ExcelPackage(stream);
        // Add Sheet vào file Excel
        ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Hoa Don");

        // Đổ data vào Excel file
        Sheet.Cells["B1"].Value = "PHIẾU GIAO HÀNG";
        Sheet.Cells["A2"].Value = "Cửa hàng: ";
        Sheet.Cells["A3"].Value = "Địa chỉ: ";
        Sheet.Cells["A4"].Value = "Hotline: ";
        Sheet.Cells["B2"].Value = "PN Shop";
        Sheet.Cells["B3"].Value = "170 An Dương Vương, TP.Quy nhơn, Tỉnh Bình Định";
        Sheet.Cells["B4"].Value = "0987654321";
        Sheet.Cells["A5"].Value = "Người nhận hàng: ";
        Sheet.Cells["A6"].Value = "Địa chỉ: ";
        Sheet.Cells["A7"].Value = "Hotline: ";
        Sheet.Cells["B5"].Value = order.FullName;
        Sheet.Cells["B6"].Value = order.Address;
        Sheet.Cells["B7"].Value = order.PhoneNumber;
        Sheet.Cells["A9"].Value = "STT";
        Sheet.Cells["B9"].Value = "Mặt hàng";
        Sheet.Cells["C9"].Value = "Số lượng";
        Sheet.Cells["D9"].Value = "Đơn giá";
        Sheet.Cells["E9"].Value = "Thành tiền";
        int row = 10;
        int i = 1;
        int sum = 0;
        string PriceDiscount, TotalPrice;
        
        foreach(OrderDetail item in order.OrderDetails.ToList()){
            Product product = _context.Products.Where(x => x.Id == item.IdProduct)
                .First();

            Sheet.Cells[string.Format("A{0}", row)].Value = i;
            Sheet.Cells[string.Format("B{0}", row)].Value = product.Title;
            Sheet.Cells[string.Format("C{0}", row)].Value = item.Num;
            Sheet.Cells[string.Format("D{0}", row)].Value = (PriceDiscount = String.Format("{0:0,00}", product.Price));
            Sheet.Cells[string.Format("E{0}", row)].Value = (TotalPrice = String.Format("{0:0,00}", item.TotalMoney));

            sum += Int32.Parse(item.TotalMoney.ToString());
            row++;
            i++;
        }
        
        Sheet.Cells[string.Format("A{0}", row)].Value = "Ngày đặt hàng: ";
        Sheet.Cells[string.Format("B{0}", row++)].Value = order.OrderDate.GetDateTimeFormats();

        Sheet.Cells[string.Format("A{0}", row)].Value = "Tổng Tiền: ";
        Sheet.Cells[string.Format("E{0}", row)].Value =  String.Format("{0:0,00}", sum) + " vnđ";

        Sheet.Cells["A:AZ"].AutoFitColumns();

        Ep.Save();

        stream.Position = 0;

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DonHang.xlsx");

    }

    // public void DownloadExcel(int id)
    // {
    //     List<OrderDetail> ordersdetail = _context.OrderDetails.Where(q => q.IdOrder == id).ToList();
    //     List<Order> orders = _context.Orders.ToList();
    //     List<Product> products = _context.Products.ToList();

    //     using ExcelPackage Ep = new ExcelPackage();
    //     // Add Sheet vào file Excel
    //     ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Hoa Don");

    //     // Đổ data vào Excel file
    //     Sheet.Cells["A1"].Value = "Cửa hàng: THƠ AN";
    //     Sheet.Cells["A2"].Value = "Địa chỉ: Ngã ba Cây Xoài";
    //     Sheet.Cells["A3"].Value = "Hotline: 035353523";
    //     Sheet.Cells["C4"].Value = "PHIẾU GIAO HÀNG";
    //     foreach (var item in ordersdetail.Take(1))
    //     {
    //         foreach (var item1 in orders.Where(q => q.Id == item.IdOrder).Take(1))
    //         {
    //             Sheet.Cells["A5"].Value = "Người nhận hàng: " + item1.FullName;
    //             Sheet.Cells["A6"].Value = "Địa chỉ: " + item1.Address;
    //             Sheet.Cells["A7"].Value = "Hotline: " + item1.PhoneNumber;
    //         }
    //     }
    //     Sheet.Cells["B9"].Value = "STT";
    //     Sheet.Cells["C9"].Value = "Mặt hàng";
    //     Sheet.Cells["D9"].Value = "Số lượng";
    //     Sheet.Cells["E9"].Value = "Đơn giá";
    //     Sheet.Cells["F9"].Value = "Thành tiền";
    //     int row = 10;
    //     int i = 1;
    //     string PriceDiscount, TotalPrice;
    //     foreach (var item in ordersdetail)
    //     {
    //         foreach (var item2 in products.Where(q => q.Id == item.IdProduct).Take(1))
    //         {
    //             Sheet.Cells[string.Format("B{0}", row)].Value = i;
    //             Sheet.Cells[string.Format("C{0}", row)].Value = item2.Title;
    //             Sheet.Cells[string.Format("D{0}", row)].Value = item.Num;
    //             Sheet.Cells[string.Format("E{0}", row)].Value = (PriceDiscount = String.Format("{0:0,00}", item2.Price));
    //             Sheet.Cells[string.Format("F{0}", row)].Value = (TotalPrice = String.Format("{0:0,00}", item2.Price*item.Num));
               
    //         }
    //         row++;
    //         i++;
    //     }
    //     string TotalMoney;
    //     string tenDonHang = "";
    //     foreach (var item in ordersdetail.Take(1))
    //     {
    //         foreach (var item3 in orders.Where(q => q.Id == item.IdOrder).Take(1))
    //         {
    //             Sheet.Cells[string.Format("F{0}", row)].Value = "Tổng Tiền: " + (TotalMoney = String.Format("{0:0,00}", item.TotalMoney)) + " vnđ";
    //             row++;
    //         }
            

    //     }
    //     Sheet.Cells[string.Format("A{0}", row)].Value = "Người nhận hàng";

    //     Sheet.Cells["A:AZ"].AutoFitColumns();

    //     Response.Clear();
    //     //content Type dành cho file excel
    //     Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    //     // File name của Excel này là Tên đơn hàng
    //     Response.AddHeader("Content-Disposition", "attachment; filename=" + "Name" + ".xlsx");
    //     // Lưu file excel của chúng ta như 1 mảng byte để trả về response
    //     Response.BinaryWrite(Ep.GetAsByteArray());
    //     Response.End();
    // }
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
