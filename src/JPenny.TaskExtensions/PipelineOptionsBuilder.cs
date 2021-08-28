using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions
{
    public sealed class PipelineOptionsBuilder
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Task _cancelledAction;

        private Task _completedAction;

        private IList<IPipelineTask> _tasks = new List<IPipelineTask>();

        internal PipelineOptionsBuilder()
        {
        }

        public Pipeline Build() => new Pipeline
        {
            CancellationTokenSource = _cancellationTokenSource,
            CancelledAction = _cancelledAction,
            CompletedAction = _completedAction,
            Tasks = _tasks
        };

        public Task BuildAndExecuteAsync() => Build().ExecuteAsync();

        public PipelineOptionsBuilder CancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
            return this;
        }

        /// <summary>
        /// Queue an action to be run when the pipeline is cancelled.
        /// </summary>
        public PipelineOptionsBuilder OnCancelled(Action cancelAction, CancellationToken cancellationToken = default)
        {
            _cancelledAction = new Task(cancelAction, cancellationToken);
            return this;
        }

        /// <summary>
        /// Queue an action to be run after the pipeline has completed.
        /// </summary>
        public PipelineOptionsBuilder OnCompleted(Action completedAction, CancellationToken cancellationToken = default)
        {
            _completedAction = new Task(completedAction, cancellationToken);
            return this;
        }

        public PipelineOptionsBuilder Task(Action<TaskOptionsBuilder> taskOptionsBuilder)
        {
            var builder = new TaskOptionsBuilder();
            taskOptionsBuilder(builder);

            var task = builder.Build();
            _tasks.Add(task);
            return this;
        }

        public PipelineOptionsBuilder Task(Task task)
            => Task(options => options.Action(task));

        public PipelineOptionsBuilder Task<TResult>(Task<TResult> task)
            => Task(options => options.Action(task));

        public PipelineOptionsBuilder Task(Action action)
            => Task(options => options.Action(action));

        public PipelineOptionsBuilder Then(Action<TaskOptionsBuilder> taskOptionsBuilder)
        {
            if (!_tasks.Any())
            {
                throw new IndexOutOfRangeException("Must create at least one pipeline task before calling .Then(), try calling .Task() first.");
            }

            var previousTask = _tasks.Last();

            var builder = new TaskOptionsBuilder(previousTask);
            taskOptionsBuilder(builder);

            var task = builder.Build();
            _tasks.Add(task);
            return this;
        }

        public PipelineOptionsBuilder Then(Task task)
            => Task(task);

        public PipelineOptionsBuilder Then<TResult>(Task<TResult> task)
            => Task(task);

        public PipelineOptionsBuilder Then(Action action)
            => Then(options => options.Action(action));

        public PipelineOptionsBuilder Then<TPrevious>(Action<TPrevious> action)
            => Then(options => options.Action(action));

        public PipelineOptionsBuilder Then<TPrevious>(Func<TPrevious, Task> taskResolverFunc)
            => Then(options => options.Action(taskResolverFunc));

        public PipelineOptionsBuilder Then<TPrevious, TResult>(Func<TPrevious, Task<TResult>> taskResolverFunc)
            => Then(options => options.Action(taskResolverFunc));
    }
}
