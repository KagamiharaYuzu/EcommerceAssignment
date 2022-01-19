namespace KLH60Customer.Models.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public int Stock { get; set; }
        public decimal SellPrice { get; set; }

        public ProductDTO()
        { }
    }
}