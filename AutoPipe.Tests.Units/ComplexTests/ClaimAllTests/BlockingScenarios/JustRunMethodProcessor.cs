namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.Implementations
{
    public class JustRunMethodProcessor
    {
        [Run]
        [Strict]
        public object CheckProperty(string message)
        {
            return new { valid = message != null };
        }
    }
}
