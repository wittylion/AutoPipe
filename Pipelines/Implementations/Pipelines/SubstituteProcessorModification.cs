using Pipelines.Implementations.Processors;
using System.Collections.Generic;
using System.Linq;

namespace Pipelines.Implementations.Pipelines
{
    public class SubstituteProcessorModification : IModificationConfiguration
    {
        public SubstituteProcessorModification(IProcessorMatcher matcher, IEnumerable<IProcessor> substitutes)
        {
            Matcher = matcher;
            Substitutes = substitutes;
        }

        public IProcessorMatcher Matcher { get; }
        public IEnumerable<IProcessor> Substitutes { get; }

        public IEnumerable<IProcessor> GetModifications(IProcessor processorType)
        {
            if (Matcher.Matches(processorType))
            {
                return Substitutes;
            }

            return Enumerable.Empty<IProcessor>();
        }
    }
}
