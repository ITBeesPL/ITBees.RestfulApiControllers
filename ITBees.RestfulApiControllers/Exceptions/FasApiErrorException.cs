using System;
using ITBees.RestfulApiControllers.Models;

namespace ITBees.RestfulApiControllers.Exceptions;

public class FasApiErrorException : Exception
{
    public FasApiErrorException(FasApiErrorVm fasApiErrorVm) : base(fasApiErrorVm.Message)
    {
        FasApiErrorVm = fasApiErrorVm;
    }

    public FasApiErrorException(string message, int statusCode) : base(message)
    {
        FasApiErrorVm = new FasApiErrorVm(message, statusCode);
    }
    
    public FasApiErrorException(string message, int statusCode, string errorKey) : base(message)
    {
        FasApiErrorVm = new FasApiErrorVm(message, statusCode, errorKey);
    }

    public FasApiErrorVm FasApiErrorVm { get; }
}