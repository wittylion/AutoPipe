using Castle.Core.Internal;
using FluentAssertions;
using Moq;
using Pipelines.Implementations.Contexts;
using Pipelines.Implementations.Processors;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class AutoProcessorTests
    {
        [Fact]
        public void GetMethodsToExecute_ShouldReturnEmptyCollection_WhenGetMethodBindingAttributesReturnsNull()
        {
            Mock<AutoProcessor> mock = new Mock<AutoProcessor>();
            mock.Setup(x => x.GetMethodBindingAttributes()).Returns(() => null);

            mock.Object.GetMethodsToExecute().Should().BeEmpty();
        }

        [Fact]
        public void GetMethodsToExecute_ShouldReturnNonEmptyCollection_WhenDescendantClassContainsImplementations()
        {
            TestAutoProcessor processor = new TestAutoProcessor();

            processor.GetMethodsToExecute().Should().NotBeEmpty().And.Contain(x => x.Name == nameof(TestAutoProcessor.EmptyMethod));
        }

        [Fact]
        public void AcceptableByFilter_ShouldAcceptMethod_WhenMethodHasExecuteMethodAttribute()
        {
            TestAutoProcessor processor = new TestAutoProcessor();
            processor.AcceptableByFilter(processor.GetType().GetMethod(nameof(TestAutoProcessor.EmptyMethod)))
                .Should().BeTrue();
        }

        [Fact]
        public void AcceptableByFilter_ShouldNotThrowExceptionAndBeNotAcceptable_WhenMethodIsNull()
        {
            TestAutoProcessor processor = new TestAutoProcessor();
            processor.AcceptableByFilter(null).Should().BeFalse();
        }

        [Fact]
        public void AcceptableByFilter_ShouldNotAcceptMethod_WhenMethodHasNoExecuteMethodAttribute()
        {
            TestAutoProcessor processor = new TestAutoProcessor();
            processor.AcceptableByFilter(processor.GetType().GetMethod(nameof(TestAutoProcessor.EmptyMethodNotForExecution)))
                .Should().BeFalse();
        }

        [Fact]
        public void GetMethodsToExecute_ShouldReturnDifferentMethods_WhenAcceptableByFilterMethodIsOverriden()
        {
            var processor = new Mock<TestAutoProcessor>(MockBehavior.Loose) { CallBase = true };
            processor.Setup(x => x.AcceptableByFilter(It.IsAny<MethodInfo>())).Returns(() => false);
            processor.Setup(x => x.AcceptableByFilter(It.Is<MethodInfo>(m => m.Name == nameof(TestAutoProcessor.EmptyMethodNotForExecution)))).Returns(() => true);

            processor.Object.GetMethodsToExecute()
                .Should().HaveCount(1)
                .And.Contain(x => x.Name == nameof(TestAutoProcessor.EmptyMethodNotForExecution))
                .And.NotContain(x => x.Name == nameof(TestAutoProcessor.EmptyMethod));
        }

        [Fact]
        public void GetMethodsToExecute_ShouldReturnMethodsInSpecificOrder_WhenOneMethodHasExplicitOrderAndOtherIsNot()
        {
            TestOrderOfAutoProcessor processor = new TestOrderOfAutoProcessor();
            var expected = new[]
            {
                processor.GetType().GetMethod(nameof(TestOrderOfAutoProcessor.EmptyMethodMinus2)),
                processor.GetType().GetMethod(nameof(TestOrderOfAutoProcessor.EmptyMethod)),
                processor.GetType().GetMethod(nameof(TestOrderOfAutoProcessor.EmptyMethod2)),
            };

            processor.GetMethodsToExecute().Should().
                    Equal(expected);
        }

        [Fact]
        public void GetOrderOfExecution_ShouldReturnNumberFromExecuteMethodAttribute_WhenItIsSpecified()
        {
            TestAutoProcessor processor = new TestAutoProcessor();
            var method = processor.GetType().GetMethod(nameof(TestAutoProcessor.EmptyMethod2));
            processor.GetOrderOfExecution(method).Should().Be(method.GetAttribute<ExecuteMethodAttribute>().Order);
        }

        [Fact]
        public void GetOrderOfExecution_ShouldReturnNotThrowExceptionAndReturnZero_WhenMethodDoesntHaveAttribute()
        {
            TestAutoProcessor processor = new TestAutoProcessor();
            var method = processor.GetType().GetMethod(nameof(TestAutoProcessor.EmptyMethodNotForExecution));
            processor.GetOrderOfExecution(method).Should().Be(default);
        }

        [Fact]
        public void GetOrderOfExecution_ShouldReturnNotThrowExceptionAndReturnZero_WhenMethodIsNull()
        {
            TestAutoProcessor processor = new TestAutoProcessor();
            processor.GetOrderOfExecution(null).Should().Be(default);
        }

        [Fact]
        public async Task ProcessResult_ShouldNotThrowException_WhenTaskIsNull()
        {
            TestAutoProcessor processor = new TestAutoProcessor();

            Bag context = Bag.Create();
            Task task = null;

            await processor.ProcessResult(context, task).ConfigureAwait(false);

            context.GetAllPropertyObjects().Should().BeEmpty();
        }


        [Fact]
        public async Task ProcessResult_ShouldNotAssignExtraProperties_WhenTaskIsEmpty()
        {
            TestAutoProcessor processor = new TestAutoProcessor();

            Bag context = Bag.Create();
            Task task = Task.CompletedTask;

            await processor.ProcessResult(context, task).ConfigureAwait(false);

            context.GetAllPropertyObjects().Should().BeEmpty();
        }


        [Fact]
        public async Task AutoProcessor_ShouldSkipOtherMethods_WhenOneAborts()
        {
            var processor = new Mock<TestAbortingContextParameter>(MockBehavior.Loose) { CallBase = true };

            Bag context = Bag.Create();
            await processor.Object.Execute(context).ConfigureAwait(false);

            processor.Verify(x => x.EmptyMethod(It.IsAny<object>()), Times.Never );
            processor.Verify(x => x.EmptyMethod2(), Times.Never );
        }
    }

    public class TestAbortingContextParameter : AutoProcessor
    {
        [ExecuteMethod(Order = 1)]
        public virtual void EmptyMethod([ContextParameter(AbortIfNotExist = true, ErrorMessage = "Parameter does not exist.")] object parameter) { }

        [ExecuteMethod(Order = 2)]
        public virtual void EmptyMethod2() { }

        public TestAbortingContextParameter()
        {
        }
    }

    public class TestAutoProcessor : AutoProcessor
    {
        [ExecuteMethod]
        public void EmptyMethod() { }

        [ExecuteMethod(Order = 2)]
        public void EmptyMethod2() { }

        public void EmptyMethodNotForExecution() { }

        public new Task ProcessResult(Bag context, object methodResult)
        {
            return base.ProcessResult(context, methodResult);
        }
    }

    public class TestOrderOfAutoProcessor : AutoProcessor
    {

        [ExecuteMethod(Order = 2)]
        public void EmptyMethod2() { }

        [ExecuteMethod(Order = -2)]
        public void EmptyMethodMinus2() { }

        [ExecuteMethod]
        public void EmptyMethod() { }

    }
}
