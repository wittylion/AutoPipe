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

        public async Task Execute(object arguments)
        {
            if (!(arguments is T))
            {
                return;
            }

            if (!SafeCondition((T)arguments))
            {
                return;
            }

            await SafeExecute((T)arguments);
        }
    }
}