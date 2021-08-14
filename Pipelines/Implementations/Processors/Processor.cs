using System;
using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Processor constructed from action or function.
    /// </summary>
    public class Processor : IProcessor
    {
        public static readonly string ActionMustBeSpecifiedInGenericProcessor = "Creating a generic 'action' processor, you have to provide action which will be executed. Action represented by parameter Func<GenericType, Task>.";
        public static readonly string ActionMustBeSpecified = "Creating an 'action' processor, you have to provide action which will be executed. Action represented by parameter Func<object, Task>.";

        public static Processor From(Action action)
        {
            return new Processor(action.ToAsync<object>());
        }

        public static Processor<TArgs> From<TArgs>(Action action)
        {
            return new Processor<TArgs>(action.ToAsync<TArgs>());
        }

        public static Processor From(Action<object> action)
        {
            return new Processor(action.ToAsync());
        }

        public static Processor<TArgs> From<TArgs>(Action<TArgs> action)
        {
            return new Processor<TArgs>(action.ToAsync());
        }

        public static Processor From(Func<object, Task> action)
        {
            return new Processor(action);
        }

        public static Processor<TArgs> From<TArgs>(Func<TArgs, Task> action)
        {
            return new Processor<TArgs>(action);
        }

        public Processor(Func<object, Task> action)
        {
            Action = action ?? throw new ArgumentNullException(Processor.ActionMustBeSpecified);
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
    public class Processor<TArgs> : SafeTypeProcessor<TArgs>
    {
        public static SafeTypeProcessor<TArgs> From(Func<TArgs, Task> action)
        {
            return new Processor<TArgs>(action);
        }

        public Processor(Func<TArgs, Task> action)
        {
            Action = action ?? throw new ArgumentNullException(Processor.ActionMustBeSpecifiedInGenericProcessor);
        }

        private Func<TArgs, Task> Action { get; }

        public override Task SafeExecute(TArgs args)
        {
            return this.Action(args);
        }
    }
}