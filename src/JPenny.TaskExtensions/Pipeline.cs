using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JPenny.TaskExtensions
{
    public class Pipeline
    {
        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
        public IList<IPipe> Tasks { get; } = new List<IPipe>();

        public void Cancel() => CancellationTokenSource.Cancel();

        public void CancelAfter(TimeSpan timeSpan) => CancellationTokenSource.CancelAfter(timeSpan);

        public Pipeline Pipe(Func<IPipe> pipelineTask)
        {
            var pipe = pipelineTask();
            Tasks.Add(pipe);
            return this;
        }

        public Pipeline Pipe<TResult>(Task<TResult> task, Action<Pipe<TResult>> pipelineTask)
        {
            var pipe = new Pipe<TResult>(task);
            pipelineTask(pipe);
            Tasks.Add(pipe);
            return this;
        }

        // TODO: allow following pipes to receive the result of the previous pipe

        public async Task RunAsync()
        {
            foreach (var task in Tasks)
            {
                await task.RunAsync(CancellationTokenSource.Token);
            }
        }

        public static Pipeline Create() => new Pipeline();
    }
}
