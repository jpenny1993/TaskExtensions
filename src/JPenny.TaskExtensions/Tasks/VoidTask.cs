using System.Threading.Tasks;

namespace JPenny.TaskExtensions.Tasks
{
    public sealed class VoidTask : PipelineTask, IPipelineTask
    {
        public Task ExecuteAsync()
        {
            var task = MainTaskResolver.Resolve();
            return ExecuteAsync(task);
        }
    }
}
