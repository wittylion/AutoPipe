using System.Threading.Tasks;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest
{
    [Order(1)]
    public class TestProcessorOrder1Additional : IProcessor
    {
        public Task Run(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}