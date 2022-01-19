using StoreClassLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KLH60Store.Controllers
{
    [Authorize(Roles = "Clerk,Manager")]
    public class ProductCategoriesController : Controller
    {
        // GET: ProductCategories
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await new ProductCategory().GetAllCategories());
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // GET: ProductCategories/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                return View(new ProductCategory().GetCategory(id.Value));
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // GET: ProductCategories/Create
        public IActionResult Create() => View();

        // POST: ProductCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage res;
                try
                {
                    res = await productCategory.CreateProductCategory();
                }
                catch (Exception e)
                {
                    return GoToGenericError(e);
                }
                switch (res.StatusCode)
                {
                    case HttpStatusCode.Created:
                        return RedirectToAction(nameof(Index));

                    case HttpStatusCode.Conflict:
                    case HttpStatusCode.InternalServerError:
                        string err = await res.Content.ReadAsStringAsync();
                        TempData["Err"] = err.Substring(err.IndexOf(":") + 1, err.IndexOf("\r\n") - err.IndexOf(":"));
                        return RedirectToAction(nameof(Create));

                    default:
                        return View(nameof(Create));
                }
            }

            return View(productCategory);
        }

        // GET: ProductCategories/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            try
            {
                var productCategory = new ProductCategory().GetCategory(id.Value);
                if (productCategory == null) return NotFound();
                return View(productCategory);
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // POST: ProductCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCategory productCategory)
        {
            if (id != productCategory.CategoryId)
                return NotFound();

            if (ModelState.IsValid)
            {
                HttpResponseMessage res;
                try
                {
                    res = await productCategory.UpdateProductCategory();
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
            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            try
            {
                return View(new ProductCategory().GetCategory(id.Value));
            }
            catch (Exception e)
            {
                return GoToGenericError(e);
            }
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage res;
            try
            {
                res = await new ProductCategory().DeleteProductCategory(id);
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