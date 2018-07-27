namespace Pipelines
{
    /// <summary>
    /// Implementation of <see cref="SafeTypeProcessor{TArgs}"/>
    /// which is intended to handle <see cref="PipelineContext"/>
    /// type of arguments.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Arguments of type <see cref="PipelineContext"/> or derived,
    /// which will be handled by this processor.
    /// </typeparam>
    public abstract class SafeProcessor<TArgs> : SafeTypeProcessor<TArgs> where TArgs : PipelineContext
    {
        /// <summary>
        /// Additionally to the base class method
        /// <see cref="SafeTypeProcessor{TArgs}.SafeCondition"/>,
        /// checks <see cref="PipelineContext.IsAborted"/> status.
        /// In case it true, the processor should not be executed.
        /// </summary>
        /// <param name="args">
        /// Arguments to be processed.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> in case base condition is true and
        /// <see cref="PipelineContext.IsAborted"/> property is false,
        /// otherwise returns <c>false</c> which means that the processor
        /// should not be executed.
        /// </returns>
        public override bool SafeCondition(TArgs args)
        {
            return base.SafeCondition(args) && !args.IsAborted;
        }
    }

    public abstract class SafeProcessor : SafeTypeProcessor<PipelineContext>
    {
        public override bool SafeCondition(PipelineContext args)
        {
            return base.SafeCondition(args) && !args.IsAborted;
        }
    }
}