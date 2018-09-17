using System.Threading.Tasks;

namespace Pipelines
{
    public interface IProcessorRunner
    {
        Task RunProcessor<TArgs>(IProcessor processor, TArgs args);
    }
}