﻿using Castle.Core.Internal;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace AutoPipe.Tests.Units
{
    public class AutoProcessorTests
    {
        [Fact]
        public void GetMethodsToExecute_ShouldReturnEmptyCollection_WhenGetMethodBindingAttributesReturnsNull()
        {
            var mock = new TestAutoProcessor();
            mock.GetFlags = () => null;

            mock.GetMethodsToExecute().Should().BeEmpty();
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
            var processor = new TestAutoProcessor();

            processor.MethodsFilter = m =>
            {
                return m.Name == nameof(TestAutoProcessor.EmptyMethodNotForExecution);
            };

            processor.GetMethodsToExecute()
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
                processor.GetType().GetMethod(nameof(TestOrderOfAutoProcessor.EmptyMethod2)),
                processor.GetType().GetMethod(nameof(TestOrderOfAutoProcessor.EmptyMethod)),
            };

            processor.GetMethodsToExecute().Should().
                    Equal(expected);
        }

        [Fact]
        public void GetOrderOfExecution_ShouldReturnNumberFromExecuteMethodAttribute_WhenItIsSpecified()
        {
            TestAutoProcessor processor = new TestAutoProcessor();
            var method = processor.GetType().GetMethod(nameof(TestAutoProcessor.EmptyMethod2));
            processor.GetOrderOfExecution(method).Should().Be(method.GetAttribute<OrderAttribute>().Order);
        }

        [Fact(Skip = "Api changes.")]
        public void GetOrderOfExecution_ShouldReturnNotThrowExceptionAndReturnZero_WhenMethodDoesntHaveAttribute()
        {
            TestAutoProcessor processor = new TestAutoProcessor();
            var method = processor.GetType().GetMethod(nameof(TestAutoProcessor.EmptyMethodNotForExecution));
            processor.GetOrderOfExecution(method).Should().Be(default);
        }

        [Fact(Skip = "Api changes.")]
        public void GetOrderOfExecution_ShouldReturnNotThrowExceptionAndReturnZero_WhenMethodIsNull()
        {
            TestAutoProcessor processor = new TestAutoProcessor();
            processor.GetOrderOfExecution(null).Should().Be(default);
        }

        [Fact]
        public async Task ProcessResult_ShouldNotThrowException_WhenTaskIsNull()
        {
            TestAutoProcessor processor = new TestAutoProcessor();

            Bag context = new Bag(debug: true);
            Task task = null;

            await processor.ProcessResult(TestAutoProcessor.EmptyMethodInfo, context, task).ConfigureAwait(false);

            context.Should().BeEmpty("because method does not do any action");
        }


        [Fact]
        public async Task ProcessResult_ShouldNotAssignExtraProperties_WhenTaskIsEmpty()
        {
            TestAutoProcessor processor = new TestAutoProcessor();

            Bag context = Bag.Create();
            Task task = Task.CompletedTask;

            await processor.ProcessResult(TestAutoProcessor.EmptyMethodInfo, context, task).ConfigureAwait(false);

            context.Should().BeEmpty();
        }


        [Fact]
        public async Task AutoProcessor_ShouldSkipOtherMethods_WhenOneEnds()
        {
            var processor = new Mock<TestEndingContextParameter>(MockBehavior.Loose) { CallBase = true };

            Bag context = Bag.Create();
            await processor.Object.Run(context).ConfigureAwait(false);

            processor.Verify(x => x.EmptyMethod(It.IsAny<object>()), Times.Never );
            processor.Verify(x => x.EmptyMethod2(), Times.Never );
        }
    }

    public class TestEndingContextParameter : AutoProcessor
    {
        [Run]
        [Order(1)]
        public virtual void EmptyMethod(
            [Required(End = true, Message = "Parameter does not exist.")] object parameter) { }

        [Run]
        [Order(2)]
        public virtual void EmptyMethod2() { }

        public TestEndingContextParameter()
        {
        }
    }

    public class TestAutoProcessor : AutoProcessor
    {
        public static MethodInfo EmptyMethodInfo = typeof(TestAutoProcessor).GetMethod(nameof(EmptyMethod));
        public static MethodInfo EmptyMethod2Info = typeof(TestAutoProcessor).GetMethod(nameof(EmptyMethod2));

        public Func<IEnumerable<BindingFlags>> GetFlags { get; set; }
        public Func<MethodInfo, bool> MethodsFilter { get; set; }

        [Run]
        public void EmptyMethod() { }

        [Run]
        [Order(2)]
        public void EmptyMethod2() { }

        public void EmptyMethodNotForExecution() { }

        public new Task ProcessResult(MethodInfo info, Bag context, object methodResult, bool skip = true)
        {
            return base.ProcessResult(info, context, methodResult, skip);
        }

        protected override IEnumerable<BindingFlags> GetMethodBindingAttributes()
        {
            if (GetFlags != null)
            {
                return GetFlags();
            }

            return base.GetMethodBindingAttributes();
        }

        public override bool AcceptableByFilter(MethodInfo method)
        {
            if (MethodsFilter != null)
            {
                return MethodsFilter(method);
            }

            return base.AcceptableByFilter(method);
        }
    }

    public class TestOrderOfAutoProcessor : AutoProcessor
    {

        [Run]
        [Order(2)]
        public void EmptyMethod2() { }

        [Run]
        [Order(-2)]
        public void EmptyMethodMinus2() { }

        [Run]
        public void EmptyMethod() { }

    }
}
