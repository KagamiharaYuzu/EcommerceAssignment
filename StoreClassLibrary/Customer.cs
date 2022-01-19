using Microsoft.AspNetCore.Identity;
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
using ServiceReferenceCheckCreditCard;

namespace StoreClassLibrary
{
    [DataContract(Name = "customers")]
    public class Customer
    {
        private static readonly string CustomerApi = "http://localhost:42322/api/customers/";

        [DataMember(Name = "customerId")]
        public int CustomerId { get; set; }

        [Display(Name = "First Name")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Your {0} must be between {2} and {1} characters long")]
        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z\s\-\']*", ErrorMessage = "Your {0} may only have letters, appostrophe, dashes and spaces only")]
        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Your {0} must be between {2} and {1} characters long")]
        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z\s\-\']*", ErrorMessage = "Your {0} may only have letters, appostrophe, dashes and spaces only")]
        [DataMember(Name = "lastName")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Your {0} must be between {2} and {1} characters long")]
        [EmailAddress(ErrorMessage = "Please enter a proper Email Address")]
        [DataMember(Name = "email")]
        public string Email { get; set; }

        [StringLength(10)]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"\d{10}", ErrorMessage = "Please provide a valid {0} in the format ##########")]
        [DataMember(Name = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [StringLength(2)]
        [DataMember(Name = "province")]
        public string Province { get; set; }

        [Display(Name = "Credit Card")]
        [StringLength(16, MinimumLength = 12, ErrorMessage = "Your {0} must be between {2} and {1} characters long")]
        [RegularExpression(@"\d{12,16}", ErrorMessage = "Your credit card should only have numbers and be 12 to 16 characters long.")]
        [DataMember(Name = "creditCard")]
        public string CreditCard { get; set; }

        [ForeignKey("AspNetUser")]
        [DataMember(Name = "aspnetUserId")]
        public string AspnetUserId { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public Customer()
        {
        }

        public Customer(string firstName, string lastName, string email, string phoneNumber, string province, string creditCard)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Province = province;
            CreditCard = creditCard;
        }

        public Customer(int customerId, string firstName, string lastName, string email, string phoneNumber, string province, string creditCard)
        {
            CustomerId = customerId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Province = province;
            CreditCard = creditCard;
        }

        public Customer(int customerId, string firstName, string lastName, string email, string phoneNumber, string province, string creditCard, string aspnetUserId) : this(customerId, firstName, lastName, email, phoneNumber, province, creditCard) => AspnetUserId = aspnetUserId;

        public Customer(string firstName, string lastName, string email, string phoneNumber, string province, string creditCard, string aspnetUserId) : this(firstName, lastName, email, phoneNumber, province, creditCard)
        {
            AspnetUserId = aspnetUserId;
        }

        public Customer(int customerId, string firstName, string lastName, string email, string phoneNumber, string province, string creditCard, ShoppingCart shoppingCart, ICollection<Order> orders, string aspUserId) : this(customerId, firstName, lastName, email, phoneNumber, province, creditCard, aspUserId)
        {
            ShoppingCart = shoppingCart;
            Orders = orders;
        }

        public async Task<Customer> GetCustomer(int id)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync(CustomerApi + id);
            var serializer = new DataContractJsonSerializer(typeof(Customer));
            return serializer.ReadObject(await streamTask) as Customer;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync(CustomerApi);
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<Customer>));
            return serializer.ReadObject(await streamTask) as IEnumerable<Customer>;
        }

        public async Task<Customer> GetCustomerByEmail(string email)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync($"{CustomerApi}email/{email}");
            var serializer = new DataContractJsonSerializer(typeof(Customer));
            return serializer.ReadObject(await streamTask) as Customer;
        }

        public async Task<HttpResponseMessage> CreateCustomer()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PostAsync(CustomerApi, HttpContent);
        }

        public async Task<HttpResponseMessage> UpdateCustomer()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PutAsync(CustomerApi + CustomerId, HttpContent);
        }

        public async Task<HttpResponseMessage> DeleteCustomer() => await new HttpClient().DeleteAsync(CustomerApi + CustomerId);

        public async Task<int> IsCardValid()
        {
            BasicHttpBinding binding = new();
            EndpointAddress endpoint = new("http://csdev.cegep-heritage.qc.ca/cartService/checkCreditCard.asmx");
            var client = new CheckCreditCardSoapClient(binding, endpoint);
            int validationCode = await client.CreditCardCheckAsync(CreditCard);
            return validationCode;
        }

        public async Task<string> CheckCreditCard()
        {
            int ccCode = await IsCardValid();
            return ccCode switch
            {
                 0 => "",
                -1 => "Your credit card number doesn't meet the required length of 12 to 16 numbers long",
                -2 => "The credit card you entered isn't all numbers. Please re-enter",
                -3 => "The credit card you entered doesn't follow the rule of each set of 4 numbers' sum must be less than thirty.",
                -4 => "The product of the last 2 digits isn't a multiple of 2 for the credit card you entered.",
                -5 => "You do not have enough money in your credit card.",
                 _ => "Something went wrong while checking the credit card. Please try again later.",
            };
        }
    }
}