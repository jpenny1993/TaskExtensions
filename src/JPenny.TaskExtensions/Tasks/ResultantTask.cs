using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions.Tasks
{
    public sealed class ResultantTask<TResult> : PipelineTask, IPipelineTask, IPipelineTask<TResult>
    {
        private Task<TResult> _task;

        public TResult Result => _task.Result;

        public ResultantTask(
            ITaskResolver taskResolver,
            Dictionary<Type, Action<Exception>> exceptionHandlers,
            Task onCancelled,
            Task onCompeleted)
        {
            TaskProvider = taskResolver;
            ExceptionHandlers = exceptionHandlers;
            CancelledAction = onCancelled;
            CompletedAction = onCompeleted;
        }

        public Task ExecuteAsync()
        {
            _task = (Task<TResult>)TaskProvider.Resolve();
            return ExecuteAsync(_task);
        }
    }
}
