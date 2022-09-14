using System.Threading.Tasks;

namespace AutoPipe
{
    /// <summary>
    /// Represents a unit which can process some information.
    /// You can think of this interface like about actions
    /// in command pattern. It defines and responsible only
    /// for a single <see cref="Run"/> method, which means that
    /// whatever information is passed, it will be somehow processed.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Main method of the processor, which can execute any single action,
        /// that is represented by the name of the processor.
        /// </summary>
        /// <param name="bag">Data that may be processed by this processor.</param>
        /// <returns>A promise class indicating whether operation has completed execution.</returns>
        Task Run(Bag bag);
    }
}