using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipelines
{
    public class MemoryCachePipelineWrapper : IPipeline
    {
        public static readonly string PipelineToCacheIsNull = "You have to specify a pipeline of the cache wrapper.";
        public static readonly string RenewConditionIsNull = "The function that calculates whether cache should be updated must not be null.";

        public IPipeline PipelineToCache { get; }
        public Func<IPipeline, IEnumerable<IProcessor>, bool> RenewCondition { get; }
        protected IEnumerable<IProcessor> CachedProcessors { get; set; }

        public MemoryCachePipelineWrapper(IPipeline pipelineToCache, Func<bool> renewCondition, bool useLazyLoading) 
            : this(pipelineToCache, (pipe, _) => renewCondition(), useLazyLoading)
        {
        }

        public MemoryCachePipelineWrapper(IPipeline pipelineToCache, Func<IPipeline, IEnumerable<IProcessor>, bool> renewCondition, bool useLazyLoading)
        {
            PipelineToCache = pipelineToCache ?? throw new ArgumentException(PipelineToCacheIsNull, nameof(pipelineToCache));
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
            if (RenewCondition(PipelineToCache, CachedProcessors))
            {
                CachedProcessors = PipelineToCache.GetProcessors().ToList().AsReadOnly();
            }
        }
    }
}