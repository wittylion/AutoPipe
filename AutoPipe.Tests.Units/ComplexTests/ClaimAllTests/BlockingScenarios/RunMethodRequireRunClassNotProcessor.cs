namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.Implementations
{
    [Run]
    public class RunMethodRequireRunClassNotProcessor
    {
        [Run]
        [Strict]
        public object CheckProperty(string message)
        {
            return new { valid = message != null };
        }
    }
}
