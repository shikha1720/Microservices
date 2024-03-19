using System.ComponentModel.DataAnnotations;

namespace CartAPI.Models
{
    public class CartModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
