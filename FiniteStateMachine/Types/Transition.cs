using FiniteStateMachine.Exceptions;

namespace FiniteStateMachine;

public class Transition<TState, TCommand>
{
    private readonly object? _commandParameters;
    private readonly object? _ifParameters;

    public Transition(TState current, TState next, TCommand command, object? commandParameters = null,
        object? ifParameters = null)
    {
        Current = current;
        Next = next;
        Command = command;
        _commandParameters = commandParameters;
        _ifParameters = ifParameters;
    }

    public TState Current { get; }
    public TState Next { get; }
    public TCommand Command { get; }

    public bool TryGetCommandParameters<T>(out T? commandParameters)
    {
        if (_commandParameters is T result)
        {
            commandParameters = result;
            return true;
        }

        commandParameters = default;
        return false;
    }

    public bool TryGetIfParameters<T>(out T? ifParameters)
    {
        if (_ifParameters is T result)
        {
            ifParameters = result;
            return true;
        }

        ifParameters = default;
        return false;
    }

    public T CommandParameters<T>()
    {
        if (TryGetCommandParameters<T>(out var commandParameters))
        {
            return commandParameters!;
        }

        throw new InvalidCommandParametersException(_commandParameters?.GetType(), typeof(T));
    }

    public T IfParameters<T>()
    {
        if (TryGetIfParameters<T>(out var ifParameters))
        {
            return ifParameters!;
        }

        throw new InvalidIfParametersException(_ifParameters?.GetType(), typeof(T));
    }
}