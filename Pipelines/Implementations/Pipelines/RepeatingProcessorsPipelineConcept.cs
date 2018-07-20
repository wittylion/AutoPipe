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