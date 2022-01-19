using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StoreClassLibrary
{
    [DataContract(Name = "productInCategories")]
    public class ProductInCategory
    {
        [DataMember(Name = "productCategoryName")]
        public string ProductCategoryName { get; set; }

        [DataMember(Name = "products")]
        public IEnumerable<Product> Products { get; set; }
    }
}