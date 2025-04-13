using Assignment01.Areas.Management.Models;
using Assignment01.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Assignment01.Areas.Management.Controllers;

[Area("Management")]
[Route("[area]/[controller]/[action]")]
[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
    private readonly Assignment01Db _context;

    public CategoryController(Assignment01Db context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            Log.Information("User {User} accessed endpoint {Endpoint} at {Time}",
                User.Identity?.Name ?? "Anonymous", HttpContext.Request.Path, DateTime.Now);

            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading categories by user {User} at {Time}",
                User.Identity?.Name ?? "Anonymous", DateTime.Now);
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        try
        {
            Log.Information("User {User} accessed endpoint {Endpoint} at {Time}",
                User.Identity?.Name ?? "Anonymous", HttpContext.Request.Path, DateTime.Now);
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null) return NotFound();
            return View(category);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving category details for ID {Id} by user {User} at {Time}",
                id, User.Identity?.Name ?? "Anonymous", DateTime.Now);
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        Log.Information("User {User} accessed endpoint {Endpoint} at {Time}",
            User.Identity?.Name ?? "Anonymous", HttpContext.Request.Path, DateTime.Now);
        return View(new Category());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CategoryDescription")] Category category)
    {
        if (ModelState.IsValid)
            try
            {
                Log.Information("User {User} created a new category at {Time}",
                    User.Identity?.Name ?? "Anonymous", DateTime.Now);
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create category by user {User} at {Time}",
                    User.Identity?.Name ?? "Anonymous", DateTime.Now);
            }

        return View(category);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        try
        {
            Log.Information("User {User} accessed endpoint {Endpoint} for editing at {Time}",
                User.Identity?.Name ?? "Anonymous", HttpContext.Request.Path, DateTime.Now);
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }
        catch (Exception)
        {
            TempData["Error"] = "Error loading category for editing.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("CategoryId,CategoryName,CategoryDescription")] Category category)
    {
        if (id != category.CategoryId) return NotFound();

        if (ModelState.IsValid)
            try
            {
                Log.Information("User {User} edited category ID {Id} at {Time}",
                    User.Identity?.Name ?? "Anonymous", id, DateTime.Now);
                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId)) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update category ID {Id} by user {User} at {Time}",
                    id, User.Identity?.Name ?? "Anonymous", DateTime.Now);
            }

        return View(category);
    }

    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        try
        {
            Log.Information("User {User} accessed endpoint {Endpoint} for deletion at {Time}",
                User.Identity?.Name ?? "Anonymous", HttpContext.Request.Path, DateTime.Now);
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null) return NotFound();
            return View(category);
        }
        catch (Exception)
        {
            TempData["Error"] = "Error loading category for deletion.";
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            Log.Information("User {User} deleted category ID {Id} at {Time}",
                User.Identity?.Name ?? "Anonymous", id, DateTime.Now);
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete category ID {Id} by user {User} at {Time}",
                id, User.Identity?.Name ?? "Anonymous", DateTime.Now);
            return RedirectToAction("Error", "Home");
        }
    }


    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.CategoryId == id);
    }
}



