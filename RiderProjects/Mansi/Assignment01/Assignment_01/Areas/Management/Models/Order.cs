using System.ComponentModel.DataAnnotations;

namespace Assignment01.Areas.Management.Models;

public class Order
{
    [Key] public int OrderId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now.ToUniversalTime();

    [Range(0, double.MaxValue)] public double TotalPrice { get; set; }

    [Required] public string DeliveryAddress { get; set; }

    public ICollection<OrderItem>? OrderItems { get; set; }
}