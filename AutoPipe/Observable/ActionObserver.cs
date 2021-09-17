using System;

namespace AutoPipe.Observable
{
    public class ActionObserver<T> : IRunnerObserver<T>
    {
        public Action<T> Next { get; }
        public Action<Exception, T> Error { get; }
        public Action<T> Completed { get; }

        public ActionObserver(Action<T> onNext = null, Action<Exception, T> onError = null, Action<T> onCompleted = null)
        {
            Next = onNext;
            Error = onError;
            Completed = onCompleted;
        }

        public void OnCompleted(T context)
        {
            Completed?.Invoke(context);
        }

        public void OnError(Exception error, T context)
        {
            Error?.Invoke(error, context);
        }

        public void OnNext(T value)
        {
            Next?.Invoke(value);
        }
    }
}
