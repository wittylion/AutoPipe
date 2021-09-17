using System.Threading.Tasks;

namespace AutoPipe
{
    /// <summary>
    /// Simple executor of <see cref="IProcessor"/> instances.
    /// </summary>
    public class ProcessorRunner : IProcessorRunner
    {
        /// <summary>
        /// Default instance of the <see cref="ProcessorRunner"/>.
        /// </summary>
        public static readonly IProcessorRunner Instance = new ProcessorRunner();

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
        /// <param name="bag">
        /// The arguments that has to be passed to the processor.
        /// </param>
        /// <returns>
        /// Returns a promise of the processor execution.
        /// </returns>
        public virtual async Task Run(IProcessor processor, Bag bag)
        {
            if (processor == null)
                return;

            if (!CanRun(processor, bag))
            {
                return;
            }

            await processor.Run(bag).ConfigureAwait(false);
        }

        public virtual bool CanRun(IProcessor processor, Bag bag)
        {
            return true;
        }
    }
}