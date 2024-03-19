using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Design { get; set; }
    }
}
