using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [DataContract(Name = "productCategories")]
    public partial class ProductCategory
    {
        private static readonly string ProdCatApi = "http://localhost:42322/api/productcategories/";

        [DataMember(Name = "categoryId")]
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} Characters long")]
        [DataMember(Name = "prodCat")]
        public string ProdCat { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public ProductCategory()
        {
            Products = new HashSet<Product>();
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategories()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync(ProdCatApi);
            var serializer = new DataContractJsonSerializer(typeof(IEnumerable<ProductCategory>));
            return serializer.ReadObject(await streamTask) as IEnumerable<ProductCategory>;
        }

        public async Task<ProductCategory> GetCategory(int id)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync(ProdCatApi + id);
            var serializer = new DataContractJsonSerializer(typeof(ProductCategory));
            return serializer.ReadObject(await streamTask) as ProductCategory;
        }

        public async Task<HttpResponseMessage> UpdateProductCategory()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PutAsync(ProdCatApi + CategoryId, HttpContent);
        }

        public async Task<HttpResponseMessage> CreateProductCategory()
        {
            var HttpContent = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return await new HttpClient().PostAsync(ProdCatApi, HttpContent);
        }

        public async Task<HttpResponseMessage> DeleteProductCategory(int catId) => await new HttpClient().DeleteAsync(ProdCatApi + catId);
    }
}