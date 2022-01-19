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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _prod;

        public ProductsController(IProductService prod) => _prod = prod;

        // GET: api/Products/display
        [HttpGet("display")]
        public async Task<ActionResult<IEnumerable<ProductsPerCategory>>> GetProducts()
        {
            try
            {
                IEnumerable<ProductsPerCategory> ppc = await _prod.GetAllProductsByCategory();
                return Ok(ppc);
            }
            catch (ArgumentNullException ae)
            {
                return NotFound(ae.Message);
            }
        }

        // GET: api/Products/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            try
            {
                return Ok(await _prod.GetAllProducts());
            }
            catch (ArgumentNullException ae)
            {
                return NotFound(ae.Message);
            }
        }

        // GET: api/Products/3/productcategories
        [HttpGet("{id}/productcategories")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProductsByCat(int id)
        {
            try
            {
                return Ok(await _prod.GetProductsByCategory(id));
            }
            catch (ArgumentNullException ae)
            {
                return NotFound(ae.Message);
            }
        }

        // GET: api/Products?term=term
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(string term)
        {
            try
            {
                return Ok(await _prod.Search(term));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                return Ok(await _prod.GetProduct(id));
            }
            catch (ArgumentNullException e)
            {
                return NotFound(e.Message);
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId) return BadRequest();
            try
            {
                await _prod.UpdateProduct(product);
                return NoContent();
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (ArgumentNullException ane)
            {
                return BadRequest(ane.Message);
            }
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                await _prod.CreateProduct(product);
                return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
            }
            catch (ArgumentNullException ane)
            {
                return BadRequest(ane.Message);
            }
            catch (DbUpdateConcurrencyException duce)
            {
                return Conflict(duce.Message);
            }
            catch (DbUpdateException due)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, due.Message);
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _prod.DeleteProduct(id);
                return NoContent();
            }
            catch (ArgumentException ae)
            {
                return NotFound(ae.Message);
            }
            catch (DbUpdateConcurrencyException duce)
            {
                return Conflict(duce.Message);
            }
            catch (DbUpdateException due)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, due.Message);
            }
        }
    }
}