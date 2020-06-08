using System;
using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public class ModifiedPipeline : IPipeline
    {
        public static readonly string PipelineMustBeSpecified = "You have to specify an original pipeline to be modified in ModifiedPipeline.";
        public static readonly string ConfigurationMustBeSpecified = "You have to specify a configuration to modifiy pipeline in ModifiedPipeline.";
        public ModifiedPipeline(IPipeline originalPipeline, IModificationConfiguration configuration)
        {
            OriginalPipeline = originalPipeline ?? throw new ArgumentNullException(PipelineMustBeSpecified);
            Configuration = configuration ?? throw new ArgumentNullException(ConfigurationMustBeSpecified);
        }

        public IPipeline OriginalPipeline { get; }
        public IModificationConfiguration Configuration { get; }

        public IEnumerable<IProcessor> GetProcessors()
        {
            foreach (var processor in OriginalPipeline.GetProcessors())
            {
                foreach(var substitute in Configuration.GetModifications(processor))
                {
                    yield return substitute;
                }
            }
        }
    }
}
