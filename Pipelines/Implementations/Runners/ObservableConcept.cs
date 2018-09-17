using System;
using System.Collections.Generic;
using Pipelines.ExtensionMethods;

namespace Pipelines.Implementations.Runners
{
    public class ObservableConcept<TContext> : IObservable<TContext>
    {
        private Lazy<List<IObserver<TContext>>> Observers { get; } =
            new Lazy<List<IObserver<TContext>>>();

        public virtual IDisposable Subscribe(IObserver<TContext> observer)
        {
            if (observer.HasNoValue())
            {
                return NullDisposer.Instance;
            }

            Observers.Value.Add(observer);

            return new ListElementDisposer<IObserver<TContext>>(Observers.Value, observer);
        }

        public virtual bool HasObservers()
        {
            return Observers.IsValueCreated && Observers.Value.Count > 0;
        }

        public virtual void OnNext(TContext context)
        {
            if (this.HasObservers())
            {
                foreach (var observer in Observers.Value)
                {
                    observer.OnNext(context);
                }
            }
        }

        public virtual void OnError(Exception exception)
        {
            if (this.HasObservers())
            {
                foreach (var observer in Observers.Value)
                {
                    observer.OnError(exception);
                }
            }
        }

        public virtual void OnCompleted()
        {
            if (this.HasObservers())
            {
                foreach (var observer in Observers.Value)
                {
                    observer.OnCompleted();
                }
            }
        }
    }
}