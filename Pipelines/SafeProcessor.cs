using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Implementation of SafeTypeProcessor with default type of the context
    /// which is intended to handle <see cref="PipelineContext"/>
    /// type of arguments.
    /// </summary>
    public abstract class SafeProcessor : IProcessor
    {
        protected readonly Task Done = PipelineTask.CompletedTask;

        /// <summary>
        /// Method which will be only executed in case
        /// <paramref name="args"/> parameter passes
        /// all conditional checks.
        /// </summary>
        /// <param name="args">
        /// Arguments to be processed.
        /// </param>
        /// <returns>
        /// Returns a task class which is responsible of asynchronous code execution.
        /// </returns>
        public abstract Task SafeRun(Bag args);

        /// <summary>
        /// Executes small action that does some logging in case
        /// arguments object passed to the processor does not pass
        /// safety check. This action is supposed to be brief and
        /// should not take long time to execute.
        /// </summary>
        /// <param name="arguments">
        /// Arguments of the type that this processor has received.
        /// </param>
        public virtual void ProcessUnsafeArguments(Bag arguments)
        {
        }

        /// <summary>
        /// Executes a logic defined in <see cref="SafeRun"/>
        /// class only if <paramref name="arguments"/> parameter
        /// is of type <see cref="TArgs"/> and <see cref="SafeCondition"/>
        /// returns true.
        /// </summary>
        /// <remarks>
        /// You can think of this method as of 'if' statement.
        /// </remarks>
        /// <param name="arguments">
        /// Arguments to be processed.
        /// </param>
        /// <returns>
        /// Returns a task class which is responsible of asynchronous code execution.
        /// </returns>
        public Task Run(Bag arguments)
        {
            if (arguments == null)
            {
                return Done;
            }

            if (!SafeCondition(arguments))
            {
                ProcessUnsafeArguments(arguments);
                return Done;
            }

            return SafeRun(arguments);
        }

        /// <summary>
        /// Additionally to the base class method
        /// <see cref="SafeTypeProcessor{TArgs}.SafeCondition"/>,
        /// checks <see cref="PipelineContext.Ended"/> status.
        /// In case it true, the processor should not be executed.
        /// </summary>
        /// <param name="args">
        /// Arguments to be processed.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> in case base condition is true and
        /// <see cref="PipelineContext.Ended"/> property is false,
        /// otherwise returns <c>false</c> which means that the processor
        /// should not be executed.
        /// </returns>
        public virtual bool SafeCondition(Bag args)
        {
            var containProperties = MustHaveProperties();
            foreach (var property in containProperties)
            {
                if (!args.ContainsKey(property))
                {
                    if (args.Debug)
                    {
                        var processorName = this.Name();
                        args.Debug("The bag misses property [{0}]. Skipping processor [{1}].".FormatWith(property, processorName));
                    }

                    return false;
                }
            }

            var missProperties = MustMissProperties();
            foreach (var property in missProperties)
            {
                if (args.ContainsKey(property))
                {
                    if (args.Debug)
                    {
                        var processorName = this.Name();
                        args.Debug("The bag should not contain property [{0}]. Skipping processor [{1}].".FormatWith(property, processorName));
                    }

                    return false;
                }
            }

            if (args.Ended)
            {
                args.Debug("The bag contained end property set to True. Skipping processor.");
                return false;
            }

            return true;
        }

        public virtual IEnumerable<string> MustHaveProperties()
        {
            yield break;
        }

        public virtual IEnumerable<string> MustMissProperties()
        {
            yield break;
        }
    }
}