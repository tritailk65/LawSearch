using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LawSearch_Core.Models
{
    public class BadRequestException : Exception
    {
        public int StatusCode;

        public int? StatusCodeCustom;

        public BadRequestException(string message, int statusCode, int? statusCodeCustom = 0) : base(message)
        {
            StatusCode = statusCode;
            StatusCodeCustom = statusCodeCustom;
        }
    }
}
