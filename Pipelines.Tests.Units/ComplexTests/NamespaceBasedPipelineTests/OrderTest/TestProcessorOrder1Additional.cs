using System.Threading.Tasks;
using Pipelines.Implementations.Reflection;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.OrderTest
{
    [ProcessorOrder(1)]
    public class TestProcessorOrder1Additional
    {
        public Task Execute(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}