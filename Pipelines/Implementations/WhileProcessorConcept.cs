using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations
{
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
                await this.CustomExecute(arguments);
            }
        }

        public abstract Task CustomExecute(object arguments);
    }

    public abstract class WhileProcessorConcept<T> : SafeTypeProcessor<T>
    {
        public Predicate<T> Condition { get; }

        protected WhileProcessorConcept(Predicate<T> condition)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition),
                            WhileProcessorConcept.ConditionMustBeSpecifiedInGenericProcessor);
        }

        public override async Task SafeExecute(T arguments)
        {
            while (this.Condition(arguments))
            {
                await this.CustomExecute(arguments);
            }
        }

        public abstract Task CustomExecute(T arguments);
    }
}