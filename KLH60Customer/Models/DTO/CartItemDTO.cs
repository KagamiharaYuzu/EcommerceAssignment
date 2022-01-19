namespace KLH60Customer.Models.DTO
{
    public class CartItemDTO
    {
        public string Description { get; set; }
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public CartItemDTO()
        { }
    }
}