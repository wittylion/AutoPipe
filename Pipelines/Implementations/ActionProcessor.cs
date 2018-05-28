using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations
{
    public class ActionProcessor<T> : SafeTypeProcessor<T>
    {
        public static SafeTypeProcessor<T> From(Func<T, Task> action)
        {
            return new ActionProcessor<T>(action);
        }

        public ActionProcessor(Func<T, Task> action)
        {
            Action = action ?? throw new ArgumentNullException(ActionProcessor.ActionMustBeSpecifiedInGenericProcessor);
        }

        private Func<T, Task> Action { get; }

        public override Task SafeExecute(T args)
        {
            return this.Action(args);
        }
    }

    public class ActionProcessor : IProcessor
    {
        public static readonly string ActionMustBeSpecifiedInGenericProcessor = "Creating a generic 'action' processor, you have to provide action which will be executed. Action represented by parameter Func<GenericType, Task>.";
        public static readonly string ActionMustBeSpecified = "Creating an 'action' processor, you have to provide action which will be executed. Action represented by parameter Func<object, Task>.";

        public static IProcessor From(Action<object> action)
        {
            return ActionProcessor.From(args =>
            {
                action(args);
                return Task.CompletedTask;
            });
        }

        public static SafeTypeProcessor<T> From<T>(Action<T> action)
        {
            return ActionProcessor.From<T>(args =>
            {
                action(args);
                return Task.CompletedTask;
            });
        }

        public static IProcessor From(Func<object, Task> action)
        {
            return new ActionProcessor(action);
        }

        public static SafeTypeProcessor<T> From<T>(Func<T, Task> action)
        {
            return new ActionProcessor<T>(action);
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
}