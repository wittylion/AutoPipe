using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Abstract processor with action method, that will be executed after the main one.
    /// </summary>
    public abstract class PostProcessorConcept : IProcessor
    {
        public static readonly string ActualProcessorMustBeProvided =
            "Creating a post processor, you have to provide an actual processor, after which an action will be executed.";

        public static readonly string ActualProcessorMustBeProvidedInGeneric =
            "Creating a generic post processor, you have to provide an actual processor, after which an action will be executed.";

        public IProcessor ActualProcessor { get; }

        protected PostProcessorConcept(IProcessor actualProcessor)
        {
            ActualProcessor = actualProcessor ?? throw new ArgumentNullException(
                                  PostProcessorConcept.ActualProcessorMustBeProvided);
        }

        public async Task Execute(object arguments)
        {
            await this.ActualProcessor.Execute(arguments);
            await this.CustomExecute(arguments);
        }

        public abstract Task CustomExecute(object arguments);
    }

    /// <summary>
    /// Abstract processor with action method, that will be executed after the main one.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which will be processed by this processor.
    /// </typeparam>
    public abstract class PostProcessorConcept<TArgs> : SafeTypeProcessor<TArgs>
    {
        public IProcessor ActualProcessor { get; }

        protected PostProcessorConcept(IProcessor actualProcessor)
        {
            ActualProcessor = actualProcessor ?? throw new ArgumentNullException(
                            PostProcessorConcept.ActualProcessorMustBeProvidedInGeneric);
        }

        public override async Task SafeExecute(TArgs arguments)
        {
            await this.ActualProcessor.Execute(arguments);
            await this.CustomExecute(arguments);
        }

        public abstract Task CustomExecute(TArgs arguments);
    }
}