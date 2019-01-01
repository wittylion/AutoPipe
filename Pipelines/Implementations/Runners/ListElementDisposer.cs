using System;
using System.Collections.Generic;

namespace Pipelines.Implementations.Runners
{
    /// <summary>
    /// An implementation of <see cref="IDisposable"/>
    /// that removes an item from a list.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of the item that has to be removed from the collection.
    /// </typeparam>
    internal class ListElementDisposer<TItem> : IDisposable
    {
        /// <summary>
        /// The list from which an element should be deleted.
        /// </summary>
        private List<TItem> List { get; }

        /// <summary>
        /// The item to be deleted from the list.
        /// </summary>
        private TItem Item { get; }

        /// <summary>
        /// Constructor that takes list from which
        /// an item should be deleted, and item
        /// that has to be removed from the collection.
        /// </summary>
        /// <param name="list">
        /// List to be used when an item removed.
        /// </param>
        /// <param name="item">
        /// An item to be removed from the list when <see cref="Dispose"/>
        /// method is called.
        /// </param>
        public ListElementDisposer(List<TItem> list, TItem item)
        {
            List = list;
            Item = item;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            List.Remove(Item);
        }
    }
}