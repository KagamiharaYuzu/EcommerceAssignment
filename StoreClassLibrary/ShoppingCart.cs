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
    [DataContract(Name = "shoppingCarts")]
    public class ShoppingCart
    {
        private static readonly string ShoppingCartApi = "http://localhost:42322/api/shoppingcarts/";

        [Key]
        [DataMember(Name = "cartId")]
        public int CartId { get; set; }

        [ForeignKey("Customer")]
        [DataMember(Name = "cartCustId")]
        public int CartCustId { get; set; }

        [DataMember(Name = "dateCreated")]
        [NotMapped]
        public string DateCreatedJSON { get; set; }

        [IgnoreDataMember]
        [Display(Name = "Date Created")]
        public DateTime DateCreated
        {
            get => DateTime.Parse(DateCreatedJSON);
            set => DateCreatedJSON = value.ToString();
        }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }

        public ShoppingCart() => CartItems = new HashSet<CartItem>();

        public ShoppingCart(int cartCustId, DateTime dateCreated)
        {
            CartCustId = cartCustId;
            DateCreated = dateCreated;
        }

        public ShoppingCart(int cartId, int cartCustId, DateTime dateCreated)
        {
            CartId = cartId;
            CartCustId = cartCustId;
            DateCreated = dateCreated;
        }

        public async Task<ShoppingCart> GetCustomerShoppingCart(int custId)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync($"{ShoppingCartApi}{custId}");
            var serializer = new DataContractJsonSerializer(typeof(ShoppingCart));
            return serializer.ReadObject(await streamTask) as ShoppingCart;
        }

        public async Task<HttpResponseMessage> CreateShoppingCart()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PostAsync(ShoppingCartApi, HttpContent);
        }

        public async Task<HttpResponseMessage> DeleteShoppingCart(int id) => await new HttpClient().DeleteAsync(ShoppingCartApi + id);

        public async Task<HttpResponseMessage> UpdateShoppingCart()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PutAsync(ShoppingCartApi + CartId, HttpContent);
        }
    }
}