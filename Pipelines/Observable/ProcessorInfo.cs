namespace AutoPipe.Observable
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