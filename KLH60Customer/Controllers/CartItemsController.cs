using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreClassLibrary;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KLH60Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartItemsController : Controller
    {
        // POST: CartItems/Edit/5
        [HttpPost, ActionName("UpdateItem")]
        public async Task<IActionResult> UpdateItem(int id, CartItem cartItem, int Newquantity)
        {
            if (id != cartItem.CartItemId) return NotFound();

            if (ModelState.IsValid)
            {
                HttpResponseMessage res = null;
                try
                {
                    cartItem.Quantity = Newquantity;
                    res = await cartItem.UpdateCartItemQuantity();
                }
                catch (Exception e)
                {
                    GoToGenericError(e);
                }
                switch (res.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        TempData["ok"] = $"Cart item quantity has been updated.";
                        break;

                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.Conflict:
                    case HttpStatusCode.MethodNotAllowed:
                        string err = await res.Content.ReadAsStringAsync();
                        TempData["Error"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                        break;
                }
            }
            else TempData["modelError"] = $"Something was wrong with the form. {ModelState.ErrorCount} errors were found. Please fix the issues and try again.";
            return RedirectToAction("ShoppingCart", "ShoppingCarts");
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage res = null;
            try
            {
                res = await new CartItem() { CartItemId = id }.DeleteCartItem();
            }
            catch (Exception e)
            {
                GoToGenericError(e);
            }
            switch (res.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    TempData["ok"] = $"Cart item has been deleted.";
                    break;

                case HttpStatusCode.BadRequest:
                case HttpStatusCode.InternalServerError:
                    string err = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                    break;
            }
            return RedirectToAction("ShoppingCart", "ShoppingCarts");
        }

        private IActionResult GoToGenericError(Exception e)
        {
            ViewData["err"] = e.Message;
            return View("GenericError");
        }
    }
}