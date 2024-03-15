using Microsoft.AspNetCore.Mvc;

namespace LawSearch_API.Utils
{
    public class APIResult
    {
        public APIResult() { }

        public APIResult(int status, string message, string exception, object data)
        {
            Status = status;
            Message = message;
            Exception = exception;
            Data = data;
        }

        public int Status { set; get; }
        public string Message { set; get; }
        public string Exception { set; get; }
        public object Data { set; get; }

        public APIResult Success(object? data = null)
        {
            return new APIResult()
            {
                Status = 200,
                Message = "Ok",
                Data = data
            };
        }

        public APIResult MessageSuccess(string message, object? data = null)
        {
            return new APIResult()
            {
                Status = 200,
                Message = message,
                Data = data
            };
        }

    }
}
