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
        /// <param name="args">
        /// The arguments that has to be passed to the processor.
        /// </param>
        /// <returns>
        /// Returns a promise of the processor execution.
        /// </returns>
        public virtual async Task Run(IProcessor processor, object args)
        {
            if (processor == null)
                return;

            if (!CanRun(processor, args))
            {
                return;
            }

            await processor.Run(args).ConfigureAwait(false);
        }

        public virtual bool CanRun(IProcessor processor, object args)
        {
            return true;
        }
    }
}