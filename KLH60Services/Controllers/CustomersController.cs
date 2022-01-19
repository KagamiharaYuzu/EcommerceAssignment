using StoreClassLibrary;
using KLH60Services.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace KLH60Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _cs;

        public CustomersController(ICustomerService cs) => _cs = cs;

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            try
            {
                return Ok(await _cs.GetCustomers());
            }
            catch (ArgumentNullException ane)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ane.Message);
            }
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            try
            {
                return Ok(await _cs.GetCustomer(id));
            }
            catch (ArgumentNullException ane)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ane.Message);
            }
        }

        // GET: api/Customers/email/cust@cust.com
        [HttpGet("email/{email}")]
        public async Task<ActionResult<Customer>> GetCustomerByEmail(string email)
        {
            try
            {
                Customer cust = await _cs.GetCustomerByEmail(email);
                return Ok(cust);
            }
            catch (Exception ane) when (ane is ArgumentNullException || ane is ArgumentException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, ane.Message);
            }
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId) return BadRequest();
            try
            {
                await _cs.UpdateCustomer(customer);
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

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            try
            {
                await _cs.CreateCustomer(customer);
                return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
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

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                await _cs.RemoveCustomer(id);
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