using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions.Builders
{
    public sealed class PipelineBuilder
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Task _cancelledAction;

        private Task _successAction;

        private Task _completedAction;

        private IList<IPipelineTask> _tasks = new List<IPipelineTask>();

        internal PipelineBuilder()
        {
        }

        public Pipeline Build() => new Pipeline
        {
            CancellationTokenSource = _cancellationTokenSource,
            CancelledAction = _cancelledAction,
            SuccessAction = _successAction,
            CompletedAction = _completedAction,
            Tasks = _tasks
        };

        public Task BuildAndExecuteAsync() => Build().ExecuteAsync();

        public PipelineBuilder CancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
            return this;
        }

        /// <summary>
        /// Queue an action to be run when the pipeline is cancelled.
        /// </summary>
        public PipelineBuilder OnCancelled(Action cancelAction, CancellationToken cancellationToken = default)
        {
            _cancelledAction = new Task(cancelAction, cancellationToken);
            return this;
        }

        /// <summary>
        /// Queue an action to be run when all pipeline tasks have run successfully.
        /// </summary>
        public PipelineBuilder OnSuccess(Action successAction, CancellationToken cancellationToken = default)
        {
            _successAction = new Task(successAction, cancellationToken);
            return this;
        }

        /// <summary>
        /// Queue an action to be run after the pipeline has completed.
        /// </summary>
        public PipelineBuilder OnCompleted(Action completedAction, CancellationToken cancellationToken = default)
        {
            _completedAction = new Task(completedAction, cancellationToken);
            return this;
        }

        public PipelineBuilder BuildTask(Action<VoidTaskBuilder> taskBuilder)
        {
            var task = new VoidTaskBuilder()
                .ApplyOptions(taskBuilder)
                .Build();

            _tasks.Add(task);
            return this;
        }

        public PipelineBuilder BuildTask<TResult>(Action<ResultantTaskBuilder<TResult>> taskBuilder)
        {
            var task = new ResultantTaskBuilder<TResult>()
                .ApplyOptions(taskBuilder)
                .Build();

            _tasks.Add(task);
            return this;
        }

        public PipelineBuilder BuildThen<TPreviousResult>(Action<VoidTaskBuilder<TPreviousResult>> taskBuilder)
        {
            var previousTask = ValidatePreviousTask<TPreviousResult>();

            var task = new VoidTaskBuilder<TPreviousResult>(previousTask)
                .ApplyOptions(taskBuilder)
                .Build();

            _tasks.Add(task);
            return this;
        }

        public PipelineBuilder BuildThen<TPreviousResult, TResult>(Action<ResultantTaskBuilder<TPreviousResult, TResult>> taskBuilder)
        {
            var previousTask = ValidatePreviousTask<TPreviousResult>();

            var task = new ResultantTaskBuilder<TPreviousResult, TResult>(previousTask)
                .ApplyOptions(taskBuilder)
                .Build();

            _tasks.Add(task);
            return this;
        }

        public PipelineBuilder Task(Action action)
            => BuildTask(options => options.Action(action));

        public PipelineBuilder Task(Func<Task> taskFunc)
            => BuildTask(options => options.Action(taskFunc));

        public PipelineBuilder Task(Task task)
            => BuildTask(options => options.Action(task));

        public PipelineBuilder Task<TResult>(Task<TResult> task)
           => BuildTask((ResultantTaskBuilder<TResult> options) => options.Action(task));

        public PipelineBuilder Then<TPreviousResult>(Action<TPreviousResult> action)
            => BuildThen<TPreviousResult>(options => options.Action(action));

        public PipelineBuilder Then<TPreviousResult>(Func<TPreviousResult, Task> taskFunc)
            => BuildThen<TPreviousResult>(options => options.Action(taskFunc));

        public PipelineBuilder Then<TPreviousResult, TResult>(Func<TPreviousResult, Task<TResult>> taskFunc)
            => BuildThen<TPreviousResult, TResult>(options => options.Action(taskFunc));

        private IPipelineTask<TInput> ValidatePreviousTask<TInput>()
        {
            if (!_tasks.Any())
            {
                throw new IndexOutOfRangeException("Must create at least one pipeline task before calling .Then(), try calling .Task() first.");
            }

            var previousTask = _tasks.Last();
            var expectedTaskType = typeof(IPipelineTask<TInput>);
            var previousTaskType = previousTask.GetType();

            if (!expectedTaskType.IsAssignableFrom(previousTaskType))
            {
                throw new InvalidCastException($"Unable to cast task {previousTaskType.Name} to {expectedTaskType.Name}.");
            }

            return (IPipelineTask<TInput>)previousTask;
        }
    }
}
