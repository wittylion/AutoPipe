using System;
using System.Collections.Generic;

namespace Pipelines.Observable
{
    /// <summary>
    /// Implementation of <see cref="IObservable{T}"/>
    /// that uses list of observers.
    /// </summary>
    /// <typeparam name="TContext">
    /// Type of the context used for observers.
    /// </typeparam>
    public class ObservableConcept<TContext> : IObservable<TContext>
    {
        /// <summary>
        /// List of observers that subscribed to the observable class.
        /// </summary>
        private Lazy<List<IObserver<TContext>>> Observers { get; } =
            new Lazy<List<IObserver<TContext>>>();

        /// <inheritdoc cref="IObservable{T}.Subscribe"/>
        public virtual IDisposable Subscribe(IObserver<TContext> observer)
        {
            if (observer.HasNoValue())
            {
                return NullDisposer.Instance;
            }

            Observers.Value.Add(observer);

            return new ListElementDisposer<IObserver<TContext>>(Observers.Value, observer);
        }

        /// <summary>
        /// Returns a value that defines whether this class has observers.
        /// </summary>
        /// <returns>
        /// Value that defines whether this class has observers.
        /// </returns>
        public virtual bool HasObservers()
        {
            return Observers.IsValueCreated && Observers.Value.Count > 0;
        }

        /// <summary>
        /// Calls <see cref="IObserver{T}.OnNext"/> method on
        /// each observer, that has subscribed to this collection.
        /// </summary>
        /// <param name="context">
        /// The context that is used in <see cref="IObserver{T}"/>.
        /// </param>
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

        /// <summary>
        /// Calls <see cref="IObserver{T}.OnError"/> method on
        /// each observer, that has subscribed to this collection.
        /// </summary>
        /// <param name="exception">
        /// The exception that is used in <see cref="IObserver{T}"/>.
        /// </param>
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

        /// <summary>
        /// Calls <see cref="IObserver{T}.OnCompleted"/> method on
        /// each observer, that has subscribed to this collection.
        /// </summary>
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