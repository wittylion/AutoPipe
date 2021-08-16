namespace Pipelines.Observable
{
    /// <summary>
    /// Class to be used by observer of processor runner.
    /// </summary>
    public class ProcessorInfo
    {
        /// <summary>
        /// Processor that is supposed to be run.
        /// </summary>
        public IProcessor Processor { get; set; }

        /// <summary>
        /// Arguments that are used to run the pipeline.
        /// </summary>
        public object Arguments { get; set; }
    }
}