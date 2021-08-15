using System.Collections.Generic;

namespace Pipelines
{
    /// <summary>
    /// Implementation of <see cref="SafeTypeProcessor{TArgs}"/>
    /// which is intended to handle <see cref="PipelineContext"/>
    /// type of arguments.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Arguments of type <see cref="PipelineContext"/> or derived,
    /// which will be handled by this processor.
    /// </typeparam>
    public abstract class SafeProcessor<TArgs> : SafeTypeProcessor<TArgs> where TArgs : Bag
    {
        /// <summary>
        /// Additionally to the base class method
        /// <see cref="SafeTypeProcessor{TArgs}.SafeCondition"/>,
        /// checks <see cref="PipelineContext.IsAborted"/> status.
        /// In case it true, the processor should not be executed.
        /// </summary>
        /// <param name="args">
        /// Arguments to be processed.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> in case base condition is true and
        /// <see cref="PipelineContext.IsAborted"/> property is false,
        /// otherwise returns <c>false</c> which means that the processor
        /// should not be executed.
        /// </returns>
        public override bool SafeCondition(TArgs args)
        {
            var containProperties = MustHaveProperties();
            foreach (var property in containProperties)
            {
                if (!args.ContainsKey(property))
                {
                    return false;
                }
            }

            var missProperties = MustMissProperties();
            foreach (var property in missProperties)
            {
                if (args.ContainsKey(property))
                {
                    return false;
                }
            }

            return base.SafeCondition(args) && !args.IsAborted;
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

    /// <summary>
    /// Implementation of SafeTypeProcessor with default type of the context
    /// which is intended to handle <see cref="PipelineContext"/>
    /// type of arguments.
    /// </summary>
    public abstract class SafeProcessor : SafeProcessor<Bag>
    {
    }
}