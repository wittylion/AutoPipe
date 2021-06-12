using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public static class TryExecuteActionForPropertyProcessor
    {
        public static readonly string ExceptionHandlerMustBeSpecifiedInGeneric =
            "Creating a generic class used to execute action for property and handle exception if it is thrown, you have to specify an action which will be executed after exception is thrown.";
    }

    public class TryExecuteActionForPropertyProcessor<TContext, TProperty>
        : ExecuteActionForPropertyProcessor<TContext, TProperty> where TContext : Bag
    {
        public Action<TContext, TProperty, Exception> ExceptionHandler { get; }

        public TryExecuteActionForPropertyProcessor(Func<TContext, TProperty, Task> action, string propertyName, Action<Exception> exceptionHandler)
            : this(action, propertyName, (context, property, exception) => exceptionHandler(exception))
        {
        }

        public TryExecuteActionForPropertyProcessor(Func<TContext, TProperty, Task> action, string propertyName, Action<TContext> exceptionHandler)
            : this(action, propertyName, (context, property, exception) => exceptionHandler(context))
        {
        }

        public TryExecuteActionForPropertyProcessor(Func<TContext, TProperty, Task> action, string propertyName, Action<TContext, Exception> exceptionHandler)
            : this(action, propertyName, (context, property, exception) => exceptionHandler(context, exception))
        {
        }

        public TryExecuteActionForPropertyProcessor(Func<TContext, TProperty, Task> action, string propertyName, Action<TContext, TProperty, Exception> exceptionHandler)
            : base(action, propertyName)
        {
            ExceptionHandler = exceptionHandler 
                               ?? throw new ArgumentNullException(
                                   TryExecuteActionForPropertyProcessor.ExceptionHandlerMustBeSpecifiedInGeneric);
        }

        public override async Task PropertyExecution(TContext args, TProperty property)
        {
            try
            {
                await this.Action(args, property).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                ExceptionHandler(args, property, e);
            }
        }

        public override string GetPropertyName(TContext args)
        {
            return this.PropertyName;
        }
    }
}