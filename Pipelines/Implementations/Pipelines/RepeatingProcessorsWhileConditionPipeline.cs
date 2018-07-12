using System;
using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public class RepeatingProcessorsWhileConditionPipeline : RepeatingProcessorsPipelineConcept
    {
        public static readonly string ConditionMustBeSpecified = "Creating a pipeline that repeats processor, you have to provide condition of the loop.";
        public static readonly string ConditionMustBeSpecifiedGeneric = "Creating a generic pipeline that repeats processor, you have to provide condition of the loop.";

        public Func<bool> CustomCondition { get; }

        public RepeatingProcessorsWhileConditionPipeline(IEnumerable<IProcessor> processors, Func<bool> condition) : base(processors)
        {
            CustomCondition = condition ?? throw new ArgumentNullException(nameof(condition),
                                  RepeatingProcessorsWhileConditionPipeline.ConditionMustBeSpecified);
        }

        public override bool Condition()
        {
            return this.CustomCondition();
        }
    }

    public class RepeatingProcessorsWhileConditionPipeline<T> : RepeatingProcessorsPipelineConcept<T>
    {
        public Func<bool> CustomCondition { get; }

        public RepeatingProcessorsWhileConditionPipeline(IEnumerable<SafeTypeProcessor<T>> processors, Func<bool> condition) : base(processors)
        {
            CustomCondition = condition ?? throw new ArgumentNullException(nameof(condition),
                                  RepeatingProcessorsWhileConditionPipeline.ConditionMustBeSpecifiedGeneric);
        }

        public override bool Condition()
        {
            return this.CustomCondition();
        }
    }
}