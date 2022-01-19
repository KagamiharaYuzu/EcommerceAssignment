using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreClassLibrary;
using KLH60Services.Models.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace KLH60Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _ls;

        public LoginController(ILoginService ls) => _ls = ls;

        [HttpGet("email/{email}")]
        public async Task<ActionResult> Login(string email)
        {
            try
            {
                (int code, string msg) = await _ls.IsManagerLogin(email);
                return code switch
                {
                    0 => Ok(msg),
                    -1 => Unauthorized($"Sorry you're {msg} to login to this part of the site. Contact admin for more info."),
                    -2 => NotFound($"{msg}"),
                    _ => throw new HttpResponseException(HttpStatusCode.InternalServerError, "Sorry, an issue happened on our part. Please try again later.")
                };
            }
            catch (ArgumentNullException ane)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, ane.Message);
            }
        }
    }
}