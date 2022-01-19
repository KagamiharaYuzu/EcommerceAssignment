using StoreClassLibrary;
using KLH60Services.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace KLH60Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoryService _pcs;

        public ProductCategoriesController(IProductCategoryService pc) => _pcs = pc;

        // GET: api/ProductCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
            try
            {
                return Ok(await _pcs.GetAllCategories());
            }
            catch (ArgumentNullException ae)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ae.Message);
            }
        }

        // GET: api/ProductCategories/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
        //{
        //    var productCategory = await _pcs.ProductCategories.FindAsync(id);

        //    if (productCategory == null)
        //    {
        //        return NotFound();
        //    }

        //    return productCategory;
        //}

        // PUT: api/ProductCategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductCategory(int id, ProductCategory productCategory)
        {
            if (id != productCategory.CategoryId) return BadRequest();
            try
            {
                await _pcs.UpdateProductCategory(productCategory);
                return NoContent();
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (ArgumentException ae)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ae.Message);
            }
        }

        // POST: api/ProductCategories
        [HttpPost]
        public async Task<ActionResult<ProductCategory>> PostProductCategory(ProductCategory productCategory)
        {
            try
            {
                await _pcs.CreateProductCategory(productCategory);
                return CreatedAtAction("GetProductCategory", new { id = productCategory.CategoryId }, productCategory);
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (ArgumentNullException ane)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, ane.Message);
            }
        }

        // DELETE: api/ProductCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            try
            {
                await _pcs.DeleteProductCategory(id);
                return NoContent();
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (ArgumentException ae)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ae.Message);
            }
        }
    }
}