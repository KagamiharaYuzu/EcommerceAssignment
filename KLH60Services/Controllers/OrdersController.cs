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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _os;

        public OrdersController(IOrderService os) => _os = os;

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                return Ok(await _os.GetOrders());
            }
            catch (Exception e) when (e is ArgumentException || e is ArgumentNullException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, e.Message);
            }
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder(int id)
        {
            try
            {
                return Ok(await _os.GetOrder(id));
            }
            catch (Exception e) when (e is ArgumentException || e is ArgumentNullException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, e.Message);
            }
        }

        // GET: api/Orders/customers/5
        [HttpGet("customers/{id}")]//in this case id refers to the customer id
        public async Task<ActionResult<IEnumerable<Order>>> GetCustomerOrders(int id)
        {
            try
            {
                return Ok(await _os.GetCustomerOrders(id));
            }
            catch (Exception e) when (e is ArgumentException || e is ArgumentNullException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, e.Message);
            }
        }

        // GET: api/Orders/date/5
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByDate(DateTime date)
        {
            try
            {
                return Ok(await _os.GetOrdersByDate(date));
            }
            catch (Exception e) when (e is ArgumentException || e is ArgumentNullException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, e.Message);
            }
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderId) return BadRequest();
            try
            {
                await _os.UpdateOrder(order);
                return NoContent();
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (ArgumentNullException ane)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ane.Message);
            }
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            try
            {
                await _os.CreateOrder(order);
                return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
            }
            catch (Exception e) when (e is DbUpdateConcurrencyException || e is DbUpdateException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (ArgumentNullException ane)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ane.Message);
            }
        }
    }
}