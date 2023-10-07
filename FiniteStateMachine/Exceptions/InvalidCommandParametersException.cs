using FiniteStateMachine.Extensions;

namespace FiniteStateMachine.Exceptions;

internal class InvalidCommandParametersException : Exception
{
    internal InvalidCommandParametersException(Type? type, Type mustBe) : base($"Invalid command parameters was passed. Parameters types must be ({mustBe.GetParametersTypes()}) in order. The passed types : ({type.GetParametersTypes()})")
    {
    }
}