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
    [DataContract(Name = "orderItem")]
    public class OrderItem
    {
        private static readonly string OrderItemApi = "http://localhost:42322/api/orderitems/";

        [DataMember(Name = "orderItemId")]
        public int OrderItemId { get; set; }

        [ForeignKey("Orders")]
        [DataMember(Name = "orderId")]
        public int OrderId { get; set; }

        [DataMember(Name = "productId")]
        public int ProductId { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        private decimal _price;

        [Column(TypeName = "decimal(8, 2)")]
        [DataMember(Name = "price")]
        public decimal? Price
        {
            get { return _price; }
            set { _price = decimal.Round(value.Value, 2, MidpointRounding.AwayFromZero); }
        }

        public virtual ICollection<Product> Products { get; set; }
        public virtual Order Orders { get; set; }

        public OrderItem() => Products = new HashSet<Product>();

        public OrderItem(int orderId, int productId, int quantity, decimal? price)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }

        public OrderItem(int orderItemId, int orderId, int productId, int quantity, decimal? price)
        {
            OrderItemId = orderItemId;
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }

        public OrderItem(int orderItemId, int orderId, int productId, int quantity, decimal? price, ICollection<Product> products, Order orders) : this(orderItemId, orderId, productId, quantity, price)
        {
            Products = products;
            Orders = orders;
        }

        public async Task CreateOrderItem()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            await new HttpClient().PostAsync(OrderItemApi, HttpContent);
        }

        public async Task UpdateOrderItem()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            await new HttpClient().PutAsync(OrderItemApi + OrderId, HttpContent);
        }

        public async Task DeleteOrderItem() => await new HttpClient().DeleteAsync(OrderItemApi + OrderId);

        public async Task<IEnumerable<OrderItem>> GetOrderItems()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync(OrderItemApi + OrderId);
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<OrderItem>));
            return serializer.ReadObject(await streamTask) as IEnumerable<OrderItem>;
        }
    }
}