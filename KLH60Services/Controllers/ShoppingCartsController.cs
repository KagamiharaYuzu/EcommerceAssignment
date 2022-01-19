using KLH60Services.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreClassLibrary;
using System;
using System.Net;
using System.Threading.Tasks;

namespace KLH60Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : ControllerBase
    {
        private readonly IShoppingCartService _ss;

        public ShoppingCartsController(IShoppingCartService ss) => _ss = ss;

        // GET: api/ShoppingCarts/5
        [HttpGet("{id}")]//id is for the customer id
        public async Task<ActionResult<ShoppingCart>> GetShoppingCart(int id)
        {
            try
            {
                return Ok(await _ss.GetCustomerShoppingCart(id));
            }
            catch (Exception e) when (e is ArgumentException || e is ArgumentNullException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, e.Message);
            }
        }

        // PUT: api/ShoppingCarts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShoppingCart(int id, ShoppingCart shoppingCart)
        {
            if (id != shoppingCart.CartId) return BadRequest();
            try
            {
                await _ss.UpdateShoppingCart(shoppingCart);
                return NoContent();
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (ArgumentNullException ae)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ae.Message);
            }
        }

        // POST: api/ShoppingCarts
        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> PostShoppingCart(ShoppingCart shoppingCart)
        {
            try
            {
                await _ss.CreateShoppingCart(shoppingCart);
                return CreatedAtAction("GetShoppingCart", new { id = shoppingCart.CartId }, shoppingCart);
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

        // DELETE: api/ShoppingCarts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingCart(int id)
        {
            try
            {
                await _ss.DeleteShoppingCart(id);
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