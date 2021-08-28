namespace JPenny.Tasks.Tests
{
    /// <summary>
    /// A mockable interface for a parameterless action.
    /// </summary>
    public interface IAction
    {
        void Action();
    }

    /// <summary>
    /// A mockable interface for an action with parameters.
    /// </summary>
    /// <typeparam name="TResult">The result type of the previous task.</typeparam>
    public interface IAction<in TResult>
    {
        void Action(TResult x);
    }

    /// <summary>
    /// A mockable interface for a function.
    /// </summary>
    /// <typeparam name="TResult">The result type of the previous task.</typeparam>
    /// <typeparam name="TNewResult">The result type of the function.</typeparam>
    public interface IAction<in TResult, out TNewResult>
    {
        TNewResult Action(TResult x);
    }
}
