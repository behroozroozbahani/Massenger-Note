using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace PortalCore.WebApi.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        private readonly ILogger<HttpResponseExceptionFilter> _logger;

        public HttpResponseExceptionFilter(ILogger<HttpResponseExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is not null)
            {
                //var service = context.HttpContext.RequestServices.GetService<ILogger>();

                if (context.Exception is SecurityTokenExpiredException securityTokenExpiredException)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Unauthorized",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "token expired",
                    };

                    context.Result = new ObjectResult(problemDetails)
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                    };
                    context.ExceptionHandled = true;
                }
                //else if (context.Exception is BadRequestException badRequestException)
                //{
                //    var problemDetails = new ProblemDetails
                //    {
                //        Title = "درخواست معتبر نیست",
                //        Status = badRequestException.DefaultStatusCode,
                //        Detail = badRequestException.CustomMessage,
                //    };

                //    context.Result = new ObjectResult(problemDetails)
                //    {
                //        StatusCode = badRequestException.DefaultStatusCode,
                //    };
                //    context.ExceptionHandled = true;
                //}
                //else if (context.Exception is NotFoundException notFoundException)
                //{
                //    var problemDetails = new ProblemDetails
                //    {
                //        Title = "یافت نشد",
                //        Status = notFoundException.DefaultStatusCode,
                //        Detail = notFoundException.CustomMessage,
                //    };

                //    context.Result = new ObjectResult(problemDetails)
                //    {
                //        StatusCode = notFoundException.DefaultStatusCode,
                //    };
                //    context.ExceptionHandled = true;
                //}
                else if (context.Exception is DbUpdateConcurrencyException dbUpdateConcurrencyException)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "خطای ثبت اطلاعات در پایگاه داده",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = "رکورد مورد نظر با مشخصات قبلی دیگر در پایگاه داده وجود ندارد",
                    };

                    context.Result = new ObjectResult(problemDetails)
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                    };
                    context.ExceptionHandled = true;
                    _logger.LogError(dbUpdateConcurrencyException, "CustomHttpResponseException");
                }
                else if (context.Exception is DbUpdateException dbUpdateException)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "خطای ثبت اطلاعات در پایگاه داده",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = "اطلاعات وارد شده برای اعمال تغییرات معتبر نمیباشد",
                    };

                    context.Result = new ObjectResult(problemDetails)
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                    };
                    context.ExceptionHandled = true;
                    _logger.LogError(dbUpdateException, "CustomHttpResponseException");
                }
                else
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "خطای سرور",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = "خطایی در سرور رخ داده است",
                    };

                    context.Result = new ObjectResult(problemDetails)
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                    };
                    context.ExceptionHandled = true;
                    _logger.LogError(context.Exception, "CustomHttpResponseException");
                }
            }
        }

        public int Order { get; } = int.MaxValue - 10;
    }
}
