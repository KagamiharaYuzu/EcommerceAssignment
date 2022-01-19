using StoreClassLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace KLH60Customer.Controllers
{
    [Authorize(Roles = "Customer")]
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
                ViewData["err"] = e.Message;
                return View("GenericError");
            }
        }
    }
}