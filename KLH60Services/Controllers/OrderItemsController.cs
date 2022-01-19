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
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _ois;

        public OrderItemsController(IOrderItemService ois) => _ois = ois;

        // GET: api/OrderItems/5 <- order id
        // get the order items for a specified order
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int id)
        {
            try
            {
                return Ok(await _ois.GetOrderItems(id));
            }
            catch (ArgumentNullException ane)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ane.Message);
            }
        }

        // PUT: api/OrderItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(int id, OrderItem orderItem)
        {
            if (id != orderItem.OrderItemId)
                return BadRequest();
            try
            {
                await _ois.UpdateOrderItem(orderItem);
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

        // POST: api/OrderItems
        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem)
        {
            try
            {
                await _ois.CreateOrderItem(orderItem);
                return CreatedAtAction("GetOrderItem", new { id = orderItem.OrderItemId }, orderItem);
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
    }
}