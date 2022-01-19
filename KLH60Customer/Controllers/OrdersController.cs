using StoreClassLibrary;
using KLH60Customer.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Web;

namespace KLH60Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrdersController : Controller
    {
        private readonly IMapper _map;

        public OrdersController(IMapper map) => _map = map;

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            try
            {
                int cust = HttpContext.Session.GetInt32("CustId").Value;
                return View(await new Order() { OrderCustId = cust }.GetCustomerOrders());
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // GET: Orders/Details(Summary)/5
        public async Task<IActionResult> Summary(int? id)
        {
            if (id == null) return NotFound();
            Order order;
            try
            {
                order = await new Order().GetOrder(id.Value);
                return View(order);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        public async Task<IActionResult> CheckCreditCard(int orderId)
        {
            HttpContext.Session.SetInt32("order", orderId);
            Customer cust = await new Customer().GetCustomer(HttpContext.Session.GetInt32("CustId").Value);
            if (!string.IsNullOrEmpty(cust.CreditCard))
            {
                string ccmsg = await cust.CheckCreditCard();
                ViewData["ccCheck"] = ccmsg;
            }
            else ViewData["ccCheck"] = "No credit card was found on file. Please enter one.";
            return View("_creditCardCheck", cust);
        }

        [HttpPost]
        public async Task<ActionResult> CheckCreditCard(string cc)
        {
            if (string.IsNullOrEmpty(cc))
            {
                ViewData["ccCheck"] = "Please enter a credit card to validate.";
                return View("_creditCardCheck", new Customer { CreditCard = cc });
            }
            string msg = await new Customer() { CreditCard = cc }.CheckCreditCard();
            if (msg != "")
            {
                ViewData["ccCheck"] = msg;
                return View("_creditCardCheck", new Customer { CreditCard = cc });
            }
            else
            {
                int ordId = HttpContext.Session.GetInt32("order").Value;
                HttpContext.Session.Remove("order");
                return RedirectToAction(nameof(PlaceOrder), new { orderId = ordId });
            }
        }

        // GET: Orders/PlaceOrder
        public async Task<IActionResult> PlaceOrder(int orderId)
        {
            try
            {
                Order ord = await new Order().GetOrder(orderId);
                ViewData["ord"] = ord;
                IEnumerable<OrderItem> ordItems = await new OrderItem() { OrderId = ord.OrderId }.GetOrderItems();
                IEnumerable<Product> allProds = await new Product().GetAllProducts();
                List<Product> prods = new();
                foreach (var item in ordItems)
                    prods.Add(allProds.First(prod => prod.ProductId == item.ProductId));
                //prods.Add(await new Product().GetProduct(item.ProductId));
                List<OrderItemDTO> ordItemsDTO = new();
                for (int i = 0; i < ordItems.Count(); i++)
                    ordItemsDTO.Add(_map.Map(prods.ElementAt(i), _map.Map<OrderItem, OrderItemDTO>(ordItems.ElementAt(i))));
                return View(ordItemsDTO);
            }
            catch (Exception e)
            {
                return GoToGenericError(e, "Unable to retreive order. Something wrong happened on our part. Please try again later. Sorry for the inconvenience.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            HttpResponseMessage res = await order.UpdateOrder();
            switch (res.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return View("Thanks");

                case HttpStatusCode.BadRequest:
                case HttpStatusCode.InternalServerError:
                    string err = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                    break;
            }
            return View();
        }

        // POST: Orders/Create
        [HttpPost, ActionName("Checkout")]
        public async Task<IActionResult> Checkout(int cartId, decimal totalNoTax)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage res;
                try
                {
                    decimal taxes = (decimal)await new Order().CalcTaxes(totalNoTax, HttpContext.Session.GetString("province"));
                    var ord = new Order() { OrderCustId = HttpContext.Session.GetInt32("CustId").Value, Taxes = taxes, Total = totalNoTax, DateCreated = DateTime.Now, DateFulfiled = DateTime.Parse("1970-02-02") };
                    res = await ord.CreateOrder();
                }
                catch (Exception e)
                {
                    return GoToGenericError(e);
                }
                Order createdOrd = null;
                switch (res.StatusCode)
                {
                    case HttpStatusCode.Created:
                        createdOrd = JsonConvert.DeserializeObject<Order>(await res.Content.ReadAsStringAsync());
                        TempData["ok"] = $"Order has been created!";
                        break;

                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.InternalServerError:
                        string err = await res.Content.ReadAsStringAsync();
                        TempData["Error"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                        return RedirectToAction("ShoppingCart", "ShoppingCarts");
                }
                try
                {
                    if (createdOrd is not null)
                    {
                        await TransferCartItemsToOrder(cartId, createdOrd.OrderId);
                        HttpContext.Session.SetInt32("CartId", 0);
                    }
                }
                catch (Exception e)
                {
                    return GoToGenericError(e, "something happened while ");
                }
                return RedirectToAction("PlaceOrder", new { orderId = createdOrd.OrderId });
            }
            return RedirectToAction("ShoppingCart");
        }

        private static async Task TransferCartItemsToOrder(int cartId, int orderId)
        {
            IEnumerable<CartItemDisplay> cartItems = await new CartItem().GetCartItems(cartId);
            IEnumerable<OrderItem> Oitems = from ci in cartItems select new OrderItem() { OrderId = orderId, ProductId = ci.Item.ProductId, Quantity = ci.Item.Quantity, Price = ci.Item.Price };
            for (int i = 0; i < cartItems.Count(); i++)
            {
                await Oitems.ElementAt(i).CreateOrderItem();
                await cartItems.ElementAt(i).Item.DeleteCartItem();
            }
            await new ShoppingCart().DeleteShoppingCart(cartId);
        }

        private IActionResult GoToGenericError(Exception e, string customErr = "Something happened on our end and we'll fix it asap. Please try again later")
        {
            ViewData["err"] = e.Message;
            ViewData["failed"] = customErr;
            return View("GenericError");
        }
    }
}