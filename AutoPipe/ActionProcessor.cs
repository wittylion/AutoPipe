using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoPipe
{
    /// <summary>
    /// Represents a processor that executes a provided action or function.
    /// Inherits from <see cref="Processor"/>, applying all base class rules.
    /// </summary>
    public class ActionProcessor : Processor
    {
        /// <summary>
        /// Error message thrown when no action is supplied to the processor.
        /// </summary>
        public static readonly string ActionMustBeSpecified = "Creating an 'action' processor, you have to provide action which will be executed. Action represented by parameter Func<object, Task>.";

        /// <summary>
        /// Creates an <see cref="ActionProcessor"/> from a parameterless <see cref="Action"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>An instance of <see cref="IProcessor"/>.</returns>
        public static IProcessor From(Action action)
        {
            return new ActionProcessor(action.ToAsync<Bag>());
        }

        /// <summary>
        /// Creates an <see cref="ActionProcessor"/> from an <see cref="Action{Bag}"/>.
        /// </summary>
        /// <param name="action">The action to execute with a <see cref="Bag"/> parameter.</param>
        /// <returns>An instance of <see cref="IProcessor"/>.</returns>
        public static IProcessor From(Action<Bag> action)
        {
            return new ActionProcessor(action.ToAsync());
        }

        /// <summary>
        /// Creates an <see cref="ActionProcessor"/> from a <see cref="Func{Bag, Task}"/>.
        /// </summary>
        /// <param name="action">The asynchronous function to execute with a <see cref="Bag"/> parameter.</param>
        /// <returns>An instance of <see cref="IProcessor"/>.</returns>
        public static IProcessor From(Func<Bag, Task> action)
        {
            return new ActionProcessor(action);
        }

        /// <summary>
        /// Initializes an empty processor with no action assigned.
        /// </summary>
        public ActionProcessor()
        {
        }

        /// <summary>
        /// Initializes a processor with a specified asynchronous action and optional property requirements.
        /// </summary>
        /// <param name="action">The asynchronous action to execute.</param>
        /// <param name="requiredProperties">Properties that must exist in the bag before execution.</param>
        /// <param name="unnecessaryProperties">Properties that must not exist in the bag before execution.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        public ActionProcessor(Func<Bag, Task> action, IEnumerable<string> requiredProperties = null, IEnumerable<string> unnecessaryProperties = null)
        {
            Action = action ?? throw new ArgumentNullException(ActionMustBeSpecified);
            RequiredProperties = requiredProperties;
            UnnecessaryProperties = unnecessaryProperties;
        }

        /// <summary>
        /// The asynchronous action to execute when <see cref="SafeRun(Bag)"/> is called.
        /// </summary>
        protected internal Func<Bag, Task> Action { get; }

        /// <summary>
        /// Properties that must be present in the bag for execution.
        /// </summary>
        protected internal IEnumerable<string> RequiredProperties { get; }

        /// <summary>
        /// Properties that must be absent from the bag for execution.
        /// </summary>
        protected internal IEnumerable<string> UnnecessaryProperties { get; }

        /// <summary>
        /// Executes the assigned action using the provided <see cref="Bag"/>.
        /// If no action is assigned, logs a debug message and completes immediately.
        /// </summary>
        /// <param name="bag">The pipeline context containing properties and messages.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task SafeRun(Bag bag)
        {
            if (this.Action.HasValue())
            {
                return this.Action(bag);
            }

            bag.Debug("Empty processor, no action executed.");
            return PipelineTask.CompletedTask;
        }

        /// <summary>
        /// Returns the collection of required property names, or the base implementation if none are specified.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{String}"/> of required property names.</returns>
        public override IEnumerable<string> MustHaveProperties()
        {
            if (RequiredProperties?.Any() ?? false)
            {
                return RequiredProperties;
            }

            return base.MustHaveProperties();
        }

        /// <summary>
        /// Retrieves a collection of property names that must be excluded from processing.
        /// </summary>
        /// <remarks>If <see cref="UnnecessaryProperties"/> contains any elements, they are returned. 
        /// Otherwise, the result is determined by the base implementation.</remarks>
        /// <returns>An <see cref="IEnumerable{String}"/> representing the property names to exclude. If no properties are
        /// specified, the result from the base implementation is returned.</returns>
        public override IEnumerable<string> MustMissProperties()
        {
            if (UnnecessaryProperties?.Any() ?? false)
            {
                return UnnecessaryProperties;
            }

            return base.MustMissProperties();
        }
    }
}