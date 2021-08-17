using System;
using System.Collections.Generic;

namespace Pipelines.Observable
{
    /// <summary>
    /// Implementation of <see cref="IObservableRunner{T}"/>
    /// that uses list of observers.
    /// </summary>
    /// <typeparam name="TContext">
    /// Type of the context used for observers.
    /// </typeparam>
    public class ObservableConcept<TContext> : IObservableRunner<TContext>
    {
        /// <summary>
        /// List of observers that subscribed to the observable class.
        /// </summary>
        private Lazy<List<IRunnerObserver<TContext>>> Observers { get; } =
            new Lazy<List<IRunnerObserver<TContext>>>();

        /// <inheritdoc cref="IObservableRunner{T}.Subscribe"/>
        public virtual IDisposable Subscribe(IRunnerObserver<TContext> observer)
        {
            if (observer.HasNoValue())
            {
                return NullDisposer.Instance;
            }

            Observers.Value.Add(observer);

            return new ListElementDisposer<IRunnerObserver<TContext>>(Observers.Value, observer);
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
        /// Calls <see cref="IRunnerObserver{T}.OnNext"/> method on
        /// each observer, that has subscribed to this collection.
        /// </summary>
        /// <param name="context">
        /// The context that is used in <see cref="IRunnerObserver{T}"/>.
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
        /// Calls <see cref="IRunnerObserver{T}.OnError"/> method on
        /// each observer, that has subscribed to this collection.
        /// </summary>
        /// <param name="exception">
        /// The exception that is used in <see cref="IRunnerObserver{T}"/>.
        /// </param>
        public virtual void OnError(Exception exception, TContext context)
        {
            if (this.HasObservers())
            {
                foreach (var observer in Observers.Value)
                {
                    observer.OnError(exception, context);
                }
            }
        }

        /// <summary>
        /// Calls <see cref="IRunnerObserver{T}.OnCompleted"/> method on
        /// each observer, that has subscribed to this collection.
        /// </summary>
        public virtual void OnCompleted(TContext context)
        {
            if (this.HasObservers())
            {
                foreach (var observer in Observers.Value)
                {
                    observer.OnCompleted(context);
                }
            }
        }
    }
}