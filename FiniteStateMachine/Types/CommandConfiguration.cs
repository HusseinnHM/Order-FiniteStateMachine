﻿namespace FiniteStateMachine;

public class CommandConfiguration<TState, TCommand>
{
    internal readonly TState Next;
    private readonly InternalState<TState, TCommand> _internalState;
    private readonly List<Behavior<TState, TCommand, Transition<TState, TCommand>>> _actionBehaviors = new();
    private readonly List<IfBehavior<TState, TCommand>> _ifBehaviors = new();


    public CommandConfiguration(InternalState<TState, TCommand> internalState, TState next)
    {
        Next = next;
        _internalState = internalState;
    }

    public CommandConfiguration<TState, TCommand> OrAllow(TCommand command, TState nextState) =>
        _internalState.Allow(command, nextState);


    #region If

    public CommandConfiguration<TState, TCommand> If(Func<bool> predicate)
    {
        _ifBehaviors.Add(new IfBehavior<TState, TCommand>.Sync(_ => predicate()));
        return this;
    }


    public CommandConfiguration<TState, TCommand> If<TIfParams>(Func<TIfParams, bool> predicate)
    {
        _ifBehaviors.Add(new IfBehavior<TState, TCommand>.Sync(t => (bool)predicate.DynamicInvoke(t, t.IfParameters)!));
        return this;
    }

    public CommandConfiguration<TState, TCommand> If(Func<Transition<TState, TCommand>, bool> predicate)
    {
        _ifBehaviors.Add(new IfBehavior<TState, TCommand>.Sync(predicate));
        return this;
    }

    public CommandConfiguration<TState, TCommand> If<TIfParams>(
        Func<Transition<TState, TCommand>, TIfParams, bool> predicate)
    {
        _ifBehaviors.Add(new IfBehavior<TState, TCommand>.Sync(t => predicate(t, (TIfParams)t.IfParameters!)));
        return this;
    }

    #endregion

    #region IfAsync

    public CommandConfiguration<TState, TCommand> IfAsync(Func<ValueTask<bool>> predicate)
    {
        _ifBehaviors.Add(new IfBehavior<TState, TCommand>.Async(_ => predicate()));
        return this;
    }

    public CommandConfiguration<TState, TCommand> IfAsync<TIfParams>(Func<TIfParams, ValueTask<bool>> predicate)
    {
        _ifBehaviors.Add(new IfBehavior<TState, TCommand>.Async(t =>
            (ValueTask<bool>)predicate.DynamicInvoke(t, t.IfParameters)!));
        return this;
    }

    public CommandConfiguration<TState, TCommand> IfAsync(Func<Transition<TState, TCommand>, ValueTask<bool>> predicate)
    {
        _ifBehaviors.Add(new IfBehavior<TState, TCommand>.Async(predicate));
        return this;
    }

    public CommandConfiguration<TState, TCommand> IfAsync<TIfParams>(
        Func<Transition<TState, TCommand>, TIfParams, ValueTask<bool>> predicate)
    {
        _ifBehaviors.Add(
            new IfBehavior<TState, TCommand>.Async(t => predicate(t, (TIfParams)t.IfParameters!)));
        return this;
    }

    #endregion

    #region OnExecute

    public CommandConfiguration<TState, TCommand> OnExecute(Action<Transition<TState, TCommand>> action)
    {
        _actionBehaviors.Add(new Behavior<TState, TCommand>.Sync(action));
        return this;
    }

    public CommandConfiguration<TState, TCommand> OnExecute<TCommandParams>(
        Action<Transition<TState, TCommand>, TCommandParams> action)
    {
        _actionBehaviors.Add(
            new Behavior<TState, TCommand>.Sync(t => action(t, (TCommandParams)t.CommandParameters!)));
        return this;
    }

    public CommandConfiguration<TState, TCommand> OnExecute(Action action)
    {
        _actionBehaviors.Add(new Behavior<TState, TCommand>.Sync(_ => action()));
        return this;
    }

    public CommandConfiguration<TState, TCommand> OnExecute<TCommandParams>(Action<TCommandParams> action)
    {
        _actionBehaviors.Add(new Behavior<TState, TCommand>.Sync(t => action.DynamicInvoke(t.CommandParameters)));
        return this;
    }

    #endregion


    internal async ValueTask<bool> CanAsync(Transition<TState, TCommand> transition)
    {
        foreach (var behavior in _ifBehaviors)
        {
            return await behavior.ExecuteAsync(transition);
        }

        return true;
    }


    internal async Task ExecuteAsync(Transition<TState, TCommand> transition)
    {
        foreach (var behavior in _actionBehaviors)
        {
            await behavior.ExecuteAsync(transition);
        }
    }
}