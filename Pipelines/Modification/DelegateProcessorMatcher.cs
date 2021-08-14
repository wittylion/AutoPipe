using System;

namespace Pipelines.Modifications
{
    public class DelegateProcessorMatcher : IProcessorMatcher
    {
        public static readonly string PredicateShouldBeSpecified = "Using delegate processor matcher, the predicate that checks processor must be specified.";

        public DelegateProcessorMatcher(Predicate<IProcessor> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate), PredicateShouldBeSpecified);
        }

        public Predicate<IProcessor> Predicate { get; }

        public bool Matches(IProcessor processor)
        {
            var result = Predicate(processor);
            return result;
        }
    }
}
