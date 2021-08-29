using System;
using System.Threading;
using System.Threading.Tasks;
using JPenny.Tasks.Extensions;

namespace JPenny.Tasks.Builders
{
    public sealed class PipelineBuilder<TPreviousResult> : PipelineBuilderBase
    {
        internal PipelineBuilder(PipelineOptions options) : base(options)
        {
        }

        public PipelineBuilder<TPreviousResult> CancelAfter(int delayInSeconds)
            => CancelAfter(TimeSpan.FromSeconds(delayInSeconds));

        public PipelineBuilder<TPreviousResult> CancelAfter(TimeSpan delay)
        {
            Options.CancellationTokenSource.CancelAfter(delay);
            return this;
        }

        public PipelineBuilder<TPreviousResult> CancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            Options.CancellationTokenSource = cancellationTokenSource;
            return this;
        }

        public PipelineBuilder<TPreviousResult> Catch(Action<Exception> onException)
            => Catch<Exception>(onException);

        public PipelineBuilder<TPreviousResult> Catch<TException>(Action<TException> onException)
            where TException : Exception
        {
            Options.ExceptionHandlers.AddExceptionHandler(onException);
            return this;
        }

        /// <summary>
        /// Queue an action to be run when the pipeline is cancelled.
        /// </summary>
        public PipelineBuilder<TPreviousResult> OnCancelled(Action cancelAction, CancellationToken cancellationToken = default)
        {
            Options.CancelledAction = new Task(cancelAction, cancellationToken);
            return this;
        }

        /// <summary>
        /// Queue an action to be run when all pipeline tasks have run successfully.
        /// </summary>
        public PipelineBuilder<TPreviousResult> OnSuccess(Action successAction, CancellationToken cancellationToken = default)
        {
            Options.SuccessAction = new Task(successAction, cancellationToken);
            return this;
        }

        /// <summary>
        /// Queue an action to be run after the pipeline has completed.
        /// </summary>
        public PipelineBuilder<TPreviousResult> OnCompleted(Action completedAction, CancellationToken cancellationToken = default)
        {
            Options.CompletedAction = new Task(completedAction, cancellationToken);
            return this;
        }

        public PipelineBuilder TaskBuilder(Action<VoidTaskBuilder> taskBuilder)
        {
            AddVoidTask(taskBuilder);
            return new PipelineBuilder(Options);
        }

        public PipelineBuilder<TResult> TaskBuilder<TResult>(Action<ResultantTaskBuilder<TResult>> taskBuilder)
        {
            AddResultantTask(taskBuilder);
            return new PipelineBuilder<TResult>(Options);
        }

        public PipelineBuilder ThenBuilder(Action<VoidTaskBuilder<TPreviousResult>> taskBuilder)
        {
            AddVoidTask(taskBuilder);
            return new PipelineBuilder(Options);
        }

        public PipelineBuilder<TResult> ThenBuilder<TResult>(Action<ResultantTaskBuilder<TPreviousResult, TResult>> taskBuilder)
        {
            AddResultantTask(taskBuilder);
            return new PipelineBuilder<TResult>(Options);
        }

        public PipelineBuilder Then(Action action)
        {
            AddVoidTask(options => options.Action(action));
            return new PipelineBuilder(Options);
        }

        public PipelineBuilder Then(Func<Task> taskFunc)
        {
            AddVoidTask(options => options.Action(taskFunc));
            return new PipelineBuilder(Options);
        }

        public PipelineBuilder Then(Task task)
        {
            AddVoidTask(options => options.Action(task));
            return new PipelineBuilder(Options);
        }

        public PipelineBuilder Then(Func<TPreviousResult, Task> taskFunc)
        {
            AddVoidTask<TPreviousResult>(options => options.Action(taskFunc));
            return new PipelineBuilder(Options);
        }
        public PipelineBuilder<TResult> Then<TResult>(Func<TPreviousResult, TResult> taskFunc)
        {
            AddResultantTask<TPreviousResult, TResult>(options => options.Action(taskFunc));
            return new PipelineBuilder<TResult>(Options);
        }

        public PipelineBuilder Then(Action<TPreviousResult> action)
        {
            AddVoidTask<TPreviousResult>(options => options.Action(action));
            return new PipelineBuilder(Options);
        }

        public PipelineBuilder<TResult> Then<TResult>(Task<TResult> task)
        {
            AddResultantTask<TResult>(options => options.Action(task));
            return new PipelineBuilder<TResult>(Options);
        }

        public PipelineBuilder<TResult> Then<TResult>(Func<TPreviousResult, Task<TResult>> taskFunc)
        {
            AddResultantTask<TPreviousResult, TResult>(options => options.Action(taskFunc));
            return new PipelineBuilder<TResult>(Options);
        }
    }
}
