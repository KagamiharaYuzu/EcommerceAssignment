using System;
using System.Net;

namespace StoreClassLibrary
{
    public class HttpResponseException : Exception
    {
        public HttpStatusCode Status { get; set; }

        public HttpResponseException(HttpStatusCode status, string msg) : base(msg) => Status = status;
    }
}