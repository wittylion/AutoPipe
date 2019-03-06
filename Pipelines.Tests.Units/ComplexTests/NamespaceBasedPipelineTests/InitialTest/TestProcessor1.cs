using System.Threading.Tasks;
using Pipelines.Implementations.Processors;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest
{
    [ProcessorOrder(1)]
    public class TestProcessor1 : IProcessor
    {
        public Task Execute(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}