namespace FiniteStateMachine;

internal abstract class Behavior<TState, TCommand> : Behavior<TState, TCommand, Transition<TState, TCommand>>
{
    public new class Sync : Behavior<TState, TCommand, Transition<TState, TCommand>>.Sync
    {
        public Sync(Action<Transition<TState, TCommand>> action) : base(action)
        {
        }
    }


    public new class Async : Behavior<TState, TCommand, Transition<TState, TCommand>>.Async
    {
        public Async(Func<Transition<TState, TCommand>, Task> action) : base(action)
        {
        }
    }
}