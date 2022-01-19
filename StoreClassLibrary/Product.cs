using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

#nullable disable

namespace StoreClassLibrary
{
    [DataContract(Name = "products")]
    public partial class Product
    {
        private static readonly string ProductApi = "http://localhost:42322/api/products";

        [DataMember(Name = "productId")]
        public int ProductId { get; set; }

        [Display(Name = "Category")]
        [DataMember(Name = "prodCatId")]
        public int ProdCatId { get; set; }

        [Required]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} Characters long")]
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [Required]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} Characters long")]
        [DataMember(Name = "manufacturer")]
        public string Manufacturer { get; set; }

        [Required]
        [DataMember(Name = "stock")]
        public int? Stock { get; set; }

        private decimal _buyPrice;
        private decimal _sellPrice;

        [Required]
        [Display(Name = "Buy Price")]
        [Column(TypeName = "decimal(8,2)")]
        [DataMember(Name = "buyPrice")]
        public decimal? BuyPrice
        {
            get { return _buyPrice; }
            set { _buyPrice = decimal.Round(value.Value, 2, MidpointRounding.AwayFromZero); }
        }

        [Required]
        [Display(Name = "Sell Price")]
        [Column(TypeName = "decimal(8,2)")]
        [DataMember(Name = "sellPrice")]
        public decimal? SellPrice
        {
            get { return _sellPrice; }
            set { _sellPrice = decimal.Round(value.Value, 2, MidpointRounding.AwayFromZero); }
        }

        public virtual ProductCategory ProdCat { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Product()
        {
            CartItems = new HashSet<CartItem>();
            OrderItems = new HashSet<OrderItem>();
        }

        public Product(int productId, int prodCatId, string description, string manufacturer, int? stock, decimal? buyPrice, decimal? sellPrice)
        {
            ProductId = productId;
            ProdCatId = prodCatId;
            Description = description;
            Manufacturer = manufacturer;
            Stock = stock;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
        }

        public async Task<HttpResponseMessage> CreateProduct()
        {
            VerifyStock();
            VerifySellPrice();
            VerifyBuyPrice();
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PostAsync(ProductApi, HttpContent);
        }

        public async Task<HttpResponseMessage> DeleteProduct(int id) => await new HttpClient().DeleteAsync(ProductApi + $"/{id}");

        public async Task<HttpResponseMessage> UpdateProduct()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PutAsync(ProductApi + $"/{ProductId}", HttpContent);
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync($"{ProductApi}/all");
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Product>));
            return serializer.ReadObject(await streamTask) as IEnumerable<Product>;
        }

        public async Task<Product> GetProduct(int prodId)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync(ProductApi + $"/{prodId}");
            var serializer = new DataContractJsonSerializer(typeof(Product));
            return serializer.ReadObject(await streamTask) as Product;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(int catId)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync($"{ProductApi}/{catId}/productcategories");
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Product>));
            return serializer.ReadObject(await streamTask) as IEnumerable<Product>;
        }

        public async Task<IEnumerable<ProductsPerCategory>> GetAllProductsByCategory()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync($"{ProductApi}/display");
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<ProductsPerCategory>));
            return serializer.ReadObject(await streamTask) as IEnumerable<ProductsPerCategory>;
        }

        public async Task<IEnumerable<Product>> SearchProduct(string term)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync($"{ProductApi}?term={term}");
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Product>));
            return serializer.ReadObject(await streamTask) as IEnumerable<Product>;
        }

        public async Task<HttpResponseMessage> UpdateStock()
        {
            Stock += (await GetProduct(ProductId)).Stock;
            VerifyStock();
            return await UpdateProduct();
        }

        public async Task<HttpResponseMessage> UpdateBuyPrice()
        {
            VerifyBuyPrice();
            return await UpdateProduct();
        }

        public async Task<HttpResponseMessage> UpdateSellPrice()
        {
            VerifySellPrice();
            return await UpdateProduct();
        }

        public async Task<HttpResponseMessage> UpdatePrices()
        {
            VerifyBuyPrice();
            VerifySellPrice();
            return await UpdateProduct();
        }

        public async Task<HttpResponseMessage> UpdateEntireProduct()
        {
            VerifyStock();
            VerifyBuyPrice();
            VerifySellPrice();
            return await UpdateProduct();
        }

        public void VerifyStock()
        {
            if (Stock is not int)
                throw new ArithmeticException("The updated stock must be a number");
            if (Stock < 0)
                throw new NegativeStockException("The stock cannot be 0");
        }

        private void VerifySellPrice()
        {
            if (SellPrice is not decimal)
                throw new ArithmeticException("The updated sell price must be a number");
            if (SellPrice < 0)
                throw new ArgumentException("The new sell price cannot be lower than 0");
            if (SellPrice < BuyPrice)
                throw new SellPriceTooLowException("The sell price cannot be lower than what we bought it for. We're here to make money not loose money.");
        }

        private void VerifyBuyPrice()
        {
            if (BuyPrice is not decimal)
                throw new ArithmeticException("The updated buy price must be a number");
            if (BuyPrice < 0)
                throw new ArgumentException("The new buy price cannot be lower than 0");
        }
    }
}