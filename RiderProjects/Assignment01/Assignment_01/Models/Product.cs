using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Assignment01.Models
{
    
    public class Product
    {
        
        // nav property !
        public Category Category { get; set; }
        [Key]  
        public int ProductId { get; set; }
        
        [Required] [MinLength(5)] 
        public string ProductName { get; set; } 
        
        public int? CategoryId { get; set; }
        [Required]
        [Range(0,double.MaxValue)]
        public double Price { get; set; }
        
        [Required]
        [Range(0,int.MaxValue)]
        public int Quantity { get; set; }
        [Required] [Range(0,int.MaxValue)]
        public int LowStockThreshold { get; set; } 
        
        [Range(0,int.MaxValue)]
        public int QuantityToOrder { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
        
        
    }
}