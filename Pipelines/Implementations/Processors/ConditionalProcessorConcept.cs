using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public abstract class ConditionalProcessorConcept<TArgs> : SafeTypeProcessor<TArgs>
    {
        public Predicate<TArgs> Condition { get; }

        protected ConditionalProcessorConcept(Predicate<TArgs> condition)
        {
            Condition = condition;
        }

        public override async Task SafeExecute(TArgs args)
        {
            if (this.Condition(args))
            {
                await this.CustomExecute(args);
            }
        }

        public abstract Task CustomExecute(TArgs arguments);
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