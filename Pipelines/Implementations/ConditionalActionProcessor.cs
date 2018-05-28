using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations
{
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

    public class ConditionalActionProcessor<T> : ConditionalProcessorConcept<T>
    {
        public Func<T, Task> Action { get; }

        public ConditionalActionProcessor(Predicate<T> condition, Func<T, Task> action) : base(condition)
        {
            Action = action ?? throw new ArgumentNullException(
                         ConditionalActionProcessor.ActionMustBeSpecifiedInGenericProcessor);
        }

        public override async Task CustomExecute(T arguments)
        {
            await this.Action(arguments);
        }
    }
}
