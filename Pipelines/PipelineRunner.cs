using System.Linq;
using System.Threading.Tasks;
using Pipelines.ExtensionMethods;

namespace Pipelines
{
    public class PipelineRunner
    {
        public virtual async Task RunPipeline<TArgs>(IPipeline pipeline, TArgs args)
        {
            if (pipeline.HasNoValue())
            {
                return;
            }

            var processors = pipeline.GetProcessors().Ensure(Enumerable.Empty<IProcessor>());
            foreach (var processor in processors)
            {
                await processor.Execute(args);
            }
        }
    }
}