using System.Threading.Tasks;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest
{
    [Order(0)]
    public class TestZeroProcessor : IProcessor
    {
        public Task Run(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}