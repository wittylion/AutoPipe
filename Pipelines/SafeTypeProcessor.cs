using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Implementation of <see cref="IProcessor"/> that
    /// checks passed arguments,
    /// before executing main logic defined in <see cref="SafeExecute"/>.
    /// It automatically checks that the type of arguments is the same as <see cref="TArgs"/>
    /// and does additional checks defined in <see cref="SafeCondition"/> method.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which is to be processed by this processor.
    /// </typeparam>
    public abstract class SafeTypeProcessor<TArgs> : IProcessor
    {
        protected readonly Task Done = PipelineTask.CompletedTask;

        /// <summary>
        /// Method which will be only executed in case
        /// <paramref name="args"/> parameter passes
        /// all conditional checks.
        /// </summary>
        /// <param name="args">
        /// Arguments to be processed.
        /// </param>
        /// <returns>
        /// Returns a task class which is responsible of asynchronous code execution.
        /// </returns>
        public abstract Task SafeExecute(TArgs args);

        /// <summary>
        /// Method to specify all conditions that will ensure
        /// that execution of main logic can be safely completed.
        /// </summary>
        /// <remarks>
        /// Do all the null, value consistency and access checks in this method.
        /// </remarks>
        /// <param name="args">
        /// Arguments to be processed.
        /// </param>
        /// <returns>
        /// Returns a value defining whether <see cref="SafeExecute"/>
        /// method can be safely completed and will not cause problems.
        /// </returns>
        public virtual bool SafeCondition(TArgs args)
        {
            return true;
        }

        /// <summary>
        /// Executes a logic defined in <see cref="SafeExecute"/>
        /// class only if <paramref name="arguments"/> parameter
        /// is of type <see cref="TArgs"/> and <see cref="SafeCondition"/>
        /// returns true.
        /// </summary>
        /// <remarks>
        /// You can think of this method as of 'if' statement.
        /// </remarks>
        /// <param name="arguments">
        /// Arguments to be processed.
        /// </param>
        /// <returns>
        /// Returns a task class which is responsible of asynchronous code execution.
        /// </returns>
        public Task Execute(object arguments)
        {
            if (!(arguments is TArgs))
            {
                return PipelineTask.CompletedTask;
            }

            if (!SafeCondition((TArgs)arguments))
            {
                return PipelineTask.CompletedTask;
            }

            return SafeExecute((TArgs)arguments);
        }
    }
}