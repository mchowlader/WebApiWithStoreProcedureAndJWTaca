using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebApiWithStoreProc.Api.Model
{
    public class PayloadResponse<TEntity> where TEntity : class
    {
        private readonly IHttpContextAccessor _httpContext;

        public PayloadResponse()
        {
            _httpContext = new HttpContextAccessor();
            this.RequestURL = _httpContext.HttpContext != null ? 
                $"{_httpContext.HttpContext.Request.Scheme}: //{_httpContext.HttpContext.Request.Host}{_httpContext.HttpContext.Request.PathBase}{_httpContext.HttpContext.Request.Path}" : " ";
        }
        public string RequestURL { get; set; }
        public bool Success { get; set; }
        public string ResponseTime { get; set; }
        public string RequestTime { get; set; }
        public List<string> Message { get; set; }
        public TEntity Payload { get; set; }
        public string PayloadType { get; set; }

    }
}
