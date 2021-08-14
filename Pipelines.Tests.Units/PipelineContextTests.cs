using System.Linq;
using FluentAssertions;
using Xunit;

namespace Pipelines.Tests.Units
{
    public class PipelineContextTests
    {
        [Fact]
        public void GetMessages_Returns_Information_Messages_When_All_Types_Of_Messages_Are_Added()
        {
            var information = new PipelineMessage(nameof(Bag), MessageType.Information);
            var pipelineContext = new PipelineContextTestObject()
                .WithWarning(nameof(Bag))
                .WithMessage(information)
                .WithError(nameof(Bag));

            pipelineContext.GetMessages(MessageFilter.Informations).Should()
                .HaveCount(1, "only one information message was added")
                .And
                .AllBeEquivalentTo(information, "this message was added to collection");
        }

        [Fact]
        public void AbortMessageWithError_Calls_Abort_Pipeline_Method()
        {
            var pipelineContext = new PipelineContextTestObject();
            pipelineContext.AbortPipelineWithErrorMessage(nameof(Bag));
            pipelineContext.IsAborted.Should().BeTrue("because the method should abort pipeline");
        }

        [Fact]
        public void GetInformationsAndWarnings_Should_Retrieve_Warnings_And_Informations_When_Several_Message_Are_Added()
        {
            var pipelineContext = new PipelineContextTestObject()
                .WithError("Error")
                .WithInformation("Information")
                .WithWarning("Warning");

            pipelineContext.GetInformationsAndWarnings().Should()
                .HaveCount(2, "because three messages were added, where only one of them is error")
                .And
                .Match(collection => collection.All(x => x.MessageType != MessageType.Error), "because collection should contain no Error type");
        }

        [Fact]
        public void GetPropertyValueOrNull_Retrieves_A_Proper_Value()
        {
            var pipelineContext = Bag.Create();
            var expectedValue = nameof(GetPropertyValueOrNull_Retrieves_A_Proper_Value);
            var key = nameof(PipelineContextTests);

            pipelineContext.Set(key, expectedValue);

            pipelineContext.GetOrThrow<string>(key)
                .Should()
                .Be(expectedValue, "because method must return value when it is set");
        }

        [Fact]
        public void GetPropertyValueOrNull_Retrieves_Null_When_Requested_An_Incorrect_Type()
        {
            var pipelineContext = Bag.Create();
            var value = nameof(GetPropertyValueOrNull_Retrieves_Null_When_Requested_An_Incorrect_Type);
            var key = nameof(PipelineContextTests);

            pipelineContext.Set(key, value);

            pipelineContext.Get(key, (IAsyncLifetime) null)
                .Should()
                .BeNull("because method must check a type before value is converted into this type");
        }
        
        [Fact]
        public void GetPropertyValueOrDefault_Retrieves_Default_Value_When_Requested_An_Incorrect_Type()
        {
            var pipelineContext = new Bag();
            var value = nameof(GetPropertyValueOrDefault_Retrieves_Default_Value_When_Requested_An_Incorrect_Type);
            var key = nameof(PipelineContextTests);
            var expectedValue = 3;

            pipelineContext.Set(key, value);

            pipelineContext.Get<int>(key, expectedValue)
                .Should()
                .Be(expectedValue, "because method must check a type before value is converted into this type");
        }

        [Fact]
        public void GetPropertyValueOrNull_Retrieves_A_Proper_Value_Regardless_Name_Case()
        {
            var pipelineContext = new Bag();
            var expectedValue = nameof(GetPropertyValueOrNull_Retrieves_A_Proper_Value_Regardless_Name_Case);
            var key = nameof(PipelineContextTests);

            pipelineContext.Set(key, expectedValue);

            pipelineContext.GetOrThrow<string>(key.ToUpperInvariant())
                .Should()
                .Be(expectedValue, "because method must return value regardless case of the property name");
        }

        [Fact]
        public void ObjectConstructor_Sets_Properties_Of_An_Object_To_Property_Collection()
        {
            var expectedValue = nameof(ObjectConstructor_Sets_Properties_Of_An_Object_To_Property_Collection);
            var pipelineContext = new Bag(new {PipelineContextTests = expectedValue});

            pipelineContext.GetOrThrow<string>("PipelineContextTests")
                .Should()
                .Be(expectedValue, "because method must return value of the property passed in constructor with an object");
        }


        [Fact]
        public void ObjectConstructor_Does_Not_Throw_An_Exception_When_Object_Is_Null()
        {
            var pipelineContext = new Bag(null);
            pipelineContext.Should().NotBeNull();
        }


        [Fact]
        public void GetPropertyValueOrNull_Retrieves_Null_When_Value_Is_Not_Set()
        {
            var pipelineContext = new Bag();
            var expectedValue = nameof(GetPropertyValueOrNull_Retrieves_Null_When_Value_Is_Not_Set);
            var key = nameof(PipelineContextTests);

            pipelineContext.Get(key, (string) null)
                .Should()
                .BeNull(expectedValue, "because the value was not set");
        }

        [Fact]
        public void GetPropertyValueOrDefault_Retrieves_Default_Object_When_Value_Is_Not_Set()
        {
            var pipelineContext = new Bag();
            var expectedValue = nameof(GetPropertyValueOrDefault_Retrieves_Default_Object_When_Value_Is_Not_Set);
            var key = nameof(PipelineContextTests);

            pipelineContext.Get<string>(key, expectedValue)
                .Should()
                .Be(expectedValue, "because method must return the default passed value");
        }

        [Fact]
        public void SetOrAddProperty_Should_Update_Value_When_It_Has_Already_Been_Set()
        {
            var pipelineContext = new Bag();
            var value = 234;
            var expectedValue = nameof(SetOrAddProperty_Should_Update_Value_When_It_Has_Already_Been_Set);
            var key = nameof(PipelineContextTests);

            pipelineContext.Set(nameof(PipelineContextTests), value);
            pipelineContext.Set(nameof(PipelineContextTests), expectedValue);
            
            pipelineContext.GetOrThrow<string>(key)
                .Should()
                .Be(expectedValue, "because method must update a value if it previously was set");
        }

        [Fact]
        public void AddOrSkipPropertyIfExists_Should_Skip_Property_When_It_Has_Already_Been_Set()
        {
            var pipelineContext = new Bag();
            var value = 234;
            var expectedValue = nameof(AddOrSkipPropertyIfExists_Should_Skip_Property_When_It_Has_Already_Been_Set);
            var key = nameof(PipelineContextTests);

            pipelineContext.Set(nameof(PipelineContextTests), expectedValue, skipIfExists: true);
            pipelineContext.Set(nameof(PipelineContextTests), value, skipIfExists: true);

            pipelineContext.GetOrThrow<string>(key)
                .Should()
                .Be(expectedValue, "because method must skip a value if it previously was set");
        }

        [Fact]
        public void DeleteProperty_Should_Not_Throw_Exception_When_Property_Does_Not_Exists()
        {
            var pipelineContext = new Bag();
            var property = nameof(DeleteProperty_Should_Not_Throw_Exception_When_Property_Does_Not_Exists);
            pipelineContext.DeleteProperty(property);
        }

        [Fact]
        public void DeleteProperty_Should_Delete_Property_If_It_Is_Existing()
        {
            var pipelineContext = new Bag();
            var property = nameof(DeleteProperty_Should_Delete_Property_If_It_Is_Existing);
            pipelineContext.Set(property, nameof(PipelineContextTests), skipIfExists: true);

            pipelineContext.DeleteProperty(property);

            pipelineContext.Get(property, (string) null)
                .Should()
                .BeNull("because value has been deleted by 'DeleteProperty' method");
        }

        [Fact]
        public void GetAllPropertyObjects_Retrieves_Empty_Array_By_Default()
        {
            var pipelineContext = new Bag();
            
            pipelineContext.GetAllPropertyObjects()
                .Should()
                .BeEmpty("because by default there are no properties");
        }
        
        [Fact]
        public void GetAllPropertyObjects_Contains_An_Element_That_Was_Added()
        {
            var pipelineContext = new Bag();

            pipelineContext.Set(
                nameof(GetAllPropertyObjects_Contains_An_Element_That_Was_Added),
                nameof(PipelineContextTests), skipIfExists: true);

            pipelineContext.GetAllPropertyObjects()
                .Should()
                .ContainSingle("because by default there are no properties");
        }
        
        [Fact]
        public void ApplyProperty_With_SkipIfExists_Does_Not_Overwrite_The_Existing_Property()
        {
            var name = nameof(ApplyProperty_With_SkipIfExists_Does_Not_Overwrite_The_Existing_Property);
            var context = Bag.Create(new {Message = name});
            context.ApplyProperty("Message", "test", PropertyModificator.SkipIfExists);
            context.GetOrThrow<string>("Message")
                .Should().Be(name);
        }

        [Fact]
        public void ApplyProperty_With_UpdateValue_Overwrites_The_Existing_Property()
        {
            var name = nameof(ApplyProperty_With_UpdateValue_Overwrites_The_Existing_Property);
            var context = Bag.Create(new { Message = name });
            context.ApplyProperty("Message", "test", PropertyModificator.UpdateValue);
            context.GetOrThrow<string>("Message")
                .Should().Be("test");
        }
        
        [Fact]
        public void ContainsProperty_With_Generic_Type_Returns_False_If_Type_Of_The_Property_Has_Other_Type()
        {
            var name = nameof(ContainsProperty_With_Generic_Type_Returns_False_If_Type_Of_The_Property_Has_Other_Type);
            var members = new { Message = name };
            var context = Bag.Create(members);

            context.ContainsProperty<bool>(nameof(members.Message))
                .Should().BeFalse();
        }
        
        [Fact]
        public void ContainsProperty_With_Generic_Type_Returns_True_If_Type_Of_The_Property_Requested()
        {
            var name = nameof(ContainsProperty_With_Generic_Type_Returns_True_If_Type_Of_The_Property_Requested);
            var members = new { Message = name };
            var context = Bag.Create(members);

            context.ContainsProperty<string>(nameof(members.Message))
                .Should().BeTrue();
        }
    }

    public class PipelineContextTestObject : Bag
    {
        public virtual PipelineContextTestObject WithMessage(PipelineMessage message)
        {
            this.AddMessageObject(message);
            return this;
        }

        public virtual PipelineContextTestObject WithMessage(string message)
        {
            this.AddMessage(message);
            return this;
        }

        public virtual PipelineContextTestObject WithMessage(string message, MessageType type)
        {
            this.AddMessage(message, type);
            return this;
        }

        public virtual PipelineContextTestObject WithWarning(string message)
        {
            this.AddWarning(message);
            return this;
        }

        public virtual PipelineContextTestObject WithInformation(string message)
        {
            this.AddInformation(message);
            return this;
        }

        public virtual PipelineContextTestObject WithError(string message)
        {
            this.AddError(message);
            return this;
        }
    }
}
