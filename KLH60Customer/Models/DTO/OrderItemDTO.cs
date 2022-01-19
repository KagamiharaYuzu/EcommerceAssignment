namespace KLH60Customer.Models.DTO
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public OrderItemDTO()
        { }
    }
}