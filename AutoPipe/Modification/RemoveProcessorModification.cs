using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPipe.Modifications
{
    public class RemoveProcessorModification : IModificationConfiguration
    {
        public RemoveProcessorModification(IEnumerable<IProcessorMatcher> matchers)
        {
            Matchers = matchers ?? throw new ArgumentNullException(nameof(matchers), "Matchers collection must be specified when creating a modification that removes processors.");
        }

        public IEnumerable<IProcessorMatcher> Matchers { get; }

        public IEnumerable<IProcessor> GetModifications(IEnumerable<IProcessor> processors)
        {
            foreach (var processor in processors)
            {
                var match = Matchers.Any(matcher => matcher.Matches(processor));

                if (match)
                {
                    continue;
                }

                yield return processor;
            }
        }
    }
}
