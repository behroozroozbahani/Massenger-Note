using Microsoft.AspNetCore.Http;

namespace PortalCore.WebApi.Helpers
{
    public class ApiResult<T>
    {
        public ApiResult(bool isSuccess, int statusCode)
        {
            IsSuccess = isSuccess;
            Status = statusCode;
        }

        public ApiResult(bool isSuccess, int statusCode, string[] message)
            : this(isSuccess, statusCode)
        {
            Message = message;
        }

        public ApiResult(bool isSuccess, int statusCode, T data)
            : this(isSuccess, statusCode)
        {
            Data = data;
        }

        public ApiResult(bool isSuccess, int statusCode, string[] message, T data)
            : this(isSuccess, statusCode, message)
        {
            Data = data;
        }

        public bool IsSuccess { get; private set; }

        public int Status { get; private set; }

        public string[]? Message { get; private set; }

        public T? Data { get; private set; }
    }

    public class OkApiResult<T> : ApiResult<T>
    {
        private const int DefaultStatusCode = StatusCodes.Status200OK;

        public OkApiResult() :
            base(true, DefaultStatusCode)
        {

        }

        public OkApiResult(string[] message)
            : base(true, DefaultStatusCode, message)
        {

        }

        public OkApiResult(T data)
            : base(true, DefaultStatusCode, data)
        {

        }

        public OkApiResult(string[] message, T data)
            : base(true, DefaultStatusCode, message, data)
        {

        }
    }
}
