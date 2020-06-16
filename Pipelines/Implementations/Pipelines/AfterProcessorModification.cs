using Pipelines.Implementations.Processors;
using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public class AfterProcessorModification : IModificationConfiguration
    {
        public AfterProcessorModification(IProcessorMatcher matcher, IEnumerable<IProcessor> successors)
        {
            Matcher = matcher;
            Successors = successors;
        }

        public IProcessorMatcher Matcher { get; }
        public IEnumerable<IProcessor> Successors { get; }

        public IEnumerable<IProcessor> GetModifications(IProcessor processor)
        {
            yield return processor;

            var match = Matcher.Matches(processor);
            
            if (match)
            {
                foreach (var successor in Successors)
                {
                    yield return successor;
                }
            }
        }

        public bool HasModifications(IProcessor processor)
        {
            var match = Matcher.Matches(processor);
            return match;
        }
    }
}
