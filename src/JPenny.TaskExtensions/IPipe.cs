using System;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions
{
    public interface IPipe
    {
        IPipe OnCancelled(Action action);
        IPipe OnComplete(Action action);
        IPipe OnException(Action<Exception> action);
        IPipe OnException<TException>(Action<TException> action) where TException : Exception;

        // TODO: be able to call on success from an IPipe
        // IPipe OnSuccess(Action<TResult> action);

        Task RunAsync(CancellationToken cancellationToken = default);
    }
}