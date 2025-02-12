using Assignment01.Data;
using Microsoft.AspNetCore.Mvc;
using Assignment01.Models;
using Assignment01.Controllers;
using Microsoft.EntityFrameworkCore;

public class OrderController : Controller
{
    private readonly Assignment01DB _context;

    public OrderController(Assignment01DB context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var products = _context.Products
            .Where(p => p.QuantityToOrder > 0)
            .ToList();

        return View(products); 
    }



    [HttpPost]
    public IActionResult PlaceOrder(List<int> ProductIds, List<int> OrderQuantities, string deliveryAddress)
    {
        for (int i = 0; i < ProductIds.Count; i++)
        {
            if (OrderQuantities[i] > 0)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == ProductIds[i]);
                if (product != null)
                {
                    product.QuantityToOrder = OrderQuantities[i];
                }
            }
        }

        _context.SaveChanges(); 
        return RedirectToAction("Index", "Order"); 
    }


   
}