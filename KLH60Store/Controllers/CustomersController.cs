using StoreClassLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KLH60Store.Controllers
{
    [Authorize(Roles = "Clerk,Manager")]
    public class CustomersController : Controller
    {
        // GET: Customers
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Customer> custs = await new Customer().GetCustomers();
                custs = custs.OrderBy(c => c.LastName).ThenBy(c => c.FirstName);
                return View(custs);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var customer = await new Customer().GetCustomer(id);
                if (customer == null) return NotFound();
                return View(customer);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var customer = await new Customer().GetCustomer(id);
                if (customer == null) return NotFound();
                return View(customer);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId) return NotFound();
            if (ModelState.IsValid)
            {
                HttpResponseMessage res;
                try
                {
                    res = await customer.UpdateCustomer();
                }
                catch (Exception e)
                {
                    return GoToGenericError(e);
                }
                switch (res.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        return RedirectToAction(nameof(Index));

                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.Conflict:
                    case HttpStatusCode.InternalServerError:
                        string err = await res.Content.ReadAsStringAsync();
                        TempData["Err"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                        return RedirectToAction(nameof(Edit), id);

                    default:
                        return View(nameof(Edit), id);
                }
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var customer = await new Customer().GetCustomer(id);
                if (customer == null) return NotFound();
                return View(customer);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage res;
            try
            {
                var cust = await new Customer().GetCustomer(id);
                res = await cust.DeleteCustomer();
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
            switch (res.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return RedirectToAction(nameof(Index));

                case HttpStatusCode.NotFound:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.InternalServerError:
                    string err = await res.Content.ReadAsStringAsync();
                    TempData["Err"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                    return RedirectToAction(nameof(Delete), id);

                default:
                    return View(nameof(Delete), id);
            }
        }

        private IActionResult GoToGenericError(Exception e)
        {
            ViewData["err"] = e.Message;
            return View("GenericError");
        }
    }
}