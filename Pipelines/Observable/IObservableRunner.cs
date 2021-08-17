using System;

namespace Pipelines.Observable
{
    public interface IObservableRunner<in T>
    {
        void OnCompleted(T value);
        void OnError(Exception error, T value);
        void OnNext(T value);
    }
}