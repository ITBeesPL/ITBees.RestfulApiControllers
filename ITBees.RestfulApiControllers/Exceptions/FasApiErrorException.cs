using System;
using ITBees.RestfulApiControllers.Models;

namespace ITBees.RestfulApiControllers.Exceptions;

public class FasApiErrorException : Exception
{
    public FasApiErrorException(FasApiErrorVm fasApiErrorVm) : base(fasApiErrorVm.Message)
    {
        FasApiErrorVm = fasApiErrorVm;
    }

    public FasApiErrorVm FasApiErrorVm { get; }
}