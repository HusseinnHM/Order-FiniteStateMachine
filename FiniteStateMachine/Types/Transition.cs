namespace FiniteStateMachine;

public class Transition<TState, TCommand>
{
    public Transition(TState current, TState next, TCommand command, object? commandParameters = null,
        object? ifParameters = null)
    {
        Current = current;
        Next = next;
        Command = command;
        CommandParameters = commandParameters;
        IfParameters = ifParameters;
    }
    
    public TState Current { get; }
    public TState Next { get; }
    public TCommand Command { get; }
    public object? CommandParameters { get; }
    public object? IfParameters { get; }

}