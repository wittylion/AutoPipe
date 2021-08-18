namespace Pipelines.Observable
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
        public object Context { get; set; }

        public Bag Bag()
        {
            return Args<Bag>();
        }

        public TArgs Args<TArgs>() where TArgs : class
        {
            if (Context is TArgs args)
            {
                return args;
            }

            return default;
        }
    }
}