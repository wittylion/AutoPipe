using Pipelines.ExtensionMethods;

namespace Pipelines
{
    public class QueryContext<T> : PipelineContext where T : class
    {
        protected T Result { get; set; }

        public T GetResult()
        {
            return this.Result;
        }

        public T GetResultOr(T fallbackValue)
        {
            return this.Result.Ensure(fallbackValue);
        }

        public void SetResultWithInformation(T result, string message)
        {
            this.Result = result;
            this.AddInformation(message);
        }

        public void SetResultWithWarning(T result, string message)
        {
            this.Result = result;
            this.AddWarning(message);
        }

        public void SetResultWithError(T result, string message)
        {
            this.Result = result;
            this.AddError(message);
        }

        public virtual void AbortPipelineWithErrorAndNoResult(string message)
        {
            this.Result = null;
            this.AbortPipelineWithErrorMessage(message);
        }

        public virtual void AbortPipelineWithWarningAndNoResult(string message)
        {
            this.Result = null;
            this.AbortPipelineWithWarningMessage(message);
        }

        public virtual void AbortPipelineWithInformationAndNoResult(string message)
        {
            this.Result = null;
            this.AbortPipelineWithInformationMessage(message);
        }

        public virtual void ResetResultWithInformation(string message)
        {
            this.Result = null;
            this.AddInformation(message);
        }

        public virtual void ResetResultWithWarning(string message)
        {
            this.Result = null;
            this.AddWarning(message);
        }

        public virtual void ResetResultWithError(string message)
        {
            this.Result = null;
            this.AddError(message);
        }
    }
}