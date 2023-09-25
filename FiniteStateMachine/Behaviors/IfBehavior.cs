namespace FiniteStateMachine;

internal abstract class IfBehavior<TState, TCommand>
{
    public abstract bool Execute(Transition<TState, TCommand> transition);
    public abstract ValueTask<bool> ExecuteAsync(Transition<TState, TCommand> transition);


    public class Sync : IfBehavior<TState, TCommand>
    {
        readonly Func<Transition<TState, TCommand>, bool> _predicate;

        public Sync(Func<Transition<TState, TCommand>, bool> predicate)
        {
            _predicate = predicate;
        }


        public override bool Execute(Transition<TState, TCommand> transition)
        {
            return _predicate(transition);
        }

        public override ValueTask<bool> ExecuteAsync(Transition<TState, TCommand> transition)
        {
            return ValueTask.FromResult(Execute(transition));
        }
    }

    public class Async : IfBehavior<TState, TCommand>
    {
        readonly Func<Transition<TState, TCommand>, ValueTask<bool>> _predicate;

        public Async(Func<Transition<TState, TCommand>, ValueTask<bool>> predicate)
        {
            _predicate = predicate;
        }

        public override bool Execute(Transition<TState, TCommand> transition)
        {
            throw new InvalidOperationException(
                $"Cannot execute asynchronous action specified in OnExit event for '{transition.Current}' state. " +
                "Use asynchronous version of Fire [FireAsync]");
        }

        public override async ValueTask<bool> ExecuteAsync(Transition<TState, TCommand> transition)
        {
            return await _predicate(transition);
        }
    }
}