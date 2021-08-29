using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.Tasks.Builders
{
    public abstract class PipelineBuilderBase
    {
        public class PipelineOptions
        {
            public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

            public Task CancelledAction { get; set; }

            public Task SuccessAction { get; set; }

            public Task CompletedAction { get; set; }

            public IList<IPipelineTask> Tasks { get; } = new List<IPipelineTask>();

            public IDictionary<Type, Action<Exception>> ExceptionHandlers { get; } = new Dictionary<Type, Action<Exception>>();
        }

        protected PipelineOptions Options { get; }

        protected PipelineBuilderBase()
        {
            Options = new PipelineOptions();
        }

        protected PipelineBuilderBase(PipelineOptions options)
        {
            Options = options;
        }

        public Pipeline Build() => new Pipeline
        {
            CancellationTokenSource = Options.CancellationTokenSource,
            CancelledAction = Options.CancelledAction,
            SuccessAction = Options.SuccessAction,
            CompletedAction = Options.CompletedAction,
            Tasks = Options.Tasks
        };

        public Task BuildAndExecuteAsync() => Build().ExecuteAsync();

        protected void AddVoidTask(Action<VoidTaskBuilder> taskBuilder)
        {
            var task = new VoidTaskBuilder()
                .ApplyOptions(taskBuilder)
                .Build();

            Options.Tasks.Add(task);
        }

        protected void AddVoidTask<TPreviousResult>(Action<VoidTaskBuilder<TPreviousResult>> taskBuilder)
        {
            var previousTask = ValidatePreviousTask<TPreviousResult>();
            var task = new VoidTaskBuilder<TPreviousResult>(previousTask)
                .ApplyOptions(taskBuilder)
                .Build();

            Options.Tasks.Add(task);
        }

        protected void AddResultantTask<TResult>(Action<ResultantTaskBuilder<TResult>> taskBuilder)
        {
            var task = new ResultantTaskBuilder<TResult>()
                .ApplyOptions(taskBuilder)
                .Build();

            Options.Tasks.Add(task);
        }

        protected void AddResultantTask<TPreviousResult, TResult>(Action<ResultantTaskBuilder<TPreviousResult, TResult>> taskBuilder)
        {
            var previousTask = ValidatePreviousTask<TPreviousResult>();
            var task = new ResultantTaskBuilder<TPreviousResult, TResult>(previousTask)
                .ApplyOptions(taskBuilder)
                .Build();

            Options.Tasks.Add(task);
        }

        private IPipelineTask<TInput> ValidatePreviousTask<TInput>()
        {
            if (!Options.Tasks.Any())
            {
                throw new IndexOutOfRangeException("Must create at least one pipeline task before calling .Then(), try calling .Task() first.");
            }

            var previousTask = Options.Tasks.Last();
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
