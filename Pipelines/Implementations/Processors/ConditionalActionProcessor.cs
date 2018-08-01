using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Processor which constructed from action and condition defining whether action should be executed.
    /// </summary>
    public class ConditionalActionProcessor : ConditionalProcessorConcept
    {
        public static readonly string ActionMustBeSpecifiedInGenericProcessor =
            "Creating a generic 'conditional' processor, that handles action delegate, you have to provide action delegate, represented by Func<GenericType, Task>.";

        public static readonly string ActionMustBeSpecified =
            "Creating a 'conditional' processor, that handles action delegate, you have to provide action delegate, represented by Func<object, Task>.";

        public Func<object, Task> Action { get; }

        public ConditionalActionProcessor(Predicate<object> condition, Func<object, Task> action) : base(condition)
        {
            Action = action ?? throw new ArgumentNullException(ConditionalActionProcessor.ActionMustBeSpecified);
        }

        public override async Task CustomExecute(object arguments)
        {
            await this.Action(arguments);
        }
    }

    /// <summary>
    /// Processor which constructed from action and condition defining whether action should be executed.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which will be processed by this processor.
    /// </typeparam>
    public class ConditionalActionProcessor<TArgs> : ConditionalProcessorConcept<TArgs>
    {
        public Func<TArgs, Task> Action { get; }

        public ConditionalActionProcessor(Predicate<TArgs> condition, Func<TArgs, Task> action) : base(condition)
        {
            Action = action ?? throw new ArgumentNullException(
                         ConditionalActionProcessor.ActionMustBeSpecifiedInGenericProcessor);
        }

        public override async Task CustomExecute(TArgs arguments)
        {
            await this.Action(arguments);
        }
    }
}
