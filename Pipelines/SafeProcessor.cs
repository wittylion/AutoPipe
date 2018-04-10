namespace Pipelines
{
    public abstract class SafeProcessor<T> : SafeTypeProcessor<T> where T : PipelineContext
    {
        public override bool SafeCondition(T args)
        {
            return base.SafeCondition(args) && !args.IsAborted;
        }
    }
}