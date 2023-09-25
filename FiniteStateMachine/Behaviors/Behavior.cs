namespace FiniteStateMachine;

internal abstract class Behavior<TState, TCommand, TTransition> where TTransition : Transition<TState, TCommand>
{
    public abstract void Execute(TTransition transition);
    public abstract Task ExecuteAsync(TTransition transition);

    public class Sync : Behavior<TState, TCommand, TTransition>
    {
        readonly Action<TTransition> _action;

        public Sync(Action<TTransition> action)
        {
            _action = action;
        }


        public override void Execute(TTransition transition)
        {
            _action(transition);
        }

        public override Task ExecuteAsync(TTransition transition)
        {
            Execute(transition);
            return Task.CompletedTask;
        }
    }


    public class Async : Behavior<TState, TCommand, TTransition>
    {
        readonly Func<TTransition, Task> _action;

        public Async(Func<TTransition, Task> action)
        {
            _action = action;
        }

        public override void Execute(TTransition transition)
        {
            throw new InvalidOperationException("Cannot execute asynchronous action. Use FireAsync()");
        }

        public override async Task ExecuteAsync(TTransition transition)
        {
            await _action(transition);
        }
    }
}