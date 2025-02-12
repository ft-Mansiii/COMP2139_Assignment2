using System.ComponentModel.DataAnnotations;

namespace Assignment01.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, ErrorMessage = "Category Name can't be longer than 100 characters")]
        [MinLength(5, ErrorMessage = "Category Name should have at least 5 characters")]
        public string CategoryName { get; set; }

        [StringLength(500, ErrorMessage = "Category Description can't be longer than 500 characters")]
        public string? CategoryDescription { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}