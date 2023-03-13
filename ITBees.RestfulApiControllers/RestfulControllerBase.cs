using System;
using System.Linq;
using ITBees.RestfulApiControllers.Authorization;
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


    }
}
