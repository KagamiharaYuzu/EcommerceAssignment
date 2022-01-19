using StoreClassLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KLH60Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomersController : Controller
    {
        [HttpPost]
        public IActionResult Edit(int id, Customer cust) => View();
    }
}