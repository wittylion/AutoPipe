using Pipelines.Implementations.Processors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pipelines.Implementations.Pipelines
{
    public class BeforeProcessorModification : IModificationConfiguration
    {
        public BeforeProcessorModification(IProcessorMatcher matcher, IEnumerable<IProcessor> predecessors)
        {
            Matcher = matcher;
            Predecessors = predecessors;
        }

        public IProcessorMatcher Matcher { get; }
        public IEnumerable<IProcessor> Predecessors { get; }

        public IEnumerable<IProcessor> GetModifications(IEnumerable<IProcessor> processors)
        {
            foreach (var processor in processors)
            {
                var match = Matcher.Matches(processor);

                if (match)
                {
                    foreach (var predecessor in Predecessors)
                    {
                        yield return predecessor;
                    }
                }

                yield return processor;
            }
        }
    }
}
