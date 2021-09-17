namespace AutoPipe.Observable
{
    /// <summary>
    /// Class to be used by observer of pipeline runner.
    /// </summary>
    public class PipelineInfo
    {
        /// <summary>
        /// Pipeline that is supposed to be run.
        /// </summary>
        public IPipeline Pipeline { get; set; }

        /// <summary>
        /// Arguments that are used to run the pipeline.
        /// </summary>
        public Bag Context { get; set; }
    }
}