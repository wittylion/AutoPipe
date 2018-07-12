using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
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

    public class PostActionProcessor<T> : PostProcessorConcept<T>
    {
        public Func<T, Task> Action { get; }

        public PostActionProcessor(IProcessor processor, Func<T, Task> action) : base(processor)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action),
                         PostActionProcessor.ActionMustBeProvidedInGeneric);
        }

        public override async Task CustomExecute(T arguments)
        {
            await this.Action(arguments);
        }
    }
}