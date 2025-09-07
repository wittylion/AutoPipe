using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoPipe
{
    /// <summary>
    /// Abstract base processor for handling pipeline context of type <see cref="Bag"/>.
    /// Provides core logic for safe execution and property validation.
    /// </summary>
    public abstract class Processor : IProcessor
    {
        /// <summary>
        /// Represents a completed task, used for early exits or no-op scenarios.
        /// </summary>
        protected readonly Task Done = PipelineTask.CompletedTask;

        /// <summary>
        /// Provides an empty processor instance for testing or placeholder use. Does nothing but applies <see cref="SafeProcessor"/> rules.
        /// </summary>
        public static readonly IProcessor Empty = new ActionProcessor();

        /// <summary>
        /// Executes the processor's main logic if all safety checks pass.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="bag">Pipeline context containing properties and messages.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task SafeRun(Bag bag);

        /// <summary>
        /// Handles cases where the provided arguments do not pass safety checks.
        /// Can be overridden to perform logging or other actions for invalid input.
        /// </summary>
        /// <param name="arguments">The pipeline context that failed safety validation.</param>
        public virtual void ProcessUnsafeArguments(Bag arguments)
        {
        }

        /// <summary>
        /// Executes <see cref="SafeRun"/> only if <paramref name="bag"/> passes all safety conditions.
        /// Otherwise, performs unsafe argument handling and returns a completed task.
        /// </summary>
        /// <param name="bag">Pipeline context to process.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task Run(Bag bag)
        {
            if (bag == null)
            {
                return Done;
            }

            if (!SafeCondition(bag))
            {
                ProcessUnsafeArguments(bag);
                return Done;
            }

            return SafeRun(bag);
        }

        /// <summary>
        /// Determines whether the processor should execute based on the state of <see cref="Bag"/> and required/missing properties.
        /// Skips execution if the pipeline is ended or property requirements are not met.
        /// </summary>
        /// <param name="bag">Pipeline context to validate.</param>
        /// <returns><c>true</c> if all conditions are met and processor should execute; otherwise, <c>false</c>.</returns>
        public virtual bool SafeCondition(Bag bag)
        {
            if (bag.Ended)
            {
                bag.Debug("The bag contained end property set to True. Skipping processor.");
                return false;
            }

            var containProperties = MustHaveProperties();
            foreach (var property in containProperties)
            {
                if (!bag.ContainsKey(property))
                {
                    if (bag.Debug)
                    {
                        var processorName = this.Name();
                        bag.Debug("The bag misses property [{0}]. Skipping processor [{1}].".FormatWith(property, processorName));
                    }

                    return false;
                }
            }

            var missProperties = MustMissProperties();
            foreach (var property in missProperties)
            {
                if (bag.ContainsKey(property))
                {
                    if (bag.Debug)
                    {
                        var processorName = this.Name();
                        bag.Debug("The bag should not contain property [{0}]. Skipping processor [{1}].".FormatWith(property, processorName));
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a collection of property names that must be present in the pipeline context for execution.
        /// Override in derived classes to specify required properties.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{String}"/> of required property names.</returns>
        public virtual IEnumerable<string> MustHaveProperties()
        {
            yield break;
        }

        /// <summary>
        /// Returns a collection of property names that must be absent from the pipeline context for execution.
        /// Override in derived classes to specify excluded properties.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{String}"/> of property names to exclude.</returns>
        public virtual IEnumerable<string> MustMissProperties()
        {
            yield break;
        }
    }
}