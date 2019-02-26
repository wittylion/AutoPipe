using System.Threading.Tasks;
using Pipelines.Implementations.Reflection;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest
{
    [ProcessorOrder(0)]
    public class TestZeroProcessor : IProcessor
    {
        public Task Execute(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}