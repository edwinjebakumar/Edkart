using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class APIResponse
    {
        public APIResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultApiResponseMessage(statusCode);
        }


        public int StatusCode { get; set; }
        public string Message { get; set; }

        private string GetDefaultApiResponseMessage(int statusCode)
        {
            string msg = statusCode switch
            {
                400 => "You've made a Bad Request_400",
                401 => "You are not authorized_401",
                404 => "No resource found_404",
                500 => "Some internal exception_500",
                _ => null
            };

            return msg;
        }
    }
}