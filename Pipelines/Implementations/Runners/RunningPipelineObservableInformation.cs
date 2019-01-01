namespace Pipelines.Implementations.Runners
{
    /// <summary>
    /// Class to be used by observer of pipeline runner.
    /// </summary>
    public class RunningPipelineObservableInformation
    {
        /// <summary>
        /// Pipeline that is supposed to be run.
        /// </summary>
        public IPipeline Pipeline { get; set; }

        /// <summary>
        /// Arguments that are used to run the pipeline.
        /// </summary>
        public object Arguments { get; set; }
    }
}