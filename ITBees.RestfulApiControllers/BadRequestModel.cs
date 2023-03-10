using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ITBees.RestfulApiControllers
{
    public class AuthorizationErrorModel : BadRequestModel
    {
        public AuthorizationErrorModel(string message, object inputModel) : base(message)
        {
        }
    }

    public class BadRequestModel : BadRequestObjectResult
    {
        public BadRequestModel(object error) : base(error)
        {
        }
        public BadRequestModel(string[] errorMessages, object inputModel) : base(errorMessages.First())
        {
            ErrorMessages = errorMessages;
            InputModel = inputModel;
        }

        public BadRequestModel(ModelStateDictionary modelState) : base(modelState)
        {
        }

        public object InputModel { get; set; }
        public string[] ErrorMessages { get; set; }
    }
}