using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmplBank.Domain.Exception;
using System.Net;

namespace SmplBank.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            if (typeof(DomainException).IsAssignableFrom(context.Exception.GetType()))
            {
                var domainException = context.Exception as DomainException;

                int statusCode;
                string errorDetail;

                switch (domainException.DomainExceptionType)
                {
                    case DomainExceptionType.Validation:
                    case DomainExceptionType.Duplication:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        errorDetail = domainException.Message;
                        break;
                    case DomainExceptionType.NotFound:
                        statusCode = (int)HttpStatusCode.NotFound;
                        errorDetail = domainException.Message;
                        break;
                    case DomainExceptionType.InvalidOperation:
                        statusCode = (int)HttpStatusCode.Conflict;
                        errorDetail = domainException.Message;
                        break;
                    default:
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        errorDetail = "Unexpected error just happens. Please try again or contact administrator.";
                        break;
                }

                var problemDetails = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = statusCode,
                    Detail = errorDetail
                };

                problemDetails.Errors.Add("DomainValidations", new string[] { domainException.Message.ToString() });

                context.Result = new ObjectResult(problemDetails) { StatusCode = statusCode };
                context.HttpContext.Response.StatusCode = statusCode;
            }
            else
            {
                var json = new JsonErrorResponse
                {
                    Messages = new[] { "An error ocurred." }
                };

                if (env.IsDevelopment())
                {
                    json.DeveloperMessage = $@"{context.Exception.Message}
{context.Exception.StackTrace}";
                }

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            context.ExceptionHandled = true;
        }

        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMessage { get; set; }
        }

        private class InternalServerErrorObjectResult : ObjectResult
        {
            public InternalServerErrorObjectResult(object error)
                : base(error)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}
