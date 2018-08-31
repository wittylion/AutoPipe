﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Pipelines.ExtensionMethods;
using Pipelines.Implementations.Processors;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class ProcessorConstructorTests
    {
        [Fact]
        public async void ExecuteActionForPropertyOrAbort_When_Property_Is_Missing_Should_Abort_With_A_Given_Message()
        {
            var constructor = new ProcessorConstructor();
            var propertyName = nameof(ExecuteActionForPropertyOrAbort_When_Property_Is_Missing_Should_Abort_With_A_Given_Message);
            var abortMessage = "Aborting... Property was not found.";
            var processor = constructor.ExecuteActionForPropertyOrAbort<PipelineContext, string>(
                (x, y) => PipelineTask.CompletedTask, propertyName, abortMessage, MessageType.Error);
            var context = new PipelineContext();

            await processor.Execute(context);

            context.GetErrorMessages()
                .Should()
                .ContainSingle(x => x.Message == abortMessage, "because property is not defined in the context");
        }

        [Fact]
        public async void ExecuteActionForPropertyOrAbort_When_Property_Is_Provided_Should_Execute_A_Given_Action()
        {
            var constructor = new ProcessorConstructor();
            var propertyName = nameof(ExecuteActionForPropertyOrAbort_When_Property_Is_Missing_Should_Abort_With_A_Given_Message);
            var abortMessage = "Aborting... Property was not found.";

            var successProperty = "ExecuteActionForPropertyOrAbortSuccess";
            Action<PipelineContext, string> setSuccessFunction = (pipelineContext, _) =>
            {
                pipelineContext.AddOrSkipPropertyIfExists(successProperty, true);
            };

            var processor = constructor.ExecuteActionForPropertyOrAbort<PipelineContext, string>(
                setSuccessFunction, propertyName, abortMessage, MessageType.Error);
            var context = new PipelineContext();
            context.AddOrSkipPropertyIfExists(propertyName, nameof(ProcessorConstructorTests));

            await processor.Execute(context);

            context.GetPropertyValueOrDefault(successProperty, false)
                .Should()
                .BeTrue("because property is defined in the context and the action must have been set a success property");
        }

        [Fact]
        public async void TryExecuteActionForProperty_When_Property_Is_Provided_And_Exception_Is_Thrown_Should_Execute_An_Exception_Handler()
        {
            var constructor = new ProcessorConstructor();
            var propertyName = nameof(TryExecuteActionForProperty_When_Property_Is_Provided_And_Exception_Is_Thrown_Should_Execute_An_Exception_Handler);


            var exceptionHandlerProperty = "TryExecuteActionForPropertyExceptionHandler";
            Action<PipelineContext> exceptionHandlerFunction = (pipelineContext) =>
            {
                pipelineContext.AddOrSkipPropertyIfExists(exceptionHandlerProperty, true);
            };

            var processor = constructor.TryExecuteActionForProperty<PipelineContext, string>(
                (x, y) => throw new NotImplementedException(), propertyName, exceptionHandlerFunction);
            var context = new PipelineContext();
            context.AddOrSkipPropertyIfExists(propertyName, nameof(ProcessorConstructorTests));

            await processor.Execute(context);

            context.GetPropertyValueOrDefault(exceptionHandlerProperty, false)
                .Should()
                .BeTrue("because property is defined in the context and the action must have been set a success property");
        }
    }
}