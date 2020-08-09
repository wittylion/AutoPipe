using Pipelines.Implementations.Processors;
using System.Collections.Generic;

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

        public IEnumerable<IProcessor> GetModifications(IEnumerable<IProcessor> processors)
        {
            foreach (var processor in processors)
            {
                var match = Matcher.Matches(processor);

                if (match)
                {
                    foreach (var substitute in Substitutes)
                    {
                        yield return substitute;
                    }
                }
                else
                {
                    yield return processor;
                }
            }
        }
    }
}
