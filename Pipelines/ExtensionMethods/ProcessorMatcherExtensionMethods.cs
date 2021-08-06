using Pipelines.Implementations.Processors;
using System;
using System.Collections.Generic;
using System.Text;

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
