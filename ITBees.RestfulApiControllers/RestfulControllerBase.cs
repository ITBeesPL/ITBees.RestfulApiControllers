﻿using System;
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

        /// <summary>
        /// Returns ok result, but if there will be an error, it will be logged. Authorization error will return 401, not found 404, bad request 500
        /// </summary>
        /// <param name="func"></param>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        private IActionResult HandleException(Exception ex, object[] inputModel)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
            var allErrors = string.Join("; ", errors);

            _logger.LogError(allErrors, inputModel.FirstOrDefault());
            _logger.LogError(ex.Message);

            if (ex is FasApiErrorException)
            {
                var fasApiErrorVm = (ex as FasApiErrorException).FasApiErrorVm;
                return StatusCode(fasApiErrorVm.StatusCode, fasApiErrorVm);
            }

            if (ex is AuthorizationException)
            {
                return Unauthorized(new FasApiErrorVm(ex.Message, 401, ""));
            }

            if (ex is UnauthorizedAccessException)
            {
                return Unauthorized(new FasApiErrorVm(ex.Message, 401, ""));
            }

            if (ex is Authorization403ForbiddenException)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new FasApiErrorVm(ex.Message, StatusCodes.Status403Forbidden, ""));
            }

            if (ex is ResultNotFoundException)
            {
                return StatusCode(404, new FasApiErrorVm(ex.Message,404,""));
            };

            if (ex is ArgumentException)
            {
                return StatusCode(400, new FasApiErrorVm(ex.Message, 400, ""));
            }

            return StatusCode(500, new FasApiErrorVm(ex.Message, 500, ""));
        }
    }
}