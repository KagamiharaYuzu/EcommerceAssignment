using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreClassLibrary;

namespace KLH60Services.Models.Services
{
    public class ProductService : IProductService
    {
        private readonly StoreServiceContext _db;

        public ProductService(StoreServiceContext db) => _db = db;

        public async Task CreateProduct(Product prod)
        {
            if (prod is null)
                throw new ArgumentNullException(nameof(prod), "Cannot add an empty product.");
            await _db.Products.AddAsync(prod);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            if (await ProductExists(id))
            {
                _db.Products.Remove(await _db.Products.FindAsync(id));
                await _db.SaveChangesAsync();
            }
            else throw new ArgumentException("Product to delete was not found.", nameof(id));
        }

        public async Task ReduceProductStock(int prodId, int amount = 1)
        {
            if (await ProductExists(prodId))
            {
                Product prodToUpdate = _db.Products.First(prod => prod.ProductId == prodId);
                int reducedStock = prodToUpdate.Stock.Value - amount;
                if (reducedStock < 0)
                    throw new NegativeStockException("Stock amount cannot be lower than 0");
                prodToUpdate.Stock = reducedStock;
                await UpdateProduct(prodToUpdate);
            }
            else throw new ArgumentException("Product to reduce stock was not found.", nameof(prodId));
        }

        public async Task RestoreProductStock(int prodId, int amount = 1)
        {
            if (await ProductExists(prodId))
            {
                Product prodToUpdate = _db.Products.First(prod => prod.ProductId == prodId);
                int reducedStock = prodToUpdate.Stock.Value + amount;
                prodToUpdate.Stock = reducedStock;
                await UpdateProduct(prodToUpdate);
            }
            else throw new ArgumentException("Product to add stock was not found.", nameof(prodId));
        }

        public async Task<IEnumerable<Product>> GetAllProducts() => await _db.Products.OrderBy(prod => prod.Description).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<ProductsPerCategory>> GetAllProductsByCategory() => await _db.Products.Join(_db.ProductCategories, prod => prod.ProdCatId, cat => cat.CategoryId, (prod, cat) => new ProductsPerCategory() { Product = prod, CategoryName = cat.ProdCat }).OrderBy(cat => cat.CategoryName).ThenBy(prod => prod.Product.Description).AsNoTracking().ToListAsync();

        public async Task<Product> GetProduct(int prodId) => await _db.Products.AsNoTracking().FirstOrDefaultAsync(prod => prod.ProductId == prodId);

        public async Task<IEnumerable<Product>> GetProductsByCategory(int catId) => await _db.Products.Where(prod => prod.ProdCatId == catId).OrderBy(prod => prod.Description).AsNoTracking().ToListAsync();

        public async Task UpdateProduct(Product prod)
        {
            if (prod is null)
                throw new ArgumentNullException(nameof(prod), "Please enter a product to update.");
            prod.VerifyStock();
            _db.Products.Update(prod);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> Search(string searchTerm)
        {
            if (searchTerm is null)
                throw new ArgumentNullException(nameof(searchTerm), "We can't search for nothing. Please enter a term to search");
            return await _db.Products.Where(p => p.Description.ToLower().Contains(searchTerm.ToLower())).OrderBy(prod => prod.Description).AsNoTracking().ToListAsync();
        }

        private async Task<bool> ProductExists(int id) => await _db.Products.AsNoTracking().AnyAsync(e => e.ProductId == id);
    }
}