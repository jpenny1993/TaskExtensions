using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JPenny.TaskExtensions.Builders;

namespace JPenny.TaskExtensions
{
    public sealed class Pipeline
    {
        public CancellationTokenSource CancellationTokenSource { get; internal set; } = new CancellationTokenSource();

        public Task CancelledAction { get; internal set; }

        public Task SuccessAction { get; internal set; }

        public Task CompletedAction { get; internal set; }

        public IDictionary<Type, Action<Exception>> ExceptionHandlers { get; internal set; } = new Dictionary<Type, Action<Exception>>();

        public IList<IPipelineTask> Tasks { get; internal set; } = new List<IPipelineTask>();

        public void Cancel() => CancellationTokenSource.Cancel();

        internal Pipeline()
        {
        }

        public async Task ExecuteAsync()
        {
            var token = CancellationTokenSource.Token;

            try
            {
                int taskIndex;
                for (taskIndex = 0; taskIndex < Tasks.Count; taskIndex++)
                {
                    IPipelineTask pipelineTask = Tasks[taskIndex];

                    token.ThrowIfCancellationRequested();
                    await Pipeline.ExecuteAsync(pipelineTask);

                    if (pipelineTask.Cancelled || pipelineTask.Failed)
                    {
                        await Pipeline.ExecuteAsync(CancelledAction);
                        break;
                    }
                }

                if (taskIndex == Tasks.Count)
                {
                    await Pipeline.ExecuteAsync(SuccessAction);
                }
            }
            catch (TaskCanceledException)
            {
                await Pipeline.ExecuteAsync(CancelledAction);
            }
            catch (AggregateException aggEx)
            {
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
                await Pipeline.ExecuteAsync(CompletedAction);
            }
        }

        public static PipelineBuilder Create() => new PipelineBuilder();

        public static Task ExecuteAsync(IPipelineTask pipeline)
        {
            var task = pipeline.ExecuteAsync();
            return Pipeline.ExecuteAsync(task);
        }
        public static Task ExecuteAsync(ITaskResolver resolver)
        {
            var task = resolver?.Resolve();
            return Pipeline.ExecuteAsync(task);
        }

        public static async Task ExecuteAsync(Task task)
        {
            // Skip tasks that aren't defined
            if (task == default)
            {
                return;
            }

            // Start tasks that haven't been started
            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

            await task.ConfigureAwait(false);
        }
    }
}
