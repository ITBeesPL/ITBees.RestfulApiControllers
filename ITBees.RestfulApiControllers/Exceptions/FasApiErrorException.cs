using System;
using ITBees.RestfulApiControllers.Models;

namespace ITBees.RestfulApiControllers.Exceptions;

public class FasApiErrorException : Exception
{
    public FasApiErrorException(string message, FasApiErrorVm fasApiErrorVm) : base(message)
    {
        FasApiErrorVm = fasApiErrorVm;
    }

    public FasApiErrorVm FasApiErrorVm { get; }
}