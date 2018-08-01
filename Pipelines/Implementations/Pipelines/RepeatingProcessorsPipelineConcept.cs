using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    /// <summary>
    /// Abstract pipeline class that repeats processors, according to the condition.
    /// </summary>
    public abstract class RepeatingProcessorsPipelineConcept : PredefinedPipeline
    {
        protected RepeatingProcessorsPipelineConcept(IEnumerable<IProcessor> processors) : base(processors)
        {
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

        public abstract bool Condition();
    }

    /// <summary>
    /// Abstract pipeline class that repeats processors, according to the condition.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which has to be handled by each processor of this pipeline.
    /// </typeparam>
    public abstract class RepeatingProcessorsPipelineConcept<TArgs> : PredefinedPipeline<TArgs>
    {
        protected RepeatingProcessorsPipelineConcept(IEnumerable<SafeTypeProcessor<TArgs>> processors) : base(processors)
        {
        }

        public override IEnumerable<SafeTypeProcessor<TArgs>> GetProcessorsOfType()
        {
            while (this.Condition())
            {
                foreach (var processor in base.Processors)
                {
                    yield return processor;
                }
            }
        }

        public abstract bool Condition();
    }
}