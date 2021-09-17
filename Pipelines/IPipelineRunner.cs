using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Interface that is responsible for running pipeline.
    /// </summary>
    public interface IPipelineRunner
    {
        /// <summary>
        /// Runs processors that are returned by <paramref name="pipeline"/>.
        /// </summary>
        /// <typeparam name="TArgs">
        /// Type of the arguments that are supposed to be used in each processors of the pipeline.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline which processors should be executed.
        /// </param>
        /// <param name="args">
        /// The arguments that has to be passed to each processor
        /// of the executed pipeline.
        /// </param>
        /// <returns>
        /// Returns a promise of the pipeline execution.
        /// </returns>
        Task Run(IPipeline pipeline, Bag args);
    }
}