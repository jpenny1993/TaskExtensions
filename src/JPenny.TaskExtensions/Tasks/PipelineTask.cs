using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions.Tasks
{
    public abstract class PipelineTask
    {
        public bool Cancelled { get; private set; }

        public bool Completed { get; private set; }

        public bool Failed { get; private set; }

        public bool Started { get; private set; }

        public bool Succeeded { get; private set; }

        public Task CancelledAction { get; set; }

        public Task CompletedAction { get; set; }

        public IDictionary<Type, Action<Exception>> ExceptionHandlers { get; internal set; } = new Dictionary<Type, Action<Exception>>();

        public ITaskResolver TaskProvider { get; set; }

        protected async Task ExecuteAsync(Task task)
        {
            Started = true;
            try
            {
                await Pipeline.ExecuteAsync(task);
                Succeeded = true;
            }
            catch (TaskCanceledException)
            {
                Cancelled = true;
                await Pipeline.ExecuteAsync(CancelledAction);
            }
            catch (AggregateException aggEx)
            {
                Failed = true;
                aggEx.Handle(ex =>
                {
                    var exType = ex.GetType();
                    if (ExceptionHandlers.ContainsKey(exType))
                    {
                        var handler = ExceptionHandlers[exType];
                        handler(ex);
                        return true;
                    }
                    return false;
                });
            }
            catch (Exception ex)
            {
                Failed = true;
                var exType = ex.GetType();
                if (ExceptionHandlers.ContainsKey(exType))
                {
                    var handler = ExceptionHandlers[exType];
                    handler(ex);
                    return;
                }
            }
            finally
            {
                Completed = true;
                await Pipeline.ExecuteAsync(CompletedAction);
            }
        }
    }
}
