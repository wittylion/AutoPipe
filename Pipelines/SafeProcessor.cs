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
        public override bool SafeCondition(TArgs args)
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

            return base.SafeCondition(args);
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