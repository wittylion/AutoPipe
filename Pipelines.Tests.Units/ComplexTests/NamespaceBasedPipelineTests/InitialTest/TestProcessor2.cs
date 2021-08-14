using System.Threading.Tasks;

namespace Pipelines.Tests.Units.ComplexTests.NamespaceBasedPipelineTests.InitialTest
{
    [Order(2)]
    public class TestProcessor2 : IProcessor
    {
        public Task Run(object arguments)
        {
            return PipelineTask.CompletedTask;
        }
    }
}