using System;

namespace RestfullApi.Authorization
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(AuthorizationExceptionMessages message):base(message.ToString())
        {
            
        }
    }
}