namespace AutoPipe.Modifications
{
    /// <summary>
    /// The derived class should implement a logic
    /// that matches a processor to certain criteria.
    /// </summary>
    public interface IProcessorMatcher
    {
        /// <summary>
        /// A method to match a processor to certain criteria.
        /// </summary>
        /// <param name="processor">
        /// A processor to be matched.
        /// </param>
        /// <returns>
        /// Value indicating whether processor matches to criteria or not.
        /// </returns>
        bool Matches(IProcessor processor);
    }
}
