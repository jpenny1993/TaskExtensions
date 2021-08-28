using System.Threading.Tasks;

namespace JPenny.TaskExtensions.Tasks
{
    public sealed class VoidTask : PipelineTask, IPipelineTask
    {
        public Task ExecuteAsync()
        {
            var task = TaskProvider.Resolve();
            return ExecuteAsync(task);
        }
    }
}
