namespace Pipelines.Implementations.Pipelines
{
    public class ProcessorMatcherByInstance : IProcessorMatcher
    {
        public ProcessorMatcherByInstance(IProcessor processor)
        {
            Processor = processor;
        }

        public IProcessor Processor { get; }

        public bool Matches(IProcessor processor)
        {
            return processor == Processor;
        }
    }
}
