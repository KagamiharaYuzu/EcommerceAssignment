using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreClassLibrary;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using KLH60Store.Models.DTO;

namespace KLH60Store.Controllers
{
    [Authorize(Roles = "Clerk,Manager")]
    public class ProductsController : Controller
    {
        private readonly UserManager<IdentityUser> _user;
        private readonly IMapper _map;

        public ProductsController(UserManager<IdentityUser> user, IMapper map) {
            _user = user;
            _map = map;
        }

        // GET: All Products
        public async Task<IActionResult> Index()
        {
            try
            {
                var allProds = await new Product().GetAllProducts();
                var categories = await new ProductCategory().GetAllCategories();
                allProds = allProds.OrderBy(prod => prod.ProdCatId).ThenBy(prod => prod.Description);
                categories = categories.OrderBy(cat => cat.CategoryId);
                List<ProductDTO> dtoList = new();
                foreach (var cat in categories)
                {
                    var inCat = allProds.Where(prod => prod.ProdCatId == cat.CategoryId);
                    foreach (var prod in inCat)
                        dtoList.Add(_map.Map(cat, _map.Map<Product, ProductDTO>(prod)));
                }
                return View(dtoList);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        public async Task<IActionResult> ProductsInCategory(int? catId)
        {
            try
            {
                return View(await new Product().GetProductsByCategory(catId.Value));
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        public async Task<IActionResult> Search(string term)
        {
            try
            {
                if (term.Trim() == "")
                    return View("Index");
                ViewData["search"] = term;
                var categories = await new ProductCategory().GetAllCategories();
                var prodsFound = await new Product().SearchProduct(term);
                prodsFound = prodsFound.OrderBy(prod => prod.ProdCatId).ThenBy(prod => prod.Description);
                categories = categories.OrderBy(cat => cat.CategoryId);
                List<ProductDTO> dtoList = new();
                foreach (var cat in categories)
                {
                    var inCat = prodsFound.Where(prod => prod.ProdCatId == cat.CategoryId);
                    foreach (var prod in inCat)
                        dtoList.Add(_map.Map(cat, _map.Map<Product, ProductDTO>(prod)));
                }
                return View(dtoList);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var product = await new Product().GetProduct(id.Value);
                return View(product);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage res = null;
                try
                {
                    res = await product.CreateProduct();
                    switch (res.StatusCode)
                    {
                        case HttpStatusCode.Created:
                            TempData["ok"] = $"Product {product.Description} has been created.";
                            return RedirectToAction(nameof(Index));

                        case HttpStatusCode.BadRequest:
                        case HttpStatusCode.Conflict:
                        case HttpStatusCode.InternalServerError:
                            string err = await res.Content.ReadAsStringAsync();
                            ViewData["Err"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                            ViewData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
                            return View(product);
                    }
                }
                catch (ArithmeticException ae)
                {
                    ViewData["Err"] = ae.Message;
                }
                catch (ArgumentException are)
                {
                    ViewData["Err"] = are.Message;
                }
                catch (NegativeStockException nse)
                {
                    ViewData["Err"] = nse.Message;
                }
                catch (SellPriceTooLowException sle)
                {
                    ViewData["Err"] = sle.Message;
                }
                catch (Exception e)
                {
                    GoToGenericError(e);
                }
                return View(product);
            }
            else return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string view = await _user.IsInRoleAsync(await _user.GetUserAsync(HttpContext.User), "Clerk") ? "EditClerk" : "Edit";
            if (id == null)
                return NotFound();
            try
            {
                var product = await new Product().GetProduct(id.Value);
                ViewData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
                return View(view, product);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // GET: Products/EditStock/5
        public async Task<IActionResult> EditStock(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var product = await new Product().GetProduct(id.Value);
                return View(product);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> EditPrices(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var product = await new Product().GetProduct(id.Value);
                return View(product);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> EditSellPrice(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var product = await new Product().GetProduct(id.Value);
                return View(product);
            }
            catch (Exception e) { return GoToGenericError(e); }
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> EditBuyPrice(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                var product = await new Product().GetProduct(id.Value);
                return View(product);
            }
            catch (Exception e) { return GoToGenericError(e); }
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();
            TempData["Err"] = "";
            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage res = await product.UpdateEntireProduct();
                    switch (res.StatusCode)
                    {
                        case HttpStatusCode.NoContent:
                            return RedirectToAction(nameof(Index));

                        case HttpStatusCode.BadRequest:
                        case HttpStatusCode.InternalServerError:
                            string err = await res.Content.ReadAsStringAsync();
                            TempData["Err"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                            TempData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
                            return RedirectToAction(nameof(Edit), product.ProductId);
                    }
                }
                catch (Exception e) when (e is ArithmeticException || e is NegativeStockException || e is SellPriceTooLowException || e is ArgumentException)
                {
                    TempData["Err"] = e.Message;
                    return RedirectToAction(nameof(Edit), product.ProductId);
                }
            }
            ViewData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
            return View(product);
        }

        // POST: Products/EditStock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStock(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();
            TempData["Err"] = "";
            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage res = await product.UpdateStock();
                    switch (res.StatusCode)
                    {
                        case HttpStatusCode.NoContent:
                            return RedirectToAction(nameof(Index));

                        case HttpStatusCode.BadRequest:
                        case HttpStatusCode.InternalServerError:
                            string err = await res.Content.ReadAsStringAsync();
                            TempData["Err"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                            TempData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
                            return RedirectToAction(nameof(EditStock), product.ProductId);

                        default:
                            return View();
                    }
                }
                catch (Exception e) when (e is ArithmeticException || e is NegativeStockException)
                {
                    TempData["Err"] = e.Message;
                    return RedirectToAction(nameof(EditStock), product.ProductId);
                }
            }
            else
            {
                ViewData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
                return View(product);
            }
        }

        [Authorize(Roles = "Manager")]
        // POST: Products/EditSellPrice/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSellPrice(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();
            TempData["Err"] = "";
            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage res = await product.UpdateSellPrice();
                    switch (res.StatusCode)
                    {
                        case HttpStatusCode.NoContent:
                            return RedirectToAction(nameof(Index));

                        case HttpStatusCode.BadRequest:
                        case HttpStatusCode.InternalServerError:
                            string err = await res.Content.ReadAsStringAsync();
                            TempData["Err"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                            TempData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
                            return RedirectToAction(nameof(EditSellPrice), product.ProductId);

                        default:
                            return View();
                    }
                }
                catch (Exception e) when (e is ArithmeticException || e is ArgumentException || e is SellPriceTooLowException)
                {
                    TempData["Err"] = e.Message;
                    return RedirectToAction(nameof(EditSellPrice), product.ProductId);
                }
            }
            else
            {
                ViewData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
                return View(product);
            }
        }

        [Authorize(Roles = "Manager")]
        // POST: Products/EditBuyPrice/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBuyPrice(int id, Product product)
        {
            if (id != product.ProductId) return NotFound();
            TempData["Err"] = "";
            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage res = await product.UpdateBuyPrice();
                    switch (res.StatusCode)
                    {
                        case HttpStatusCode.NoContent:
                            return RedirectToAction(nameof(Index));

                        case HttpStatusCode.BadRequest:
                        case HttpStatusCode.InternalServerError:
                            string err = await res.Content.ReadAsStringAsync();
                            TempData["Err"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                            TempData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
                            return RedirectToAction(nameof(EditBuyPrice), product.ProductId);

                        default:
                            return View();
                    }
                }
                catch (Exception e) when (e is ArithmeticException || e is ArgumentException)
                {
                    TempData["Err"] = e.Message;
                    return RedirectToAction(nameof(EditBuyPrice), product.ProductId);
                }
            }
            else
            {
                ViewData["ProdCatId"] = new SelectList(JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(HttpContext.Session.GetString("pc")), "CategoryId", "ProdCat", product.ProdCatId);
                return View(product);
            }
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var product = await new Product().GetProduct(id.Value);
                return View(product);
            }
            catch (Exception e) { return GoToGenericError(e); }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage res;
            try
            {
                res = await new Product().DeleteProduct(id);
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
                    return RedirectToAction(nameof(Index));
            }
        }

        private IActionResult GoToGenericError(Exception e)
        {
            ViewData["err"] = e.Message;
            return View("GenericError");
        }
    }
}