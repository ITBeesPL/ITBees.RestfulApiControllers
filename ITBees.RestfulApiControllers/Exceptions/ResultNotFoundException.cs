using System;

namespace ITBees.RestfulApiControllers.Exceptions;

public class ResultNotFoundException(string message) : Exception(message);