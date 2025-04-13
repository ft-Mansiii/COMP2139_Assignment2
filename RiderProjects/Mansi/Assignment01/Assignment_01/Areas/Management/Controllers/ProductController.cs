using Assignment01.Areas.Management.Models;
using Assignment01.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Assignment01.Areas.Management.Controllers;

[Area("Management")]
[Route("[area]/[controller]/[action]")]
[Authorize]
public class ProductController : Controller
{
    private readonly Assignment01Db _context;

    public ProductController(Assignment01Db context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        try
        {
            Log.Information("User {User} accessed the Product Index page at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);

            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading products by user {User} at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);
            TempData["ErrorMessage"] = "An error occurred while loading the products. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }

    

    public IActionResult Search()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View("Search");
    }


    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Create()
    {
        try
        {
            Log.Information("User {User} accessed the product creation page at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View(new Product());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading product creation page by user {User} at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);
            TempData["ErrorMessage"] =
                "An error occurred while loading the product creation page. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("ProductId,ProductName,Price,Quantity,LowStockThreshold,CategoryId")] Product product)
    {
        try
        {
            Log.Information("User {User} attempted to create a new product at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);

            var category = await _context.Categories.FindAsync(product.CategoryId);
            if (category != null) product.Category = category;

            ModelState.ClearValidationState("Category");
            TryValidateModel(product);

            if (ModelState.IsValid)
            {
                Log.Information("User {User} successfully created product {ProductName} at {Time}",
                    User.Identity?.Name ?? "Anonymous", product.ProductName, DateTime.Now);
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Product");
            }

            ViewData["CategoryId"] =
                new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating product by user {User} at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);
            TempData["ErrorMessage"] = "An error occurred while creating the product. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            Log.Information("User {User} accessed the product edit page for product ID {Id} at {Time}",
                User.Identity?.Name ?? "Anonymous", id, DateTime.Now);
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading product edit page by user {User} at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);
            TempData["ErrorMessage"] = "An error occurred while loading the edit page. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }
    
    [HttpPost]
    public IActionResult UpdateStock(int productId, int quantityToOrder)
    {
        try
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            // Check if quantity to order is valid
            if (quantityToOrder > product.Quantity)
            {
                return Json(new { success = false, message = "Not enough stock available." });
            }

            // Update the stock and save
            product.Quantity -= quantityToOrder;
            _context.SaveChanges();

            return Json(new { success = true, updatedStock = product.Quantity });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating stock for product ID {Id} by user {User} at {Time}",
                productId, User.Identity?.Name ?? "Anonymous", DateTime.Now);
            return Json(new { success = false, message = "An error occurred while updating stock." });
        }
    }

    
    

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product product)
    {
        try
        {
            Log.Information("User {User} attempted to edit product ID {Id} at {Time}",
                User.Identity?.Name ?? "Anonymous", product.ProductId, DateTime.Now);
            var category = await _context.Categories.FindAsync(product.CategoryId);
            if (category != null) product.Category = category;

            ModelState.ClearValidationState("Category");
            TryValidateModel(product);

            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                Log.Information("User {User} successfully edited product ID {Id} at {Time}",
                    User.Identity?.Name ?? "Anonymous", product.ProductId, DateTime.Now);
                return RedirectToAction("Index");
            }

            return View(product);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error editing product ID {Id} by user {User} at {Time}",
                product.ProductId, User.Identity?.Name ?? "Anonymous", DateTime.Now);
            TempData["ErrorMessage"] = "An error occurred while editing the product. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Delete(int id)
    {
        try
        {
            Log.Information("User {User} accessed the product delete page for product ID {Id} at {Time}",
                User.Identity?.Name ?? "Anonymous", id, DateTime.Now);
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(m => m.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading product delete page by user {User} at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);
            TempData["ErrorMessage"] = "An error occurred while loading the delete page. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            Log.Information("User {User} attempted to delete product ID {Id} at {Time}",
                User.Identity?.Name ?? "Anonymous", id, DateTime.Now);
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            Log.Information("User {User} successfully deleted product ID {Id} at {Time}",
                User.Identity?.Name ?? "Anonymous", id, DateTime.Now);
            return RedirectToAction("Index", "Product");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting product ID {Id} by user {User} at {Time}",
                id, User.Identity?.Name ?? "Anonymous", DateTime.Now);

            TempData["ErrorMessage"] = "An error occurred while deleting the product. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    public IActionResult PlaceOrder(int[] ProductIds, int[] OrderQuantities, string deliveryAddress)
    {
        try
        {
            for (var i = 0; i < ProductIds.Length; i++)
            {
                var productId = ProductIds[i];
                var quantityToOrder = OrderQuantities[i];

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
            return RedirectToAction("Index", "Order");
        }
        catch
        {
            TempData["ErrorMessage"] = "An error occurred while placing the order. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }
}