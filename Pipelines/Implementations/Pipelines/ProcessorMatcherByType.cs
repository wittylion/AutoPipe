using System;

namespace Pipelines.Implementations.Pipelines
{
    public class ProcessorMatcherByType : IProcessorMatcher
    {
        public ProcessorMatcherByType(Type type)
        {
            Type = type;
        }

        public Type Type { get; }

        public bool Matches(IProcessor processor)
        {
            return processor.GetType() == Type;
        }
    }
}
