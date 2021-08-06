using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public static class ExecuteActionForPropertyProcessor
    {
        public static readonly string ActionMustBeSpecifiedInGeneric =
            "Creating a generic class used to execute action for property, you have to specify an action which will be executed on each element.";

        public static readonly string PropertyNameMustBeSpecifiedInGeneric =
            "Creating a generic class used to execute action for property, you have to specify property name of the enumerable of elements.";
    }

    public class ExecuteActionForPropertyProcessor<TContext, TProperty> 
        : ExecuteActionForPropertyProcessorConcept<TContext, TProperty> where TContext : Bag
    {
        public string PropertyName { get; }
        public Func<TContext, TProperty, Task> Action { get; }

        public ExecuteActionForPropertyProcessor(Action<TContext, TProperty> action, string propertyName)
            : this(action.ToAsync(), propertyName)
        {
        }

        public ExecuteActionForPropertyProcessor(string propertyName, Action<TContext, TProperty> action)
            : this(action.ToAsync(), propertyName)
        {
        }

        public ExecuteActionForPropertyProcessor(string propertyName, Func<TContext, TProperty, Task> action)
            : this(action, propertyName)
        {
        }

        public ExecuteActionForPropertyProcessor(Func<TContext, TProperty, Task> action, string propertyName)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName),
                               ExecuteActionForPropertyProcessor.PropertyNameMustBeSpecifiedInGeneric);
            Action = action ?? throw new ArgumentNullException(nameof(action),
                         ExecuteActionForPropertyProcessor.ActionMustBeSpecifiedInGeneric);
        }

        public override Task PropertyExecution(TContext args, TProperty property)
        {
            return this.Action(args, property);
        }

        public override string GetPropertyName(TContext args)
        {
            return this.PropertyName;
        }
    }
}