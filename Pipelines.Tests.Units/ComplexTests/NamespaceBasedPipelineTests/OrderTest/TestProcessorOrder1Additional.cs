using System.Threading.Tasks;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest
{
    [Order(1)]
    public class TestProcessorOrder1Additional : IProcessor
    {
        public Task Run(Bag arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}