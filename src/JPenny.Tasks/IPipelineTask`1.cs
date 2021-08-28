namespace JPenny.Tasks
{
    public interface IPipelineTask<TResult> : IPipelineTask
    {
        TResult Result { get; }
    }
}
