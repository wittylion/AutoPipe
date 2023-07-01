using AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.AfterAttributeScenarios;
using AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.OrderAttributeScenarios;
using AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.SmartOrderScenarios;
using System.Collections.Generic;

namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests
{
    public class OrderMethodsDataSets
    {
        public static object[] A<T>(params string[] expected) where T : AutoProcessor, new()
        {
            return new object[] { new T(), expected };
        }
        public static IEnumerable<object[]> GetProcessorsAndOrder()
        {
            return new List<object[]> {
                A<ProcessorWithMethodsWhereOrderAttributeAssignedToAll>(nameof(ProcessorWithMethodsWhereOrderAttributeAssignedToAll.UseProperty),
                nameof(ProcessorWithMethodsWhereOrderAttributeAssignedToAll.GetProperty)),
            
                A<ProcessorWithMethodsWhereOrderAttributeAssignedToAllButTheOrderIsChanged>(nameof(ProcessorWithMethodsWhereOrderAttributeAssignedToAllButTheOrderIsChanged.GetProperty), nameof(ProcessorWithMethodsWhereOrderAttributeAssignedToAllButTheOrderIsChanged.UseProperty)),

                A<ProcessorWhereNoOrderAttributeAssigned_ButItHasGetterMethodAndMethodThatUsesProperty>(nameof(ProcessorWhereNoOrderAttributeAssigned_ButItHasGetterMethodAndMethodThatUsesProperty.GetProperty), nameof(ProcessorWhereNoOrderAttributeAssigned_ButItHasGetterMethodAndMethodThatUsesProperty.UseProperty)),

                A<HasGetterMethodAndMethodThatUsesProperty_ButThisTimePropertyUserHasEarlierFirstLetter>(nameof(HasGetterMethodAndMethodThatUsesProperty_ButThisTimePropertyUserHasEarlierFirstLetter.GetProperty), nameof(HasGetterMethodAndMethodThatUsesProperty_ButThisTimePropertyUserHasEarlierFirstLetter.APropertyUser)),

                A<OrderTestAutoProcessor5>(nameof(OrderTestAutoProcessor5.GetProperty), nameof(OrderTestAutoProcessor5.APropertyUser)),

                A<OrderTestAutoProcessor6>(nameof(OrderTestAutoProcessor6.APropertyUser), nameof(OrderTestAutoProcessor6.GetProperty)),

                A<OrderTestAutoProcessor7>(nameof(OrderTestAutoProcessor7.GetFirst), nameof(OrderTestAutoProcessor7.GetSecond), nameof(OrderTestAutoProcessor7.APropertyUser)),

                A<OrderTestAutoProcessor8>(nameof(OrderTestAutoProcessor8.GetFirst), nameof(OrderTestAutoProcessor8.GetSecond), nameof(OrderTestAutoProcessor8.APropertyUser)),

                A<OrderTestAutoProcessor9>(nameof(OrderTestAutoProcessor9.GetSecond), nameof(OrderTestAutoProcessor9.GetFirst), nameof(OrderTestAutoProcessor9.APropertyUser)),

                A<OrderTestAutoProcessor10>(nameof(OrderTestAutoProcessor10.WantSecond), nameof(OrderTestAutoProcessor10.WantFirst), nameof(OrderTestAutoProcessor10.APropertyUser)),

                A<OrderTestAutoProcessor11>(nameof(OrderTestAutoProcessor11.ASecond), nameof(OrderTestAutoProcessor11.AFirst), nameof(OrderTestAutoProcessor11.APropertyUser)),
            };
        }
    }
}
