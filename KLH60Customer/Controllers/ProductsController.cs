using StoreClassLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using KLH60Customer.Models.DTO;
using AutoMapper;
using System.Collections.Generic;

namespace KLH60Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ProductsController : Controller
    {

        private readonly IMapper _map;

        public ProductsController(IMapper map) => _map = map;


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

        public async Task<IActionResult> SearchProducts(string term)
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

        public IActionResult Edit() => View("Test");

        private IActionResult GoToGenericError(Exception e)
        {
            ViewData["err"] = e.Message;
            return View("GenericError");
        }
    }
}