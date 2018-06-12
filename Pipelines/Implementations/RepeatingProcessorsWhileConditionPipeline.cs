using System;
using System.Collections.Generic;

namespace Pipelines.Implementations
{
    public class RepeatingProcessorsWhileConditionPipeline : PredefinedPipeline
    {
        public static readonly string ConditionMustBeSpecified = "Creating a pipeline that repeats processor, you have to provide condition of the loop.";
        public static readonly string ConditionMustBeSpecifiedGeneric = "Creating a generic pipeline that repeats processor, you have to provide condition of the loop.";

        public Func<bool> Condition { get; }

        public RepeatingProcessorsWhileConditionPipeline(IEnumerable<IProcessor> processors, Func<bool> condition) : base(processors)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition),
                            RepeatingProcessorsWhileConditionPipeline.ConditionMustBeSpecified);
        }

        public override IEnumerable<IProcessor> GetProcessors()
        {
            while (this.Condition())
            {
                foreach (var processor in base.Processors)
                {
                    yield return processor;
                }
            }
        }
    }

    public class RepeatingProcessorsWhileConditionPipeline<T> : PredefinedPipeline<T>
    {
        public Func<bool> Condition { get; }

        public RepeatingProcessorsWhileConditionPipeline(IEnumerable<SafeTypeProcessor<T>> processors, Func<bool> condition) : base(processors)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition),
                            RepeatingProcessorsWhileConditionPipeline.ConditionMustBeSpecifiedGeneric);
        }
        
        public override IEnumerable<SafeTypeProcessor<T>> GetProcessorsOfType()
        {
            while (this.Condition())
            {
                foreach (var processor in base.Processors)
                {
                    yield return processor;
                }
            }
        }
    }
}