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
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _ci;
        private readonly IProductService _ps;

        public CartItemsController(ICartItemService cic, IProductService ps)
        {
            _ci = cic;
            _ps = ps;
        }

        // GET: api/CartItems/5 <- id of the cart to get the items
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<CartItemDisplay>>> GetCartItems(int id)
        {
            try
            {
                return Ok(await _ci.GetCartItems(id));
            }
            catch (ArgumentException ae)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ae.Message);
            }
        }

        // PUT: api/CartItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(int id, CartItem cartItem)
        {
            if (id != cartItem.CartItemId) return BadRequest();

            try
            {
                await _ci.UpdateCartItem(cartItem);
                return NoContent();
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

        // PUT: api/CartItems/quantity/5
        [HttpPut("quantity/{id}")]
        public async Task<IActionResult> PutCartItemQuantity(int id, CartItem cartItem)
        {
            if (id != cartItem.CartItemId) return BadRequest();
            CartItem oldCItem = new();
            try
            {
                oldCItem = await _ci.GetCartItem(id);
                if (oldCItem.Quantity < cartItem.Quantity)
                    await _ps.ReduceProductStock(oldCItem.ProductId, cartItem.Quantity - oldCItem.Quantity);
                else
                    await _ps.RestoreProductStock(oldCItem.ProductId, oldCItem.Quantity - cartItem.Quantity);
                await _ci.UpdateCartItem(cartItem);
                return NoContent();
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, e.Message);
            }
            catch (ArgumentNullException ane)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, ane.Message);
            }
            catch (NegativeStockException nse)
            {
                await _ci.UpdateCartItem(oldCItem);
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed, nse.Message);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        // POST: api/CartItems
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItem cartItem)
        {
            try
            {
                await _ci.CreateCartItem(cartItem);
                await _ps.ReduceProductStock(cartItem.ProductId, cartItem.Quantity);
                return CreatedAtAction("GetCartItem", new { id = cartItem.CartItemId }, cartItem);
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

        // DELETE: api/CartItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            try
            {
                CartItem cartItem = await _ci.GetCartItem(id);
                await _ci.DeleteCartItem(id);
                await _ps.RestoreProductStock(cartItem.ProductId, cartItem.Quantity);
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

        // DELETE: api/CartItems/cart/5 <- the id in this case is the cart id
        // in this case, the cust has placed an order and there is no need to restore the product stock
        [HttpDelete("cart/{id}")]
        public async Task<IActionResult> DeleteItemsInCart(int id)
        {
            try
            {
                await _ci.ClearItemsInCart(id);
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