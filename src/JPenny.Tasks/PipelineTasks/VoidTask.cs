using System.Threading.Tasks;

namespace JPenny.Tasks.PipelineTasks
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
