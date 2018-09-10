using System;
using System.Threading.Tasks;
using Pipelines.ExtensionMethods;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Processor constructed from action or function.
    /// </summary>
    public class ActionProcessor : IProcessor
    {
        public static readonly string ActionMustBeSpecifiedInGenericProcessor = "Creating a generic 'action' processor, you have to provide action which will be executed. Action represented by parameter Func<GenericType, Task>.";
        public static readonly string ActionMustBeSpecified = "Creating an 'action' processor, you have to provide action which will be executed. Action represented by parameter Func<object, Task>.";

        public static ActionProcessor FromAction(Action action)
        {
            return new ActionProcessor(action.ToAsync<object>());
        }

        public static ActionProcessor<TArgs> FromAction<TArgs>(Action action)
        {
            return new ActionProcessor<TArgs>(action.ToAsync<TArgs>());
        }

        public static ActionProcessor FromAction(Action<object> action)
        {
            return new ActionProcessor(action.ToAsync());
        }

        public static ActionProcessor<TArgs> FromAction<TArgs>(Action<TArgs> action)
        {
            return new ActionProcessor<TArgs>(action.ToAsync());
        }

        public static ActionProcessor FromAction(Func<object, Task> action)
        {
            return new ActionProcessor(action);
        }

        public static ActionProcessor<TArgs> FromAction<TArgs>(Func<TArgs, Task> action)
        {
            return new ActionProcessor<TArgs>(action);
        }

        public ActionProcessor(Func<object, Task> action)
        {
            Action = action ?? throw new ArgumentNullException(ActionProcessor.ActionMustBeSpecified);
        }

        private Func<object, Task> Action { get; }

        public Task Execute(object arguments)
        {
            return this.Action(arguments);
        }
    }

    /// <summary>
    /// Processor constructed from action or function
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which will be processed by this processor.
    /// </typeparam>
    public class ActionProcessor<TArgs> : SafeTypeProcessor<TArgs>
    {
        public static SafeTypeProcessor<TArgs> From(Func<TArgs, Task> action)
        {
            return new ActionProcessor<TArgs>(action);
        }

        public ActionProcessor(Func<TArgs, Task> action)
        {
            Action = action ?? throw new ArgumentNullException(ActionProcessor.ActionMustBeSpecifiedInGenericProcessor);
        }

        private Func<TArgs, Task> Action { get; }

        public override Task SafeExecute(TArgs args)
        {
            return this.Action(args);
        }
    }
}