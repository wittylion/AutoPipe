using System.Threading.Tasks;

namespace AutoPipe.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest
{
    [Order(2)]
    public class TestProcessor2 : IProcessor
    {
        public Task Run(Bag arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}