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
public void DownloadExcel(int id)
    {
        List<OrderDetail> ordersdetail = _context.OrderDetails.Where(q => q.IdOrder == id).ToList();
        List<Order> orders = _context.Orders.ToList();
        List<Product> products = _context.Products.ToList();

        ExcelPackage Ep = new ExcelPackage();
        // Add Sheet vào file Excel
        ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Hoa Don");

        // Đổ data vào Excel file
        Sheet.Cells["A1"].Value = "Cửa hàng: THƠ AN";
        Sheet.Cells["A2"].Value = "Địa chỉ: Ngã ba Cây Xoài";
        Sheet.Cells["A3"].Value = "Hotline: 035353523";
        Sheet.Cells["C4"].Value = "PHIẾU GIAO HÀNG";
        foreach (var item in ordersdetail.Take(1))
        {
            foreach (var item1 in orders.Where(q => q.Id == item.IdOrder).Take(1))
            {
                Sheet.Cells["A5"].Value = "Người nhận hàng: " + item1.FullName;
                Sheet.Cells["A6"].Value = "Địa chỉ: " + item1.Address;
                Sheet.Cells["A7"].Value = "Hotline: " + item1.PhoneNumber;
            }
        }
        Sheet.Cells["B9"].Value = "STT";
        Sheet.Cells["C9"].Value = "Mặt hàng";
        Sheet.Cells["D9"].Value = "Số lượng";
        Sheet.Cells["E9"].Value = "Đơn giá";
        Sheet.Cells["F9"].Value = "Thành tiền";
        int row = 10;
        int i = 1;
        string PriceDiscount, TotalPrice;
        foreach (var item in ordersdetail)
        {
            foreach (var item2 in products.Where(q => q.Id == item.IdProduct).Take(1))
            {
                Sheet.Cells[string.Format("B{0}", row)].Value = i;
                Sheet.Cells[string.Format("C{0}", row)].Value = item2.Title;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Num;
                Sheet.Cells[string.Format("E{0}", row)].Value = (PriceDiscount = String.Format("{0:0,00}", item2.Price));
                Sheet.Cells[string.Format("F{0}", row)].Value = (TotalPrice = String.Format("{0:0,00}", item2.Price*item.Num));
               
            }
            row++;
            i++;
        }
        string TotalMoney;
        string tenDonHang = "";
        foreach (var item in ordersdetail.Take(1))
        {
            foreach (var item3 in orders.Where(q => q.Id == item.IdOrder).Take(1))
            {
                Sheet.Cells[string.Format("F{0}", row)].Value = "Tổng Tiền: " + (TotalMoney = String.Format("{0:0,00}", item.TotalMoney)) + " vnđ";
                row++;
            }
            

        }
        Sheet.Cells[string.Format("A{0}", row)].Value = "Người nhận hàng";

        Sheet.Cells["A:AZ"].AutoFitColumns();
        Response.Clear();
        //content Type dành cho file excel
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        // File name của Excel này là Tên đơn hàng
Response.AddHeader("Content-Disposition", "attachment; filename=" + "Name" + ".xlsx");
        // Lưu file excel của chúng ta như 1 mảng byte để trả về response
        Response.BinaryWrite(Ep.GetAsByteArray());
        Response.End();
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
