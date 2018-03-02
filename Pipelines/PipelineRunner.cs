using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pipelines
{
    public class PipelineRunner
    {
        public virtual async Task RunPipeline<TArgs>(IPipeline pipeline, TArgs args)
        {
            IEnumerable<IProcessor> processors;
            if (pipeline == null || args == null || (processors = pipeline.GetProcessors()) == null)
            {
                return;
            }

            foreach (var processor in processors)
            {
                await processor.Execute(args);
            }
        }
    }
}