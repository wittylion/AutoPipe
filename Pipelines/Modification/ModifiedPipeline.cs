using System;
using System.Collections.Generic;

namespace Pipelines.Modifications
{
    /// <summary>
    /// Pipeline that accepts a <see cref="IModificationConfiguration"/> to be used when processors returned.
    /// </summary>
    public class ModifiedPipeline : IPipeline
    {
        public static readonly string PipelineMustBeSpecified = "You have to specify an original pipeline to be modified in ModifiedPipeline.";
        public static readonly string ConfigurationMustBeSpecified = "You have to specify a configuration to modifiy pipeline in ModifiedPipeline.";

        /// <summary>
        /// Constructor that accepts <paramref name="originalPipeline"/>, which processors
        /// must be "modified" and a <paramref name="configuration"/>
        /// which defines the modification rules.
        /// </summary>
        /// <param name="originalPipeline">
        /// A pipeline, which processors will be substituted by configuration.
        /// </param>
        /// <param name="configuration">
        /// A configuration that defines rules of substitution.
        /// </param>
        public ModifiedPipeline(IPipeline originalPipeline, IModificationConfiguration configuration)
        {
            OriginalPipeline = originalPipeline ?? throw new ArgumentNullException(PipelineMustBeSpecified);
            Configuration = configuration ?? throw new ArgumentNullException(ConfigurationMustBeSpecified);
        }

        /// <summary>
        /// Pipeline, which processors will be passed to configurator to decide which
        /// processors will left and which will be substituted or expanded.
        /// </summary>
        public IPipeline OriginalPipeline { get; }

        /// <summary>
        /// Defines a set of rules for processors that will be used from
        /// <see cref="OriginalPipeline"/>.
        /// </summary>
        public IModificationConfiguration Configuration { get; }

        /// <summary>
        /// Gets each processor of the <see cref="OriginalPipeline"/>
        /// and passes it to the <see cref="IModificationConfiguration.GetModifications(IProcessor)"/>
        /// method, the result of this method will be returned instead of the processor.
        /// </summary>
        /// <returns>
        /// Processors that are returned by configurator when passing
        /// processors of the original pipeline.
        /// </returns>
        public IEnumerable<IProcessor> GetProcessors()
        {
            var originalProcessors = OriginalPipeline.GetProcessors();
            var modifiedProcessors = Configuration.GetModifications(originalProcessors);

            return modifiedProcessors;
        }
    }
}
