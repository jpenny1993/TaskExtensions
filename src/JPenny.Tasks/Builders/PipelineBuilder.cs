using System;
using System.Threading;
using System.Threading.Tasks;
using JPenny.Tasks.Extensions;

namespace JPenny.Tasks.Builders
{
    public sealed class PipelineBuilder : PipelineBuilderBase
    {
        internal PipelineBuilder() : base()
        {
        }

        internal PipelineBuilder(PipelineOptions options) : base(options)
        {
        }

        public PipelineBuilder CancelAfter(int delayInSeconds)
            => CancelAfter(TimeSpan.FromSeconds(delayInSeconds));

        public PipelineBuilder CancelAfter(TimeSpan delay)
        {
            Options.CancellationTokenSource.CancelAfter(delay);
            return this;
        }

        public PipelineBuilder CancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            Options.CancellationTokenSource = cancellationTokenSource;
            return this;
        }

        public PipelineBuilder Catch(Action<Exception> onException)
            => Catch<Exception>(onException);

        public PipelineBuilder Catch<TException>(Action<TException> onException)
            where TException : Exception
        {
            Options.ExceptionHandlers.AddExceptionHandler(onException);
            return this;
        }

        /// <summary>
        /// Queue an action to be run when the pipeline is cancelled.
        /// </summary>
        public PipelineBuilder OnCancelled(Action cancelAction, CancellationToken cancellationToken = default)
        {
            Options.CancelledAction = new Task(cancelAction, cancellationToken);
            return this;
        }

        /// <summary>
        /// Queue an action to be run when all pipeline tasks have run successfully.
        /// </summary>
        public PipelineBuilder OnSuccess(Action successAction, CancellationToken cancellationToken = default)
        {
            Options.SuccessAction = new Task(successAction, cancellationToken);
            return this;
        }

        /// <summary>
        /// Queue an action to be run after the pipeline has completed.
        /// </summary>
        public PipelineBuilder OnCompleted(Action completedAction, CancellationToken cancellationToken = default)
        {
            Options.CompletedAction = new Task(completedAction, cancellationToken);
            return this;
        }

        public PipelineBuilder TaskBuilder(Action<VoidTaskBuilder> taskBuilder)
        {
            AddVoidTask(taskBuilder);
            return this;
        }

        public PipelineBuilder<TResult> TaskBuilder<TResult>(Action<ResultantTaskBuilder<TResult>> taskBuilder)
        {
            AddResultantTask(taskBuilder);
            return new PipelineBuilder<TResult>(Options);
        }

        public PipelineBuilder Task(Action action)
            => TaskBuilder(options => options.Action(action));

        public PipelineBuilder Task(Func<Task> taskFunc)
            => TaskBuilder(options => options.Action(taskFunc));

        public PipelineBuilder Task(Task task)
            => TaskBuilder(options => options.Action(task));

        public PipelineBuilder<TResult> Task<TResult>(Task<TResult> task)
           => TaskBuilder((ResultantTaskBuilder<TResult> options) => options.Action(task));
    }
}
