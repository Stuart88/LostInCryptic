using System;
using System.Collections.Generic;
using System.Text;

namespace LostInCryptic.Shared.Models
{
    public class BasicHttpResponse<T>
    {
        public BasicHttpResponse(){}
        public BasicHttpResponse(T data){ this.Data = data; }
        public BasicHttpResponse(string errorMessage) { this.Ok = false; this.Message = errorMessage; }

        public bool Ok { get; set; } = true;

        public string Message { get; set; } = "";

        public T Data { get; set; } = default(T);
    }
}
