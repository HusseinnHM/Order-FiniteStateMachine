using FiniteStateMachine.Extensions;

namespace FiniteStateMachine.Exceptions;

internal class InvalidIfParametersException : Exception
{
    internal InvalidIfParametersException(Type? type, Type mustBe) : base($"Invalid if parameters was passed. Parameters types must be ({mustBe.GetParametersTypes()}) in order. The passed types : ({type.GetParametersTypes()})")
    {
    }
}