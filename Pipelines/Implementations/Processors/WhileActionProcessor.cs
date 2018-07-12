using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
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

    public class WhileActionProcessor<T> : WhileProcessorConcept<T>
    {
        public Func<T, Task> Action { get; }

        public WhileActionProcessor(Predicate<T> condition, Func<T, Task> action) : base(condition)
        {
            Action = action ?? throw new ArgumentNullException(
                         WhileActionProcessor.ActionMustBeSpecifiedInGenericProcessor);
        }

        public override async Task CustomExecute(T arguments)
        {
            await this.Action(arguments);
        }
    }
}
