using System.Threading.Tasks;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest
{
    [Order(0)]
    public class TestZeroProcessor : IProcessor
    {
        public Task Run(Bag arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}