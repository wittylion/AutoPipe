using System;
using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Processor constructed from action or function.
    /// </summary>
    public class Processor : IProcessor
    {
        public static readonly string ActionMustBeSpecified = "Creating an 'action' processor, you have to provide action which will be executed. Action represented by parameter Func<object, Task>.";

        public static Processor From(Action action)
        {
            return new Processor(action.ToAsync<object>());
        }

        public static Processor From(Action<Bag> action)
        {
            return new Processor(action.ToAsync());
        }

        public static Processor From(Func<Bag, Task> action)
        {
            return new Processor(action);
        }

        public Processor(Func<Bag, Task> action)
        {
            Action = action ?? throw new ArgumentNullException(Processor.ActionMustBeSpecified);
        }

        private Func<Bag, Task> Action { get; }

        public Task Run(Bag arguments)
        {
            return this.Action(arguments);
        }
    }
}