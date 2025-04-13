using Assignment01.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Assignment01.Areas.Management.Controllers;

[Area("Management")]
[Route("[area]/[controller]/[action]")]
[Authorize]
public class OrderController : Controller
{
    private readonly Assignment01Db _context;
    private readonly ILogger<OrderController> _logger;


    public OrderController(Assignment01Db context, ILogger<OrderController> logger)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var user = User?.Identity?.Name ?? "Anonymous";

        try
        {
            var products = _context.Products
                .Where(p => p.QuantityToOrder > 0)
                .ToList();

            Log.Information("User {User} accessed Order Index at {Time}", user, DateTime.UtcNow);

            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching products for the order.");
            TempData["ErrorMessage"] = "An error occurred while loading the product list. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }

    

    
    

    [HttpPost]
    public IActionResult PlaceOrder(List<int> ProductIds, List<int> OrderQuantities, string deliveryAddress)
    {
        var user = User?.Identity?.Name ?? "Anonymous";


        try
        {
            if (ProductIds == null || OrderQuantities == null || ProductIds.Count != OrderQuantities.Count)
            {
                Log.Warning("User {User} submitted invalid order data at {Time}", user, DateTime.UtcNow);

                TempData["ErrorMessage"] = "Invalid order data.";
                return RedirectToAction("Index", "Order");
            }

            for (var i = 0; i < ProductIds.Count; i++)
                if (OrderQuantities[i] > 0)
                {
                    var product = _context.Products.FirstOrDefault(p => p.ProductId == ProductIds[i]);
                    if (product != null)
                    {
                        product.QuantityToOrder = OrderQuantities[i];
                        Log.Information("User {User} ordered {Qty} of ProductId {ProductId} at {Time}",
                            user, OrderQuantities[i], ProductIds[i], DateTime.UtcNow);
                    }
                }

            _context.SaveChanges();
            Log.Information("Order placed successfully by {User} at {Time}. Delivery Address: {Address}",
                user, DateTime.UtcNow, deliveryAddress);
            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Index", "Order");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while placing the order.");
            TempData["ErrorMessage"] = "An error occurred while processing your order. Please try again later.";
            return RedirectToAction("Error", "Home");
        }
    }
}