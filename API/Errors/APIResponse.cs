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
                400 => "You've made a Bad Request",
                401 => "You are not authorized",
                404 => "No resource found",
                500 => "Some internal exception",
                _ => null
            };

            return msg;
        }
    }
}