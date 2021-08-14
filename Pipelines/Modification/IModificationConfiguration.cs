using System.Collections.Generic;

namespace Pipelines.Modifications
{
    /// <summary>
    /// An interface declares a way of retrieving modifications.
    /// In this case a collection of processors must be provided
    /// to the method which will return a new modified collection.
    /// </summary>
    /// <example>
    /// 
    /// public IPipeline GetModifiedPipeline ( IModificationConfiguration configuration, IPipeline pipeline )
    /// {
    ///     return configuration.GetModifications(pipeline.GetProcessors()).ToPipeline();
    /// }
    /// 
    /// </example>
    public interface IModificationConfiguration 
    {
        /// <summary>
        /// Returns new collection with applied rules of modification implementation.
        /// </summary>
        /// <param name="processors">
        /// An initial collection of processors to be worked on inside modification implementation.
        /// </param>
        /// <returns>
        /// A new collection of processors with applied rules of modification implementation.
        /// </returns>
        IEnumerable<IProcessor> GetModifications(IEnumerable<IProcessor> processors);
    }
}
