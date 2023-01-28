namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.Implementations
{
    [Run(ClaimAllParameters = false)]
    public class RunMethodRequireRunClassNotProcessor
    {
        [Run(ClaimAllParameters = true)]
        public object CheckProperty(string message)
        {
            return new { valid = message != null };
        }
    }
}
