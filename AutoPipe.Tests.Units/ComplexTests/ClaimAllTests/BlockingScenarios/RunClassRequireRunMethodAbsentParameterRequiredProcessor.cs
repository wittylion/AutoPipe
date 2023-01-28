namespace AutoPipe.Tests.Units.ComplexTests.ClaimAllTests.Implementations
{
    [RunAll(ClaimAllParameters = true)]
    public class RunAllClass_NoRunMethod_ParameterRequired_Processor
    {
        public object CheckProperty([Required(Message = "The parameter \'Message\' should be passed.")] string message)
        {
            return new { valid = message != null };
        }
    }
}
