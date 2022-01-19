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
using System.ServiceModel;
using ServiceReferenceCalcTaxes;

#nullable disable

namespace StoreClassLibrary
{
    [DataContract(Name = "orders")]
    public class Order
    {
        private static readonly string OrderApi = "http://localhost:42322/api/orders/";

        [DataMember(Name = "orderId")]
        public int OrderId { get; set; }

        [ForeignKey("Customer")]
        [DataMember(Name = "orderCustId")]
        public int OrderCustId { get; set; }

        private string _dateCreated;
        private string _dateFulfiled;

        [DataMember(Name = "dateCreated")]
        [NotMapped]
        public string DateCreatedJSON
        {
            get => _dateCreated;
            set => _dateCreated = value;
        }

        [IgnoreDataMember]
        [Display(Name = "Date Created")]
        public DateTime DateCreated
        {
            get => DateTime.Parse(DateCreatedJSON);
            set => DateCreatedJSON = value.ToString();
        }

        [DataMember(Name = "dateFulfiled")]
        [NotMapped]
        public string DateFulfiledJSON
        {
            get => _dateFulfiled;
            set => _dateFulfiled = value;
        }

        [IgnoreDataMember]
        [Display(Name = "Date Fulfiled")]
        public DateTime? DateFulfiled
        {
            get => DateTime.Parse(DateFulfiledJSON);
            set => DateFulfiledJSON = value.ToString();
        }

        private decimal _total;
        private decimal _taxes;

        [Column(TypeName = "decimal(10, 2)")]
        [DataMember(Name = "total")]
        public decimal? Total
        {
            get { return _total; }
            set { _total = decimal.Round(value.Value, 2, MidpointRounding.AwayFromZero); }
        }

        [Column(TypeName = "decimal(8, 2)")]
        [DataMember(Name = "taxes")]
        public decimal? Taxes
        {
            get { return _taxes; }
            set { _taxes = decimal.Round(value.Value, 2, MidpointRounding.AwayFromZero); }
        }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual Customer Customer { get; set; }

        public Order() => OrderItems = new HashSet<OrderItem>();

        public Order(int orderCustId, DateTime? dateCreated)
        {
            OrderCustId = orderCustId;
            DateCreated = dateCreated.Value;
        }

        public Order(int orderId, int orderCustId, DateTime? dateCreated, DateTime? dateFulfiled, decimal? total, decimal? taxes)
        {
            OrderId = orderId;
            OrderCustId = orderCustId;
            DateCreated = dateCreated.Value;
            DateFulfiled = dateFulfiled.Value;
            Total = total;
            Taxes = taxes;
        }

        public async Task<HttpResponseMessage> CreateOrder()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PostAsync(OrderApi, HttpContent);
        }

        public async Task<HttpResponseMessage> UpdateOrder()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PutAsync(OrderApi + OrderId, HttpContent);
        }

        public async Task<HttpResponseMessage> DeleteOrder() => await new HttpClient().DeleteAsync(OrderApi + OrderId);

        public async Task<IEnumerable<Order>> GetCustomerOrders()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync($"{OrderApi}customers/{OrderCustId}");
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Order>));
            return serializer.ReadObject(await streamTask) as IEnumerable<Order>;
        }

        public async Task<Order> GetOrder(int orderId)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync($"{OrderApi}{orderId}");
            var serializer = new DataContractJsonSerializer(typeof(Order));
            return serializer.ReadObject(await streamTask) as Order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByDate(DateTime date)
        {
            string datechosen = date.Date.ToString("MM-dd-yyyy");
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync($"{OrderApi}date/{datechosen}");
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Order>));
            return serializer.ReadObject(await streamTask) as IEnumerable<Order>;
        }

        public async Task<double> CalcTaxes(decimal total, string province)
        {
            BasicHttpBinding binding = new();
            EndpointAddress endpoint = new("http://csdev.cegep-heritage.qc.ca/cartService/calculateTaxes.asmx");
            var client = new CalculateTaxesSoapClient(binding, endpoint);
            var taxes = await client.CalculateTaxAsync(decimal.ToDouble(total), province);
            return taxes;
        }
    }
}