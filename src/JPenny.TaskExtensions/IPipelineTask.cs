using System.Threading.Tasks;

namespace JPenny.TaskExtensions
{
    public interface IPipelineTask
    {
        bool Cancelled { get; }

        bool Completed { get; }

        bool Failed { get; }

        bool Started { get; }

        bool Succeeded { get; }

        Task ExecuteAsync();
    }

    public interface IPipelineTask<TResult> : IPipelineTask
    {
        TResult Result { get; }
    }
}
