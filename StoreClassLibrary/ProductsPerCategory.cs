using System.Runtime.Serialization;
#nullable disable
namespace StoreClassLibrary
{
    [DataContract(Name = "productsPerCategories")]
    public class ProductsPerCategory
    {
        [DataMember(Name = "product")]
        public Product Product { get; set; }

        [DataMember(Name = "categoryName")]
        public string CategoryName { get; set; }

        public ProductsPerCategory()
        {
        }

        public ProductsPerCategory(Product product, string categoryName)
        {
            Product = product;
            CategoryName = categoryName;
        }
    }
}