using Assignment01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Assignment01.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ProductController : Controller
{
    private readonly Assignment01DB _context;

    public ProductController(Assignment01DB context)
    {
        _context = context;
    }
    
    public IActionResult Index()
    {
        var products = _context.Products.Include(p => p.Category).ToList();
        return View(products);
    }
    


    public async Task<IActionResult> Search(string query, int? categoryId, double? minPrice, double? maxPrice, bool? lowStock)
    {
       
        ViewBag.Categories = await _context.Categories.ToListAsync();

        var productsQuery = _context.Products.AsQueryable();

       
        if (!string.IsNullOrEmpty(query))
        {
            productsQuery = productsQuery.Where(p => p.ProductName.ToLower().Contains(query.ToLower()));
        }

        if (categoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
        }

        if (lowStock.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.Quantity < p.LowStockThreshold);
        }

        var products = await productsQuery.ToListAsync();

        if (products.Count == 0)
        {
            TempData["Message"] = "No products found.";
        }

        return View(products);   
    }



    
    [HttpGet]
    public IActionResult Create()
    {
        
        ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");
    
        return View(new Product()); 
    }


    
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,Quantity,LowStockThreshold,CategoryId")] Product product)
    {
        var category=_context.Categories.FindAsync(product.CategoryId).GetAwaiter().GetResult();
        if (category != null)
        {
            product.Category = category;
        }
        
        ModelState.ClearValidationState("Category");
        TryValidateModel(product);
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index" ,"Product"); 
        }

        
        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
        return RedirectToAction("Index" ,"Product");
    }

    

   
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p=>p.ProductId==id);
        if (product is null)
        {
            return NotFound();
        }
        return View(product);
    }

 
    [HttpPost]
    public async Task<IActionResult> Edit(Product product)
    {
        var category=_context.Categories.FindAsync(product.CategoryId).GetAwaiter().GetResult();
        if (category != null)
        {
            product.Category = category;
        }
        
        ModelState.ClearValidationState("Category");
        TryValidateModel(product);
        if (ModelState.IsValid)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(product);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var product = _context.Products
            .Include(p => p.Category) 
            .FirstOrDefault(m => m.ProductId == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index" , "Product");
    }

    [HttpPost]
    public IActionResult PlaceOrder(int[] ProductIds, int[] OrderQuantities, string deliveryAddress)
    {
        for (int i = 0; i < ProductIds.Length; i++)
        {
            int productId = ProductIds[i];
            int quantityToOrder = OrderQuantities[i];

            if (quantityToOrder > 0)
            {
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    product.QuantityToOrder = quantityToOrder;
                    _context.Update(product);
                    _context.SaveChanges();
                }
            }
        }
        
        
        TempData["SuccessMessage"] = "Order placed successfully!";
        return RedirectToAction("Index" , "Order");
    }


}