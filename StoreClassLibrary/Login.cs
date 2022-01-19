using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StoreClassLibrary
{
    public class Login
    {
        private static readonly string LoginApi = "http://localhost:42322/api/Login/";

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public async Task<HttpResponseMessage> LogIntoSystem()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetAsync($"{LoginApi}email/{Email}");
        }
    }
}