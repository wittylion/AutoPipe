using System;

namespace Pipelines.Modifications
{
    /// <summary>
    /// A simple name for a class helping to get started quickly with modifications.
    /// </summary>
    /// <example>
    /// 
    /// var mod = Modification.Configure(col => col.Remove<DebugProcessor>());
    /// pipeline.Modify(mod);
    /// 
    /// </example>
    public static class Modification
    {
        /// <summary>
        /// Generates a new <see cref="ChainingModification"/> class,
        /// that helps quickly set up modification rules.
        /// </summary>
        /// <returns>
        /// New <see cref="ChainingModification"/> class.
        /// </returns>
        public static ChainingModification Configure()
        {
            return new ChainingModification();
        }

        /// <summary>
        /// Generates a new <see cref="ChainingModification"/> class,
        /// and immediately applies configurations provided in <paramref name="configurators"/>.
        /// </summary>
        /// <param name="configurators">
        /// Configurations that will be immediately applied to the new <see cref="ChainingModification"/> class.
        /// </param>
        /// <returns>
        /// New <see cref="ChainingModification"/> class with applied to it
        /// configurations provided in <paramref name="configurators"/>.
        /// </returns>
        public static ChainingModification Configure(params Action<ChainingModification>[] configurators)
        {
            var result = new ChainingModification();

            if (configurators.IsNotNull())
            {
                foreach (var configurator in configurators)
                {
                    configurator(result);
                }
            }

            return result;
        }
    }
}
