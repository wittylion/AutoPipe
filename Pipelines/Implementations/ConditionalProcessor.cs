using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations
{
    public abstract class ConditionalProcessorConcept<T> : SafeTypeProcessor<T>
    {
        public Predicate<T> Condition { get; }

        protected ConditionalProcessorConcept(Predicate<T> condition)
        {
            Condition = condition;
        }

        public override async Task SafeExecute(T args)
        {
            if (this.Condition(args))
            {
                await this.CustomExecute(args);
            }
        }

        public abstract Task CustomExecute(T arguments);
    }

    public abstract class ConditionalProcessorConcept : IProcessor
    {
        public Predicate<object> Condition { get; }

        protected ConditionalProcessorConcept(Predicate<object> condition)
        {
            Condition = condition;
        }

        public async Task Execute(object arguments)
        {
            if (this.Condition(arguments))
            {
                await this.CustomExecute(arguments);
            }
        }

        public abstract Task CustomExecute(object arguments);
    }
}