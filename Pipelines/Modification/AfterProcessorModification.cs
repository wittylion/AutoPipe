using System.Collections.Generic;

namespace AutoPipe.Modifications
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

        public IEnumerable<IProcessor> GetModifications(IEnumerable<IProcessor> processors)
        {
            foreach (var processor in processors)
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
        }
    }
}
