using System.Threading.Tasks;

namespace Pipelines
{
    public abstract class SafeProcessor<T> : IProcessor
    {
        public abstract Task SafeExecute(T args);

        public virtual bool SafeCondition(T args)
        {
            return true;
        }

        public Task Execute(object arguments)
        {
            if (!(arguments is T))
            {
                return Task.CompletedTask;
            }

            if (!SafeCondition((T)arguments))
            {
                return Task.CompletedTask;
            }

            return SafeExecute((T)arguments);
        }
    }
}