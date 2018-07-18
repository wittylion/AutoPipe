using Pipelines.ExtensionMethods;

namespace Pipelines
{
    /// <summary>
    /// Implementation of the <see cref="PipelineContext"/> class,
    /// which allows to obtain a result of <see cref="T"/>,
    /// during pipeline execution.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the result, which you want to obtain during the request to the pipeline.
    /// Note, that it is a class type, which means that value can be null,
    /// in case something goes wrong.
    /// </typeparam>
    public class QueryContext<T> : PipelineContext where T : class
    {
        /// <summary>
        /// Result of the pipeline execution.
        /// </summary>
        protected T Result { get; set; }

        /// <summary>
        /// Returns a value of the result property.
        /// </summary>
        /// <remarks>
        /// Can be null. If this property is null, it means that it was not set
        /// or set value was invalid.
        /// </remarks>
        /// <returns>Value of the result property.</returns>
        public T GetResult()
        {
            return this.Result;
        }

        /// <summary>
        /// In case the value of the result is null, you can specify a
        /// <paramref name="fallbackValue"/> which will be returned
        /// instead of the value in result property.
        /// </summary>
        /// <returns>
        /// Value of the result property or <paramref name="fallbackValue"/>
        /// if value of the result is null.
        /// </returns>
        public T GetResultOr(T fallbackValue)
        {
            return this.Result.Ensure(fallbackValue);
        }

        /// <summary>
        /// Provide a result and some information about the result
        /// or about the process of getting this result.
        /// </summary>
        /// <param name="result">
        /// Result object which is obtained during pipeline execution.
        /// </param>
        /// <param name="message">
        /// Provide several words about the result or about the process
        /// of obtaining this result. It will be helpful to understand
        /// why this result was provided.
        /// </param>
        public void SetResultWithInformation(T result, string message)
        {
            this.Result = result;
            this.AddInformation(message);
        }

        /// <summary>
        /// Provide a result and warning message indicating some
        /// problems related to the result or to the process of
        /// getting this result.
        /// </summary>
        /// <param name="result">
        /// Result object which is obtained during pipeline execution.
        /// </param>
        /// <param name="message">
        /// Provide several words about the result or about the process
        /// of obtaining this result. It will be helpful to understand
        /// why this result was provided.
        /// </param>
        public void SetResultWithWarning(T result, string message)
        {
            this.Result = result;
            this.AddWarning(message);
        }

        /// <summary>
        /// Provide a result and error message indicating encountered
        /// problems related to the result or to the process of
        /// getting this result.
        /// </summary>
        /// <param name="result">
        /// Result object which is obtained during pipeline execution.
        /// </param>
        /// <param name="message">
        /// Provide several words about the result or about the process
        /// of obtaining this result. It will be helpful to understand
        /// why this result was provided.
        /// </param>
        public void SetResultWithError(T result, string message)
        {
            this.Result = result;
            this.AddError(message);
        }

        /// <summary>
        /// Executes 3 actions: aborts pipeline, adds error message,
        /// resets result to null.
        /// </summary>
        /// <param name="message">
        /// Error message indicating the reason of the aborted pipeline and no result.
        /// </param>
        public virtual void AbortPipelineWithErrorAndNoResult(string message)
        {
            this.Result = null;
            this.AbortPipelineWithErrorMessage(message);
        }

        /// <summary>
        /// Executes 3 actions: aborts pipeline, adds warning message,
        /// resets result to null.
        /// </summary>
        /// <param name="message">
        /// Warning message indicating the reason of the aborted pipeline and no result.
        /// </param>
        public virtual void AbortPipelineWithWarningAndNoResult(string message)
        {
            this.Result = null;
            this.AbortPipelineWithWarningMessage(message);
        }

        /// <summary>
        /// Executes 3 actions: aborts pipeline, adds information message,
        /// resets result to null.
        /// </summary>
        /// <param name="message">
        /// Information message indicating the reason of the aborted pipeline and no result.
        /// </param>
        public virtual void AbortPipelineWithInformationAndNoResult(string message)
        {
            this.Result = null;
            this.AbortPipelineWithInformationMessage(message);
        }

        /// <summary>
        /// Resets the result to null and adds an information message
        /// describing the reason of the reset result.
        /// </summary>
        /// <param name="message">
        /// Information message describing the reason of the reset result.
        /// </param>
        public virtual void ResetResultWithInformation(string message)
        {
            this.Result = null;
            this.AddInformation(message);
        }

        /// <summary>
        /// Resets the result to null and adds a warning message
        /// describing the reason of the reset result.
        /// </summary>
        /// <param name="message">
        /// Warning message describing the reason of the reset result.
        /// </param>
        public virtual void ResetResultWithWarning(string message)
        {
            this.Result = null;
            this.AddWarning(message);
        }

        /// <summary>
        /// Resets the result to null and adds a error message
        /// describing the reason of the reset result.
        /// </summary>
        /// <param name="message">
        /// Error message describing the reason of the reset result.
        /// </param>
        public virtual void ResetResultWithError(string message)
        {
            this.Result = null;
            this.AddError(message);
        }
    }
}