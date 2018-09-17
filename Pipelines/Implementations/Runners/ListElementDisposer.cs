using System;
using System.Collections.Generic;

namespace Pipelines.Implementations.Runners
{
    internal class ListElementDisposer<TItem> : IDisposable
    {
        private List<TItem> List { get; }
        private TItem Item { get; }

        public ListElementDisposer(List<TItem> list, TItem item)
        {
            List = list;
            Item = item;
        }

        public void Dispose()
        {
            List.Remove(Item);
        }
    }
}