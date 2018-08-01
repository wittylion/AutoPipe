using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Processor with predefined action, that will be executed after the main one.
    /// </summary>
    public class PostActionProcessor : PostProcessorConcept
    {
        public static readonly string ActionMustBeProvided =
            "Creating a post action processor, you have to provide an action, which will be executed after the processor.";

        public static readonly string ActionMustBeProvidedInGeneric =
            "Creating a generic post action processor, you have to provide an action, which will be executed after the processor.";

        public Func<object, Task> Action { get; }

        public PostActionProcessor(IProcessor processor, Func<object, Task> action) : base(processor)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action),
                         PostActionProcessor.ActionMustBeProvided);
        }

        public override async Task CustomExecute(object arguments)
        {
            await this.Action(arguments);
        }
    }

    /// <summary>
    /// Processor with predefined action, that will be executed after the main one.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which will be processed by this processor.
    /// </typeparam>
    public class PostActionProcessor<TArgs> : PostProcessorConcept<TArgs>
    {
        public Func<TArgs, Task> Action { get; }

        public PostActionProcessor(IProcessor processor, Func<TArgs, Task> action) : base(processor)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action),
                         PostActionProcessor.ActionMustBeProvidedInGeneric);
        }

        public override async Task CustomExecute(TArgs arguments)
        {
            await this.Action(arguments);
        }
    }
}