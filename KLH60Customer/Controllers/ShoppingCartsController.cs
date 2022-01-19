using StoreClassLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KLH60Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ShoppingCartsController : Controller
    {
        // GET: ShoppingCarts
        public async Task<IActionResult> ShoppingCart()
        {
            IEnumerable<CartItemDisplay> CItems = new List<CartItemDisplay>();
            ViewData["noItems"] = "";
            try
            {
                int cartId = HttpContext.Session.GetInt32("CartId").Value;
                ViewData["cart"] = cartId;
                if (cartId > 0)
                {
                    CItems = await new CartItem().GetCartItems(cartId);
                    decimal NoTaxTotal = 0;
                    if (CItems.Any())
                    {
                        foreach (var item in CItems)
                            NoTaxTotal += item.Item.Price * item.Item.Quantity;
                        ViewData["total"] = NoTaxTotal;
                    }
                    else ViewData["noItems"] = "There are no items in your shopping cart.";
                    return View(CItems);
                }
                else
                {
                    ViewData["noItems"] = "There are no items in your shopping cart.";
                    return View(CItems);
                }
            }
            catch (Exception)
            {
                ViewData["noItems"] = "There are no items in your shopping cart.";
                return View(CItems);
            }
        }

        //POST: shoppingcarts/add/5 <- product id of the item being added to cart
        [HttpGet("add/{id}")]
        public async Task<IActionResult> AddToCart(int id)
        {
            int cartId = HttpContext.Session.GetInt32("CartId").Value;
            if (cartId == 0)
            {
                ShoppingCart cart = new(HttpContext.Session.GetInt32("CustId").Value, DateTime.Now.Date);
                HttpResponseMessage res = await cart.CreateShoppingCart();
                switch (res.StatusCode)
                {
                    case HttpStatusCode.Created:
                        var newCart = JsonConvert.DeserializeObject<ShoppingCart>(await res.Content.ReadAsStringAsync());
                        cartId = newCart.CartId;
                        HttpContext.Session.SetInt32("CartId", cartId);
                        break;

                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.Conflict:
                    case HttpStatusCode.InternalServerError:
                        string err = await res.Content.ReadAsStringAsync();
                        TempData["Error"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                        return RedirectToAction("Index", "Products");
                }
            }
            Product prodToAdd = await new Product().GetProduct(id);
            CartItem citem = new(cartId, id, prodToAdd.SellPrice.Value);
            HttpResponseMessage resCItem = await citem.CreateCartItem();
            switch (resCItem.StatusCode)
            {
                case HttpStatusCode.Created:
                    TempData["ok"] = $"{prodToAdd.Description} added to your cart.";
                    break;

                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.InternalServerError:
                    string err = await resCItem.Content.ReadAsStringAsync();
                    TempData["Error"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                    break;
            }
            return RedirectToAction("Index", "Products");
        }

        private IActionResult GoToGenericError(Exception e)
        {
            ViewData["err"] = e.Message;
            return View("GenericError");
        }
    }
}