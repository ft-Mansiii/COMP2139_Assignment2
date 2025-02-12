using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment01.Models;

public class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }
    [Range(0,int.MaxValue)]
    public int Quantity { get; set; }
    [Range(0, double.MaxValue)]
    public double Price { get; set; }
    [ForeignKey("OrderId")]
    public int OrderId { get; set; }
    [ForeignKey("ProductId")]
    public int ProductId { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
}