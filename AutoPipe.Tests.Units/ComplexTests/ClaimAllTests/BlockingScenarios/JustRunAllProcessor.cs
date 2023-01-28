using System;

namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.Implementations
{
    [RunAll(ClaimAllParameters = true)]
    public class JustRunAllProcessor
    {
        public object CheckProperty(string message)
        {
            return new { valid = message != null };
        }
    }
}
