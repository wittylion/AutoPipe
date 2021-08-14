using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Simple executor of <see cref="IProcessor"/> instances.
    /// </summary>
    public class ProcessorRunner : IProcessorRunner
    {
        /// <summary>
        /// Default instance of the <see cref="ProcessorRunner"/>.
        /// </summary>
        public static readonly IProcessorRunner StaticInstance = new ProcessorRunner();

        /// <summary>
        /// Runs a processor by executing its <see cref="IProcessor.Run"/> method.
        /// If processor is null it will be skipped.
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
        public virtual async Task Run<TArgs>(IProcessor processor, TArgs args)
        {
            if (processor == null)
                return;

            await processor.Run(args).ConfigureAwait(false);
        }
    }
}