using System.Threading.Tasks;
using Pipelines.Implementations.Reflection;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest
{
    [ProcessorOrder(1)]
    public class TestProcessorOrder1 : IProcessor
    {
        public Task Execute(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}