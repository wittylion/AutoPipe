using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pipelines.ExtensionMethods;

namespace Pipelines
{
    public class PipelineRunner
    {
        public static readonly PipelineRunner StaticInstance = new PipelineRunner();

        public virtual Task RunPipeline<TArgs>(IPipeline pipeline, TArgs args)
        {
            if (pipeline.HasNoValue())
            {
                return Task.CompletedTask;
            }

            return RunProcessors(pipeline.GetProcessors(), args);
        }

        public virtual async Task RunProcessors<TArgs>(IEnumerable<IProcessor> processors, TArgs args)
        {
            processors = processors.Ensure(Enumerable.Empty<IProcessor>());
            foreach (var processor in processors)
            {
                await RunProcessor(processor, args);
            }
        }

        public virtual async Task RunProcessor<TArgs>(IProcessor processor, TArgs args)
        {
            if (processor.HasValue())
                await processor.Execute(args);
        }
    }
}