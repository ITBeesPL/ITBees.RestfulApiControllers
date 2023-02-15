using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using RestfullApi.Authorization;

namespace RestfullApi
{
    [ApiController]
    [Route("[controller]")]
    public abstract class RestfulControllerBase<T> : ControllerBase where T : class
    {
        public RestfulControllerBase(ILogger<T> logger)
        {
            
        }

        protected IActionResult CreateBaseErrorResponse(string[] errorMessages, object inputModel)
        {
            return new BadRequestModel(errorMessages, inputModel);
        }
        
        protected IActionResult CreateBaseErrorResponse(string errorMessage, object inputModel)
        {
            return new BadRequestModel(new string[] { errorMessage }, inputModel);
        }
        protected BadRequestObjectResult CreateBaseErrorResponse(Exception e, object inputModel)
        {
            if (e.GetType() == typeof(AuthorizationException))
            {
                return new AuthorizationErrorModel(e.Message, inputModel);
            }

            return new BadRequestModel(new string[] { e.Message}, inputModel);
        }

        
    }
}
