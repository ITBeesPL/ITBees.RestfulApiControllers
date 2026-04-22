using System;
using System.Linq;
using System.Threading.Tasks;
using ITBees.RestfulApiControllers.Authorization;
using ITBees.RestfulApiControllers.Exceptions;
using ITBees.RestfulApiControllers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace ITBees.RestfulApiControllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class RestfulControllerBase<T> : ControllerBase where T : class
    {
        private readonly ILogger<T> _logger;

        public RestfulControllerBase(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected IActionResult CreateBaseErrorResponse(string[] errorMessages, object inputModel)
        {
            return CreateBaseErrorResponse(String.Join(",", errorMessages), inputModel);
        }

        protected IActionResult CreateBaseErrorResponse(ModelStateDictionary errorMessage, object inputModel)
        {
            var errors = ModelState.Values.Select(x => x.Errors).ToList();
            var allErros = string.Join("; ", errors);

            _logger.LogError(allErros, inputModel);
            return new BadRequestModel(new string[] { allErros }, inputModel);
        }

        protected IActionResult CreateBaseErrorResponse(string errorMessage, object inputModel)
        {
            _logger.LogError(errorMessage, inputModel);
            return new BadRequestModel(new string[] { errorMessage }, inputModel);
        }

        protected BadRequestObjectResult CreateBaseErrorResponse(Exception e, object inputModel)
        {
            _logger.LogError(e.Message, inputModel);
            if (e.GetType() == typeof(AuthorizationException))
            {
                return new AuthorizationErrorModel(e.Message, inputModel);
            }

            return new BadRequestModel(new string[] { e.Message }, inputModel);
        }

        protected string GetClientIp()
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;

            // Check if the request is forwarded from a proxy
            if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedHeader = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrEmpty(forwardedHeader))
                {
                    return forwardedHeader.Split(',')[0].Trim();
                }
            }

            if (remoteIpAddress == null)
            {
                return "Unknown";
            }

            if (remoteIpAddress.IsIPv4MappedToIPv6)
            {
                return remoteIpAddress.MapToIPv4().ToString();
            }

            return remoteIpAddress.ToString();
        }

        protected async Task<IActionResult> ReturnOkResultAsync(Func<Task<object>> func, params object[] inputModel)
        {
            try
            {
                var result = await func();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, inputModel);
            }
        }

        protected async Task<IActionResult> ReturnOkResultAsync(Func<Task> action, params object[] inputModel)
        {
            try
            {
                await action();
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex, inputModel);

            }
        }


        /// <summary>
        /// Returns ok result, but if there will be an error, it will be logged. Authorization error will return 401, not found 404, bad request 500
        /// </summary>
        /// <param name="func"></param>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        protected IActionResult ReturnOkResult(Func<object> func, params object[] inputModel)
        {
            try
            {
                var result = func();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, inputModel);
            }
        }

        protected IActionResult ReturnOkResult(Action action, params object[] inputModel)
        {
            try
            {
                action();
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex, inputModel);
            }
        }

        private string GetRequestDetails()
        {
            var request = HttpContext.Request;
            var details = $"Request: {request.Method} {request.Scheme}://{request.Host}{request.Path}{request.QueryString}\n";
            details += $"ClientIP: {GetClientIp()}\n";
            details += $"Headers: {string.Join(", ", request.Headers.Select(h => $"{h.Key}={h.Value}"))}\n";

            if (request.Body.CanSeek && request.ContentLength > 0 && request.ContentLength < 10000)
            {
                try
                {
                    request.Body.Position = 0;
                    using var reader = new System.IO.StreamReader(request.Body, leaveOpen: true);
                    var body = reader.ReadToEndAsync().Result;
                    request.Body.Position = 0;
                    details += $"Body: {body}\n";
                }
                catch (Exception bodyEx)
                {
                    details += $"Body: <unavailable: {bodyEx.GetType().Name}: {bodyEx.Message}>\n";
                }
            }

            return details;
        }

        private IActionResult HandleException(Exception ex, object[] inputModel)
        {
            Exception realEx = ex;
            while ((realEx is System.AggregateException || realEx is System.Reflection.TargetInvocationException) && realEx.InnerException != null)
            {
                realEx = realEx.InnerException;
            }

            var isExpectedClientError = realEx is UnauthorizedAccessException
                                        || realEx is AuthorizationException
                                        || realEx is Authorization403ForbiddenException
                                        || realEx is ResultNotFoundException
                                        || realEx is ArgumentException
                                        || realEx is FasApiErrorException;

            if (isExpectedClientError)
            {
                if (realEx is FasApiErrorException fasEx && fasEx.FasApiErrorVm.StatusCode < 500)
                {
                    _logger.LogInformation("{ControllerType} returning controlled {StatusCode} response: {Message}",
                        this.GetType().Name, fasEx.FasApiErrorVm.StatusCode, realEx.Message);
                }
                else
                {
                    _logger.LogWarning("{ControllerType} returning client error ({ExceptionType}): {Message}",
                        this.GetType().Name, realEx.GetType().Name, realEx.Message);
                }
            }
            else
            {
                _logger.LogError(realEx, "Handle exception in controller base ({ControllerType}): {Message}", this.GetType(), realEx.Message);

                try
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                    var allErrors = string.Join("; ", errors);

                    var requestDetails = GetRequestDetails();

                    _logger.LogError(requestDetails);
                    if (!string.IsNullOrWhiteSpace(allErrors))
                    {
                        _logger.LogError(allErrors, inputModel?.FirstOrDefault());
                    }
                }
                catch (Exception loggingEx)
                {
                    try
                    {
                        _logger.LogError("Failed to log request details in {ControllerType}: {Message}", this.GetType(), loggingEx.Message);
                    }
                    catch
                    {
                        //ignore exception
                    }
                }
            }

            if (realEx is FasApiErrorException)
            {
                var fasApiErrorVm = (realEx as FasApiErrorException).FasApiErrorVm;
                return StatusCode(fasApiErrorVm.StatusCode, fasApiErrorVm);
            }

            if (realEx is AuthorizationException)
            {
                return Unauthorized(new FasApiErrorVm(realEx.Message, 401, ""));
            }

            if (realEx is UnauthorizedAccessException)
            {
                return Unauthorized(new FasApiErrorVm(realEx.Message, 401, ""));
            }

            if (realEx is Authorization403ForbiddenException)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new FasApiErrorVm(realEx.Message, StatusCodes.Status403Forbidden, ""));
            }

            if (realEx is ResultNotFoundException)
            {
                return StatusCode(404, new FasApiErrorVm(realEx.Message,404,""));
            };

            if (realEx is ArgumentException)
            {
                return StatusCode(400, new FasApiErrorVm(realEx.Message, 400, ""));
            }

            return StatusCode(500, new FasApiErrorVm(realEx.Message, 500, ""));
        }
    }
}