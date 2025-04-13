using Assignment01.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Assignment01.Areas.Management.Controllers;

[Area("Management")]
[Route("api/[controller]/[action]")]
public class ProductSearchController : Controller
{
    private readonly Assignment01Db _context;

    public ProductSearchController(Assignment01Db context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> SearchFilter(string query, int? categoryId, double? minPrice, double? maxPrice,
        bool? lowStock)
    {
        try
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();

            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(query))
                productsQuery = productsQuery.Where(p => p.ProductName.ToLower().Contains(query.ToLower()));

            if (categoryId.HasValue) productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue) productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue) productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);

            if (lowStock.HasValue) productsQuery = productsQuery.Where(p => p.Quantity < p.LowStockThreshold);

            var products = await productsQuery.ToListAsync();
           
                Log.Information("User {User} made an AJAX search at {Time}",
                    User.Identity?.Name ?? "Anonymous", DateTime.Now);
                return Json(products);
            

            /*Log.Information("User {User} accessed search results at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);
            return View(products);*/
        }

        catch (Exception ex)
        {
            Log.Error(ex, "Error searching for products by user {User} at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);
            TempData["ErrorMessage"] = "An error occurred while searching for products. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }
}