using Pipelines.Implementations.Processors;
using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public class RemoveProcessorModification : IModificationConfiguration
    {
        public RemoveProcessorModification(IProcessorMatcher matcher)
        {
            Matcher = matcher;
        }

        public IProcessorMatcher Matcher { get; }

        public IEnumerable<IProcessor> GetModifications(IEnumerable<IProcessor> processors)
        {
            foreach (var processor in processors)
            {
                var match = Matcher.Matches(processor);

                if (match)
                {
                    continue;
                }

                yield return processor;
            }
        }
    }
}
