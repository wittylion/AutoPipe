using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public static class ExecuteForEachElementInPropertyProcessor
    {
        public static readonly string ActionMustBeSpecifiedInGeneric =
            "Creating a generic class used to execute action for each element, you have to specify an action which will be executed on each element.";

        public static readonly string PropertyNameMustBeSpecifiedInGeneric =
            "Creating a generic class used to execute action for each element, you have to specify property name of the enumerable of elements.";
    }

    public class ExecuteForEachElementInPropertyProcessor<TElement> : ExecuteForEachElementInPropertyProcessor<PipelineContext, TElement>
    {
        public ExecuteForEachElementInPropertyProcessor(Action<TElement> action, string propertyName)
            : base(action, propertyName)
        {
        }

        public ExecuteForEachElementInPropertyProcessor(Action<PipelineContext, TElement> action, string propertyName) 
            : base(action, propertyName)
        {
        }
    }

    public class ExecuteForEachElementInPropertyProcessor<TContext, TElement> : 
        ExecuteForEachElementInPropertyProcessorConcept<TContext, TElement> where TContext : PipelineContext
    {
        private readonly Action<TContext, TElement> _action;
        private readonly string _propertyName;

        public ExecuteForEachElementInPropertyProcessor(Action<TElement> action, string propertyName)
            : this((context, element) => action(element), propertyName)
        {
        }

        public ExecuteForEachElementInPropertyProcessor(Action<TContext, TElement> action, string propertyName)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action),
                          ExecuteForEachElementInPropertyProcessor.ActionMustBeSpecifiedInGeneric);

            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName),
                                ExecuteForEachElementInPropertyProcessor.PropertyNameMustBeSpecifiedInGeneric);
        }

        public override Task ElementExecution(TContext context, TElement element)
        {
            this._action(context, element);
            return Task.CompletedTask;
        }

        public override string GetPropertyName(TContext context)
        {
            return this._propertyName;
        }
    }
}