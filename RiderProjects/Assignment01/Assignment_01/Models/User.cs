using System.ComponentModel.DataAnnotations;

namespace Assignment01.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    [Required] [MinLength(10)]
    public string UserName { get; set; }
    [Required] [EmailAddress]
    public string UserEmail { get; set; }
    
}