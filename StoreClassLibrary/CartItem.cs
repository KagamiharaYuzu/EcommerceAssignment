using System;
using System.Collections.Generic;
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
    [DataContract(Name = "cartItems")]
    public class CartItem
    {
        private static readonly string CartItemApi = "http://localhost:42322/api/cartitems/";

        [DataMember(Name = "cartItemId")]
        public int CartItemId { get; set; }

        [ForeignKey("ShoppingCart")]
        [DataMember(Name = "cartId")]
        public int CartId { get; set; }

        [DataMember(Name = "productId")]
        public int ProductId { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        private decimal _price;

        [DataMember(Name = "price")]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price
        {
            get { return _price; }
            set { _price = decimal.Round(value, 2, MidpointRounding.AwayFromZero); }
        }

        public virtual Product Products { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }

        public CartItem()
        { }

        public CartItem(int cartId, int productId, decimal price, int quantity = 1)
        {
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }

        public CartItem(int cartItemId, int cartId, int productId, decimal price, int quantity = 1)
        {
            CartItemId = cartItemId;
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }

        public async Task<HttpResponseMessage> CreateCartItem()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PostAsync(CartItemApi, HttpContent);
        }

        public async Task<HttpResponseMessage> DeleteCartItem() => await new HttpClient().DeleteAsync(CartItemApi + CartItemId);

        public async Task<HttpResponseMessage> UpdateCartItem()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PutAsync(CartItemApi + CartItemId, HttpContent);
        }

        public async Task<HttpResponseMessage> UpdateCartItemQuantity()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PutAsync($"{CartItemApi}quantity/{CartItemId}", HttpContent);
        }

        public async Task<IEnumerable<CartItemDisplay>> GetCartItems(int id)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync(CartItemApi + id);
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<CartItemDisplay>));
            return serializer.ReadObject(await streamTask) as IEnumerable<CartItemDisplay>;
        }
    }
}