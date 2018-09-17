using System.Threading.Tasks;

namespace Pipelines
{
    public interface IPipelineRunner
    {
        Task RunPipeline<TArgs>(IPipeline pipeline, TArgs args);
    }
}