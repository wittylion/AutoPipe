using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Abstract processor that executes action, while condition method returns true.
    /// </summary>
    public abstract class WhileProcessorConcept : IProcessor
    {
        public static readonly string ConditionMustBeSpecifiedInGenericProcessor = "Creating a generic 'while' processor, you have to provide condition of the loop.";
        public static readonly string ConditionMustBeSpecified = "Creating a 'while' processor, you have to provide condition of the loop.";
        public Predicate<object> Condition { get; }

        protected WhileProcessorConcept(Predicate<object> condition)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition),
                            WhileProcessorConcept.ConditionMustBeSpecified);
        }

        public async Task Execute(object arguments)
        {
            while (Condition(arguments))
            {
                await this.CustomExecute(arguments).ConfigureAwait(false);
            }
        }

        public abstract Task CustomExecute(object arguments);
    }

    /// <summary>
    /// Abstract processor that executes action, while condition method returns true.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which will be processed by this processor.
    /// </typeparam>
    public abstract class WhileProcessorConcept<TArgs> : SafeTypeProcessor<TArgs>
    {
        public Predicate<TArgs> Condition { get; }

        protected WhileProcessorConcept(Predicate<TArgs> condition)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition),
                            WhileProcessorConcept.ConditionMustBeSpecifiedInGenericProcessor);
        }

        public override async Task SafeExecute(TArgs arguments)
        {
            while (this.Condition(arguments))
            {
                await this.CustomExecute(arguments).ConfigureAwait(false);
            }
        }

        public abstract Task CustomExecute(TArgs arguments);
    }
}