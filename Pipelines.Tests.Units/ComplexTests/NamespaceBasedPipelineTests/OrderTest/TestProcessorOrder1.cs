using System.Threading.Tasks;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest
{
    [Order(1)]
    public class TestProcessorOrder1 : IProcessor
    {
        public Task Run(Bag arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}