using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPipe
{
    public class FixedPipeline : IPipeline
    {
        public static readonly string PipelineIsNull = "You have to specify a pipeline of the cache wrapper.";
        public static readonly string RenewConditionIsNull = "The function that calculates whether cache should be updated must not be null.";

        public IPipeline Pipeline { get; }
        public Func<IPipeline, IEnumerable<IProcessor>, bool> RenewCondition { get; }
        protected IEnumerable<IProcessor> CachedProcessors { get; set; }

        public FixedPipeline(IPipeline pipeline, Func<bool> renewCondition, bool useLazyLoading) 
            : this(pipeline, (pipe, _) => renewCondition(), useLazyLoading)
        {
        }

        public FixedPipeline(IPipeline pipeline, Func<IPipeline, IEnumerable<IProcessor>, bool> renewCondition, bool useLazyLoading)
        {
            Pipeline = pipeline ?? throw new ArgumentException(PipelineIsNull, nameof(pipeline));
            RenewCondition = renewCondition ?? throw new ArgumentException(RenewConditionIsNull, nameof(renewCondition));

            if (!useLazyLoading)
            {
                TryUpdatingProcessors();
            }
        }

        public virtual IEnumerable<IProcessor> GetProcessors()
        {
            TryUpdatingProcessors();
            return CachedProcessors;
        }

        protected void TryUpdatingProcessors()
        {
            if (RenewCondition(Pipeline, CachedProcessors))
            {
                CachedProcessors = Pipeline.GetProcessors().ToList().AsReadOnly();
            }
        }
    }
}