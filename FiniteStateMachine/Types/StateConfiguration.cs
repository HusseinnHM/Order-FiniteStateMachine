using System.Collections.Concurrent;
namespace FiniteStateMachine;

public class StateConfiguration<TState, TCommand> where TCommand : notnull
{
    private readonly ConcurrentDictionary<TCommand,CommandConfiguration<TState, TCommand>> _commands = new();


    public CommandConfiguration<TState, TCommand> Allow(TCommand command, TState nextState)
    {
        var stateInfo = new CommandConfiguration<TState, TCommand>(this, nextState);
        _commands.TryAdd(command, stateInfo);
        return stateInfo;
    }


    internal bool CommandBehaviour(TCommand command,out CommandConfiguration<TState, TCommand> commandConfiguration)
    {
        return _commands.TryGetValue(command, out commandConfiguration!);
    }
        
       
}