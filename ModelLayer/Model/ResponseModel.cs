using System;
namespace ModelLayer.Model
{
    public class ResponseModel<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
    }
}