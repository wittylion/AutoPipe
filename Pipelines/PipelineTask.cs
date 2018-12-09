using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Class that is used in library to extend <see cref="Task"/> mechanism.
    /// </summary>
    internal static class PipelineTask
    {
        /// <summary>
        /// An implementation of the completed task that should be returned in
        /// asynchronous methods when all the instructions completed.
        /// </summary>
        public static readonly Task CompletedTask = Task.Delay(0);
    }
}