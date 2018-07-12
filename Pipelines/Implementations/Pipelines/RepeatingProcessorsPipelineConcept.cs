using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
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

    public abstract class RepeatingProcessorsPipelineConcept<T> : PredefinedPipeline<T>
    {
        protected RepeatingProcessorsPipelineConcept(IEnumerable<SafeTypeProcessor<T>> processors) : base(processors)
        {
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

        public abstract bool Condition();
    }
}