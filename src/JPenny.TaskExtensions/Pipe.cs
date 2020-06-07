using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions
{
    public class Pipe<TResult> : IPipe
    {
        // TODO: add cancellation token source
        public Task<TResult> MainTask { get; private set; }
        public Action CancelledAction { get; private set; }
        public Action CompleteAction { get; private set; }
        public IDictionary<Type, Action<Exception>> ExceptionActions { get; } = new Dictionary<Type, Action<Exception>>();
        public Action<Exception> GlobalExceptionAction { get; private set; }
        public Action<TResult> SuccessAction { get; private set; }

        public Pipe(Task<TResult> task) => MainTask = task;

        public IPipe OnCancelled(Action action)
        {
            CancelledAction = action;
            return this;
        }

        public IPipe OnException(Action<Exception> action)
        {
            GlobalExceptionAction = action;
            return this;
        }

        public IPipe OnException<TException>(Action<TException> action) where TException : Exception
        {
            var exType = typeof(TException);
            if (ExceptionActions.ContainsKey(exType))
            {
                throw new ArgumentOutOfRangeException($"Unable to add multiple exception handlers of type {exType.FullName}.");
            }

            var exceptionHandler = new Action<Exception>(ex =>
            {
                if (ex is TException tEx)
                {
                    action(tEx);
                }
            });

            ExceptionActions.Add(exType, exceptionHandler);
            return this;
        }

        public IPipe OnComplete(Action action)
        {
            CompleteAction = action;
            return this;
        }

        public IPipe OnSuccess(Action<TResult> action)
        {
            SuccessAction = action;
            return this;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await MainTask;
                if (SuccessAction != default)
                {
                    SuccessAction(result);
                }
            }
            catch (TaskCanceledException)
            {
                if (CancelledAction != default)
                {
                    CancelledAction();
                }
            }
            catch (AggregateException aEx)
            {
                aEx.Handle(ex =>
                {
                    var exType = ex.GetType();
                    if (ExceptionActions.ContainsKey(exType))
                    {
                        var handler = ExceptionActions[exType];
                        handler(ex);
                        return true;
                    }
                    else if (GlobalExceptionAction != default)
                    {
                        GlobalExceptionAction(ex);
                        return true;
                    }
                    return false;
                });
                // TODO: use cancellation token source to cancel following tasks
            }
            catch (Exception ex)
            {
                var exType = ex.GetType();
                if (ExceptionActions.ContainsKey(exType))
                {
                    var handler = ExceptionActions[exType];
                    handler(ex);
                }
                else if (GlobalExceptionAction != default)
                {
                    GlobalExceptionAction(ex);
                }
                // TODO: use cancellation token source to cancel following tasks
            }
            finally
            {
                CompleteAction();
            }
        }
    }
}
