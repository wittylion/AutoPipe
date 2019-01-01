using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Runners
{
    /// <summary>
    /// Processor runner that uses observers to notify about actions.
    /// </summary>
    public class ObservableProcessorRunner : ObservableConcept<RunningProcessorObservableInformation>, IProcessorRunner
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
        /// Default constructor uses <see cref="PipelineRunner.StaticInstance"/>.
        /// </summary>
        public ObservableProcessorRunner() : this(PipelineRunner.StaticInstance)
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

        /// <inheritdoc cref="IProcessorRunner.RunProcessor{TArgs}"/>
        public virtual async Task RunProcessor<TArgs>(IProcessor processor, TArgs args)
        {
            if (this.HasObservers())
            {
                var info = new RunningProcessorObservableInformation()
                {
                    Processor = processor,
                    Arguments = args
                };
                this.OnNext(info);
            }

            try
            {
                await this.OriginalRunner.RunProcessor(processor, args);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
            }

            this.OnCompleted();
        }
    }
}