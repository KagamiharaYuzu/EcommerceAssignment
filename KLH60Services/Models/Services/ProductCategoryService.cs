using StoreClassLibrary;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KLH60Services.Models.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly StoreServiceContext _db;

        public ProductCategoryService(StoreServiceContext db) => _db = db;

        public async Task CreateProductCategory(ProductCategory prodCat)
        {
            if (prodCat is null)
                throw new ArgumentNullException(nameof(prodCat), "invalid product category");
            await _db.ProductCategories.AddAsync(prodCat);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteProductCategory(int id)
        {
            if (!await ProductCategoryExists(id))
                throw new ArgumentException("Please select a valid category", nameof(id));
            _db.ProductCategories.Remove(await _db.ProductCategories.FirstAsync(prodCat => prodCat.CategoryId == id));
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategories() => await _db.ProductCategories.AsNoTracking().ToListAsync();

        public async Task<ProductCategory> GetCategory(int id) => !await ProductCategoryExists(id) ? throw new ArgumentException("Please select a valid category", nameof(id)) : await _db.ProductCategories.AsNoTracking().FirstAsync(cat => cat.CategoryId == id);

        public async Task UpdateProductCategory(ProductCategory prodCat)
        {
            if (prodCat is null)
                throw new ArgumentNullException(nameof(prodCat), "Please select a valid category");
            _db.ProductCategories.Update(prodCat);
            await _db.SaveChangesAsync();
        }

        private async Task<bool> ProductCategoryExists(int id) => await _db.ProductCategories.AsNoTracking().AnyAsync(e => e.CategoryId == id);
    }
}