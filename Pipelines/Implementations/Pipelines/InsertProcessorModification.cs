using System;
using System.Collections.Generic;
using System.Text;

namespace Pipelines.Implementations.Pipelines
{
    public class InsertProcessorModification : IModificationConfiguration
    {
        public InsertProcessorModification(int position, IEnumerable<IProcessor> newcomers)
        {
            Position = position;
            Newcomers = newcomers;
        }

        public int Position { get; }
        public IEnumerable<IProcessor> Newcomers { get; }

        public IEnumerable<IProcessor> GetModifications(IEnumerable<IProcessor> processors)
        {
            var counter = 0;
            
            if (Position == 0)
            {
                foreach (var newcomer in Newcomers)
                {
                    yield return newcomer;
                }
            }

            foreach (var processor in processors)
            {
                if (counter == Position)
                {
                    foreach (var newcomer in Newcomers)
                    {
                        yield return newcomer;
                    }
                }

                yield return processor;
                counter++;
            }

            if (counter == Position)
            {
                foreach (var newcomer in Newcomers)
                {
                    yield return newcomer;
                }
            }
        }
    }
}
