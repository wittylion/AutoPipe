using System.Threading.Tasks;

namespace Pipelines
{
    internal static class PipelineTask
    {
        public static readonly Task CompletedTask = Task.Delay(0);
    }
}