using System.Threading.Tasks;
using Pipelines.Implementations.Processors;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest
{
    [ProcessorOrder(1)]
    public class TestProcessorOrder1Additional : IProcessor
    {
        public Task Run(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}