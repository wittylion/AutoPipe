using System;
using System.Threading.Tasks;

namespace Pipelines.Observable
{
    /// <summary>
    /// Processor runner that uses observers to notify about actions.
    /// </summary>
    public class ObservableProcessorRunner : ObservableConcept<ProcessorInfo>, IProcessorRunner
    {
        /// <summary>
        /// A message that is used by constructor to identify that wrapped runner is <c>null</c>.
        /// </summary>
        public static readonly string OriginalRunnerIsNull = "The original processor runner is not specified. Observable processor must wrap an instance of processor runner, otherwise use a default constructor.";

        /// <summary>
        /// An original runner that is wrapped by this class with observers.
        /// </summary>
        public IProcessorRunner OriginalRunner { get; }

        /// <summary>
        /// Default constructor uses <see cref="Runner.StaticInstance"/>.
        /// </summary>
        public ObservableProcessorRunner() : this(Runner.StaticInstance)
        {
        }

        /// <summary>
        /// Constructor that wraps an instance of another processor
        /// runner and uses observers.
        /// </summary>
        /// <param name="originalRunner">
        /// Instance of the runner to be wrapped by this class.
        /// </param>
        public ObservableProcessorRunner(IProcessorRunner originalRunner)
        {
            OriginalRunner = originalRunner ?? throw new ArgumentNullException(ObservableProcessorRunner.OriginalRunnerIsNull);
        }

        /// <inheritdoc cref="IProcessorRunner.Run{TArgs}"/>
        public virtual async Task Run<TArgs>(IProcessor processor, TArgs args)
        {
            var info = new ProcessorInfo()
            {
                Processor = processor,
                Context = args
            };

            this.OnNext(info);

            try
            {
                await this.OriginalRunner.Run(processor, args).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.OnError(exception, info);
            }

            this.OnCompleted(info);
        }
    }
}