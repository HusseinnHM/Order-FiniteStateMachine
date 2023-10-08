using System.Collections.Concurrent;

namespace FiniteStateMachine;

public class StateMachine<TState, TCommand> where TCommand : notnull where TState : notnull
{
    private readonly ConcurrentDictionary<TState, StateConfiguration<TState, TCommand>> _dic;

    private readonly List<Behavior<TState, TCommand, Transition<TState, TCommand>>> _onTransactionBehaviors = new();
    private TState _currentStat;


    protected StateMachine(TState currentStat)
    {
        _dic = new();
        _currentStat = currentStat;
    }

    protected StateMachine(TState currentStat, IEqualityComparer<TState> equalityComparer)
    {
        _dic = new(equalityComparer);
        _currentStat = currentStat;
    }

    public static StateMachine<TState, TCommand> Create(TState current)
    {
        return new(current);
    }

    public static StateMachine<TState, TCommand> Create(TState current, IEqualityComparer<TState> equalityComparer)
    {
        return new(current, equalityComparer);
    }

    public StateMachine<TState, TCommand> OnTransition(Action<Transition<TState, TCommand>> action)
    {
        _onTransactionBehaviors.Add(new Behavior<TState, TCommand, Transition<TState, TCommand>>.Sync(action));
        return this;
    }

    public StateMachine<TState, TCommand> OnTransitionAsync(Func<Transition<TState, TCommand>, Task> action)
    {
        _onTransactionBehaviors.Add(new Behavior<TState, TCommand, Transition<TState, TCommand>>.Async(action));
        return this;
    }

    public void Start(TState current) => _currentStat = current;

    public StateConfiguration<TState, TCommand> When(TState currentState)
    {
        if (_dic.TryGetValue(currentState, out var state))
        {
            return state;
        }

        state = new();
        _dic.TryAdd(currentState, state);

        return state;
    }

    #region Fire

    public bool Fire<TCommandParams, TIfParams>(TCommand command,
        TCommandParams commandParams,
        TIfParams ifParams)
    {
        if (!TryGetCommandConfiguration(command, out var commandConfiguration))
        {
            return false;
        }

        var transition = new Transition<TState, TCommand>(_currentStat,
            commandConfiguration.Next,
            command,
            commandParams,
            ifParams);

        if (!commandConfiguration.Can(transition))
        {
            return false;
        }

        commandConfiguration.Execute(transition);

        foreach (var onTransactionBehavior in _onTransactionBehaviors)
        {
            onTransactionBehavior.Execute(transition);
        }

        _currentStat = transition.Next;
        return true;
    }


    public bool Fire(TCommand command)
    {
        return Fire<IParameterless, IParameterless>(command, null!, null!);
    }

    public bool FireWithCommandParams<TCommandParams>(
        TCommand command,
        TCommandParams commandParams)
    {
        return Fire<TCommandParams, IParameterless>(command, commandParams, null!);
    }

    public bool FireWithIfParams<TIfParams>(
        TCommand command,
        TIfParams ifParams)
    {
        return Fire<IParameterless, TIfParams>(command, null!, ifParams);
    }

    #endregion

    #region FireAsync

    public async ValueTask<bool> FireAsync<TCommandParams, TIfParams>(TCommand command,
        TCommandParams commandParams,
        TIfParams ifParams)
    {
        if (!TryGetCommandConfiguration(command, out var commandConfiguration))
        {
            return false;
        }

        var transition = new Transition<TState, TCommand>(_currentStat,
            commandConfiguration.Next,
            command,
            commandParams,
            ifParams);

        if (!await commandConfiguration.CanAsync(transition))
        {
            return false;
        }

        await commandConfiguration.ExecuteAsync(transition);

        foreach (var onTransactionBehavior in _onTransactionBehaviors)
        {
            await onTransactionBehavior.ExecuteAsync(transition);
        }

        _currentStat = transition.Next;
        return true;
    }


    public async ValueTask<bool> FireAsync(TCommand command)
    {
        return await FireAsync<IParameterless, IParameterless>(command, null!, null!);
    }

    public async ValueTask<bool> FireWithCommandParamsAsync<TCommandParams>(
        TCommand command,
        TCommandParams commandParams)
    {
        return await FireAsync<TCommandParams, IParameterless>(command, commandParams, null!);
    }

    public async ValueTask<bool> FireWithIfParamsAsync<TIfParams>(
        TCommand command,
        TIfParams ifParams)
    {
        return await FireAsync<IParameterless, TIfParams>(command, null!, ifParams);
    }

    #endregion

    private bool TryGetCommandConfiguration(TCommand command, out CommandConfiguration<TState, TCommand> commandConfiguration)
    {
        commandConfiguration = null!;
        return _dic.TryGetValue(_currentStat, out var stat) && stat.CommandBehaviour(command, out commandConfiguration);
    }
}