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

        public IEnumerable<IProcessor> GetModifications(IProcessor processor)
        {

            var match = Matcher.Matches(processor);

            if (match)
            {
                yield break;
            }
            else
            {
                yield return processor;
            }
        }

        public bool HasModifications(IProcessor processor)
        {
            var match = Matcher.Matches(processor);
            return match;
        }
    }
}
