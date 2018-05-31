using System.Collections.Generic;

namespace Pipelines
{
    public abstract class SafeTypePipeline<T> : IPipeline
    {
        public IEnumerable<IProcessor> GetProcessors()
        {
            return this.GetProcessorsOfType();
        }

        public abstract IEnumerable<SafeTypeProcessor<T>> GetProcessorsOfType();
    }
}