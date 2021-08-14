using System.Collections.Generic;

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
                if (counter == Position && Position != 0)
                {
                    foreach (var newcomer in Newcomers)
                    {
                        yield return newcomer;
                    }
                }

                yield return processor;
                counter++;
            }

            // When the passed position is one more than amount
            // of processors, newcomers should be returned at the end.
            if (counter == Position && counter != 0)
            {
                foreach (var newcomer in Newcomers)
                {
                    yield return newcomer;
                }
            }
        }
    }
}
