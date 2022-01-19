using StoreClassLibrary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KLH60Services.Models.Services
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategory>> GetAllCategories();

        Task<ProductCategory> GetCategory(int id);

        Task UpdateProductCategory(ProductCategory prodCat);

        Task CreateProductCategory(ProductCategory prodCat);

        Task DeleteProductCategory(int id);
    }
}