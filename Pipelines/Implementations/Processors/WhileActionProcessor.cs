using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Processor that executes action, while a passed condition returns true.
    /// </summary>
    public class WhileActionProcessor : WhileProcessorConcept
    {
        public static readonly string ActionMustBeSpecifiedInGenericProcessor =
            "Creating a generic 'while' processor, that handles action delegate, you have to provide action delegate, represented by Func<GenericType, Task>.";

        public static readonly string ActionMustBeSpecified =
            "Creating a 'while' processor, that handles action delegate, you have to provide action delegate, represented by Func<object, Task>.";

        public Func<object, Task> Action { get; }

        public WhileActionProcessor(Predicate<object> condition, Func<object, Task> action) : base(condition)
        {
            Action = action ?? throw new ArgumentNullException(WhileActionProcessor.ActionMustBeSpecified);
        }

        public override async Task CustomExecute(object arguments)
        {
            await this.Action(arguments);
        }
    }

    /// <summary>
    /// Processor that executes action, while a passed condition returns true.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which will be processed by this processor.
    /// </typeparam>
    public class WhileActionProcessor<TArgs> : WhileProcessorConcept<TArgs>
    {
        public Func<TArgs, Task> Action { get; }

        public WhileActionProcessor(Predicate<TArgs> condition, Func<TArgs, Task> action) : base(condition)
        {
            Action = action ?? throw new ArgumentNullException(
                         WhileActionProcessor.ActionMustBeSpecifiedInGenericProcessor);
        }

        public override async Task CustomExecute(TArgs arguments)
        {
            await this.Action(arguments);
        }
    }
}
