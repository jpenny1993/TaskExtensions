using System.Threading;
using System.Threading.Tasks;

namespace JPenny.Tasks.PipelineTasks
{
    public sealed class VoidTask : PipelineTask, IPipelineTask
    {
        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var task = MainTaskResolver.Resolve();
            return ExecuteAsync(task, cancellationToken);
        }
    }
}
