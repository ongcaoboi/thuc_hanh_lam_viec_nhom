#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web_bh.Models;

namespace web_bh.Areas_Admin_Controllers
{
    [Area("Admin")]
    public class ProductManager : Controller
    {
        private readonly web_bhContext _context;

        public ProductManager(web_bhContext context)
        {
            _context = context;
        }

        // GET: ProductManager
        public async Task<IActionResult> Index(string q = null)
        {
            if(q == null)
            {
                var web_bhContext = _context.Products.Include(p => p.IdCategoryNavigation);
                return View(await web_bhContext.ToListAsync());
            }else
            {
                var web_bhContext = _context.Products.Include(p => p.IdCategoryNavigation).Where(p => p.Title.Contains(q));
                return View(await web_bhContext.ToListAsync());
            }
        }

        // GET: ProductManager/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.IdCategoryNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["galleries"] = _context.Galleries.Where(p => p.IdProduct == id).ToList();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddPicture(int idProduct, IFormFile file)
        {
            if(file == null)
            {
                return RedirectToAction("Details", "ProductManager", new { id = idProduct });
            }
            Product product = _context.Products.Where(p => p.Id == idProduct).FirstOrDefault();
            if(product == null)
            {
                return RedirectToAction("Details", "ProductManager", new { id = idProduct });
            }
            Gallery gallery = new Gallery();
            gallery.IdProduct = idProduct;
            string fileName = "p_"+idProduct.ToString()+"_"+DateTime.Now.ToString("yyyyMMdd-HHmmss")+file.FileName;
            string path = @"./wwwroot/img/product/"+fileName;

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            gallery.Thumbnail = fileName;
            _context.Galleries.Add(gallery);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "ProductManager", new { id = idProduct });
        }

        [HttpPost]
        public async Task<IActionResult> RemovePicture(int id, int idGallery)
        {
            Product product = _context.Products.Where(p => p.Id == id).FirstOrDefault();
            if(product == null)
            {
                return RedirectToAction("Details", "ProductManager", new { id = id });
            }
            Gallery gallery = _context.Galleries.Where(p => p.Id == idGallery).FirstOrDefault();
            if(gallery == null)
            {
                return RedirectToAction("Details", "ProductManager", new { id = id });
            }else
            {
                System.IO.File.Delete(@"./wwwroot/img/product/"+gallery.Thumbnail);
                _context.Galleries.Remove(gallery);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Details", "ProductManager", new { id = id });
            }
        }
        // GET: ProductManager/Create
        public IActionResult Create()
        {
            ViewData["IdCategory"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: ProductManager/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(int IdCategory, string Title,int Price, string Description, int Quantity, IFormFile file)
        {
            if(Title == null ||  Description == null ||  Title =="" || Quantity <= 0)
            {
                return NotFound();
            }
            Category category = _context.Categories.Where(p => p.Id == IdCategory).FirstOrDefault();
            if(category == null)
            {
                return NotFound();
            }
            if(file == null)
            {
                return RedirectToAction("Create", "ProductManager");
            }
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss")+file.FileName;
            string path = @"./wwwroot/img/product/"+fileName;

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            Product prd = new Product();
            prd.IdCategory = IdCategory;
            prd.Title = Title;
            prd.Price = Price;
            prd.Thumbnail = fileName;
            prd.Description = Description;
            prd.Quantity = Quantity;
                
            _context.Add(prd);
            await _context.SaveChangesAsync();
            // return RedirectToAction(nameof(Index));
            return RedirectToAction("Details", "ProductManager",new { id = prd.Id });
        }

        // GET: ProductManager/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["IdCategory"] = new SelectList(_context.Categories, "Id", "Name", product.IdCategory);
            return View(product);
        }

        // POST: ProductManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id,int IdCategory,string Title,int Price,string Thumbnail,string Description,int Quantity, IFormFile file)
        {
            if(Title == null || Thumbnail == null || Description == null ||  Title =="" || Thumbnail == "" || Quantity <= 0)
            {
                return NotFound();
            }
            Category category = _context.Categories.Where(p => p.Id == IdCategory).FirstOrDefault();
            if(category == null)
            {
                return NotFound();
            }
            Product prd = _context.Products.Where(p => p.Id == id).FirstOrDefault();
            if (prd == null)
            {
                return NotFound();
            }
            if(file == null)
            {
                prd.Thumbnail = Thumbnail;
            }else
            {
                System.IO.File.Delete(@"./wwwroot/img/product/"+prd.Thumbnail);

                string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss")+file.FileName;

                string path = @"./wwwroot/img/product/"+fileName;

                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);

                prd.Thumbnail = fileName;
            }
            prd.IdCategory = IdCategory;
            prd.Title = Title;
            prd.Price = Price;
            prd.Description = Description;
            prd.Quantity = Quantity;
            

            // if (ModelState.IsValid)
            // {
                try
                {
                    _context.Update(prd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(prd.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", "ProductManager",new { id = prd.Id });
            // }
        }

        // GET: ProductManager/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.IdCategoryNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: ProductManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            List<Gallery> galleries = _context.Galleries.Where(p => p.IdProduct == id).ToList();
            _context.Products.Remove(product);
            _context.Galleries.RemoveRange(galleries);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
