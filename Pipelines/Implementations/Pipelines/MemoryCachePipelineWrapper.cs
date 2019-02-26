﻿using System;
using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public class MemoryCachePipelineWrapper : IPipeline
    {
        public static readonly string PipelineToCacheIsNull = "You have to specify a pipeline of the cache wrapper.";
        public static readonly string RenewConditionIsNull = "The function that calculates whether cache should be updated must not be null.";

        public IPipeline PipelineToCache { get; }
        public Func<IPipeline, IEnumerable<IProcessor>, bool> RenewCondition { get; }
        protected IEnumerable<IProcessor> CachedProcessors { get; set; }

        public MemoryCachePipelineWrapper(IPipeline pipelineToCache, Func<bool> renewCondition) : this(pipelineToCache, (pipe, _) => renewCondition())
        {
        }

        public MemoryCachePipelineWrapper(IPipeline pipelineToCache, Func<IPipeline, IEnumerable<IProcessor>, bool> renewCondition)
        {
            PipelineToCache = pipelineToCache ?? throw new ArgumentException(PipelineToCacheIsNull, nameof(pipelineToCache));
            RenewCondition = renewCondition ?? throw new ArgumentException(RenewConditionIsNull, nameof(renewCondition));
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            if (RenewCondition(PipelineToCache, CachedProcessors))
            {
                CachedProcessors = PipelineToCache.GetProcessors();
            }

            return CachedProcessors;
        }
    }
}