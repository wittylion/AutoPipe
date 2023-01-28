using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoPipe
{
    /// <summary>
    /// Processor constructed from action or function. Inherits <see cref="Processor"/>
    /// so all the rules used in base class will be applied here.
    /// </summary>
    public class ActionProcessor : Processor
    {
        /// <summary>
        /// A message to be thrown when the action is not provided.
        /// </summary>
        public static readonly string ActionMustBeSpecified = "Creating an 'action' processor, you have to provide action which will be executed. Action represented by parameter Func<object, Task>.";

        public static IProcessor From(Action action)
        {
            return new ActionProcessor(action.ToAsync<Bag>());
        }

        public static IProcessor From(Action<Bag> action)
        {
            return new ActionProcessor(action.ToAsync());
        }

        public static IProcessor From(Func<Bag, Task> action)
        {
            return new ActionProcessor(action);
        }

        /// <summary>
        /// Creates an empty processor that has no action.
        /// </summary>
        public ActionProcessor()
        {
        }

        /// <summary>
        /// Creates a processor with an action. Action will be executed
        /// as soon as <see cref="SafeRun(Bag)"/> is called.
        /// </summary>
        /// <param name="action">
        /// An action to be executed during the <see cref="SafeRun(Bag)"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// An exception thrown in case action parameter is null.
        /// </exception>
        public ActionProcessor(Func<Bag, Task> action, IEnumerable<string> requiredProperties = null, IEnumerable<string> unnecessaryProperties = null)
        {
            Action = action ?? throw new ArgumentNullException(ActionMustBeSpecified);
            RequiredProperties = requiredProperties;
            UnnecessaryProperties = unnecessaryProperties;
        }

        /// <summary>
        /// An action to be executed during the <see cref="SafeRun(Bag)"/>.
        /// </summary>
        protected internal Func<Bag, Task> Action { get; }
        protected internal IEnumerable<string> RequiredProperties { get; }
        protected internal IEnumerable<string> UnnecessaryProperties { get; }

        /// <summary>
        /// Executing the action passed via constructor or doing nothing
        /// in case processor was created without an <see cref="Action"/>.
        /// </summary>
        /// <param name="bag">
        /// A bag of properties, messages and bunch of handy methods.
        /// </param>
        /// <returns>
        /// Returns the <see cref="Task"/> that identifies execution result.
        /// </returns>
        public override Task SafeRun(Bag bag)
        {
            if (this.Action.HasValue())
            {
                return this.Action(bag);
            }

            bag.Debug("Empty processor, no action executed.");
            return PipelineTask.CompletedTask;
        }

        public override IEnumerable<string> MustHaveProperties()
        {
            if (RequiredProperties?.Any() ?? false)
            {
                return RequiredProperties;
            }

            return base.MustHaveProperties();
        }

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