namespace Pipelines.Implementations.Processors
{
    public static class ProcessorMatcher
    {
        public static ProcessorMatcherByInstance ByInstance(IProcessor instance)
        {
            return new ProcessorMatcherByInstance(instance);
        }

        public static ProcessorMatcherByType ByType<TProcessor>() where TProcessor: IProcessor
        {
            return new ProcessorMatcherByType(typeof(TProcessor));
        }
    }
}
