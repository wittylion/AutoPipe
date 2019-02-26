using System.Threading.Tasks;
using Pipelines.Implementations.Reflection;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest
{
    [ProcessorOrder(2)]
    public class TestProcessor2 : IProcessor
    {
        public Task Execute(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}