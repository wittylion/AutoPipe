using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Interface that runs a processor.
    /// </summary>
    public interface IProcessorRunner
    {
        /// <summary>
        /// Runs a processor passed by <paramref name="processor"/>.
        /// </summary>
        /// <typeparam name="TArgs">
        /// The type of arguments that has to be passed to the processor.
        /// </typeparam>
        /// <param name="processor">
        /// The processor to be executed.
        /// </param>
        /// <param name="args">
        /// The arguments that has to be passed to the processor.
        /// </param>
        /// <returns>
        /// Returns a promise of the processor execution.
        /// </returns>
        Task Run<TArgs>(IProcessor processor, TArgs args);
    }
}