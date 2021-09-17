using System;

namespace AutoPipe.Modifications
{
    public class ProcessorMatcherDisjunction : IProcessorMatcher
    {
        public ProcessorMatcherDisjunction(IProcessorMatcher left, IProcessorMatcher right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left), "You have to specify a first processor matcher before using it in disjunction operator.");
            Right = right ?? throw new ArgumentNullException(nameof(left), "You have to specify a second processor matcher before using it in disjunction operator."); ;
        }

        public IProcessorMatcher Left { get; }
        public IProcessorMatcher Right { get; }

        public bool Matches(IProcessor processor)
        {
            return Left.Matches(processor) || Right.Matches(processor);
        }
    }
}
