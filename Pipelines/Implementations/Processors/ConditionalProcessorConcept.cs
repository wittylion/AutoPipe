using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Abstract processor which is constructed from conditional function, that defines whether action should be executed.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which will be processed by this processor.
    /// </typeparam>
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
                await this.CustomExecute(args).ConfigureAwait(false);
            }
        }

        public abstract Task CustomExecute(TArgs arguments);
    }

    /// <summary>
    /// Abstract processor which is constructed from conditional function, that defines whether action should be executed.
    /// </summary>
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
                await this.CustomExecute(arguments).ConfigureAwait(false);
            }
        }

        public abstract Task CustomExecute(object arguments);
    }
}