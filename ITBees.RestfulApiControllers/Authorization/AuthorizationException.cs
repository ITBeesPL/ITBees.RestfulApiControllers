using System;

namespace ITBees.RestfulApiControllers.Authorization
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(AuthorizationExceptionMessages message):base(message.ToString())
        {
            
        }
    }
}