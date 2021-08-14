using Pipelines.Modifications;

namespace Pipelines
{
    public static class ProcessorMatcherExtensionMethods
    {
        public static IProcessorMatcher Or(this IProcessorMatcher originalMatcher, IProcessorMatcher alternativeMatcher)
        {
            return new ProcessorMatcherDisjunction(originalMatcher, alternativeMatcher);
        }
    }
}
