using System.Threading.Tasks;

namespace JPenny.Tasks
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
}
