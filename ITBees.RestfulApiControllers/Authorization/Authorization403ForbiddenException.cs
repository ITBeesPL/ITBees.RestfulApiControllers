using System;

namespace ITBees.RestfulApiControllers.Authorization;

public class Authorization403ForbiddenException : Exception
{
    public Authorization403ForbiddenException(string message) : base (message)
    {
        
    }

    public Authorization403ForbiddenException(AuthorizationExceptionMessages message) : base(message.ToString())
    {

    }
}