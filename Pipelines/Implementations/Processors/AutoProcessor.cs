using Pipelines.ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public class AutoProcessor : SafeProcessor
    {
        public IEnumerable<MethodInfo> Methods { get; set; }

        public AutoProcessor()
        {
            Methods = GetMethodsToExecute();
        }

        protected virtual IEnumerable<MethodInfo> GetMethodsToExecute()
        {
            var type = this.GetType();
            var bindingAttr = GetMethodBindingAttributes().Aggregate((l,r) => l | r);
            return type.GetMethods(bindingAttr).Where(MethodFilter).OrderBy(OrderByAttributeProperty);
        }

        protected virtual IEnumerable<BindingFlags> GetMethodBindingAttributes()
        {
            return new [] { BindingFlags.Public, BindingFlags.NonPublic, BindingFlags.Instance, BindingFlags.Static };
        }

        protected virtual bool MethodFilter(MethodInfo method)
        {
            return method.GetCustomAttribute<ExecuteMethodAttribute>(false) != null;
        }

        protected virtual int OrderByAttributeProperty(MethodInfo method)
        {
            return method.GetCustomAttribute<ExecuteMethodAttribute>().Order;
        }

        protected virtual async Task ExecuteMethod(MethodInfo method, PipelineContext context)
        {
            var values = GetExecutionParameters(method, context);
            var result = method.Invoke(this, values.ToArray());
            await ProcessResult(method, context, result);
        }

        protected virtual async Task ProcessResult(MethodInfo method, PipelineContext context, object methodResult)
        {
            if (methodResult == null)
            {
                return;
            }

            if (methodResult is Task task)
            {
                await ProcessTask(method, context, task);
            }

            if (methodResult is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    await ProcessResult(method, context, item);
                }
                return;
            }

            if (methodResult is Action<PipelineContext> action)
            {
                action(context);
            }

            ProcessObjectProperties(method, context, methodResult);
        }

        protected virtual void ProcessObjectProperties(MethodInfo method, PipelineContext context, object propertyContainer)
        {
            foreach (var prop in propertyContainer.GetType().GetProperties())
            {
                var contextProperty = new PipelineProperty(prop.Name, prop.GetValue(propertyContainer, null));
                context.SetOrAddProperty(contextProperty.Name, contextProperty.Value);
            }
        }

        protected virtual async Task ProcessTask(MethodInfo method, PipelineContext context, Task task)
        {
            await task;

            var property = task.GetType().GetProperty("Result");

            if (property != null)
            {
                var value = property.GetValue(task);
                if (value != null)
                {
                    await ProcessResult(method, context, value);
                }
            }
        }

        protected virtual Action<PipelineContext> AddErrorMessage(string message)
        {
            return context => context.AddError(message);
        }

        protected virtual Action<PipelineContext> AddMessageObjects(params PipelineMessage[] messages)
        {
            return context => context.AddMessageObjects(messages);
        }

        protected virtual Action<PipelineContext> AbortPipelineWithErrorMessage(string message)
        {
            return context => context.AbortPipelineWithErrorMessage(message);
        }

        protected virtual IEnumerable<object> GetExecutionParameters(MethodInfo method, PipelineContext context)
        {
            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                if (typeof(PipelineContext).IsAssignableFrom(parameter.ParameterType))
                {
                    yield return context;
                }
                else
                {
                    var name = parameter.Name;

                    var metadata = parameter.GetCustomAttribute<ContextParameterAttribute>();
                    if (metadata != null)
                    {
                        if (!string.IsNullOrWhiteSpace(metadata.Name) && context.ContainsProperty(metadata.Name))
                        {
                            name = metadata.Name;
                        }
                    }

                    var obj = context.GetPropertyObjectOrNull(name);

                    if (obj.HasValue)
                    {
                        var val = obj.Value.Value;

                        if (val != null && parameter.ParameterType.IsAssignableFrom(val.GetType()))
                        {
                            yield return val;
                        }
                        else
                        {
                            yield return metadata.DefaultValue;
                        }
                    }
                    else
                    {
                        yield return metadata.DefaultValue;
                    }
                }
            }
        }

        protected virtual bool AllParametersAreValid(MethodInfo method, PipelineContext context)
        {
            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                var metadata = parameter.GetCustomAttribute<ContextParameterAttribute>();

                if (metadata == null)
                {
                    continue;
                }

                if (metadata.Required || metadata.AbortIfNotExist)
                {
                    PipelineProperty? property;

                    bool containsErrorMessage = metadata.ErrorMessage.HasValue();
                    bool containsMetadataProperty =
                        metadata.Name.HasNoValue()
                            ? false
                            : context.ContainsProperty(metadata.Name);

                    bool containsPropertyName =
                        parameter.Name.HasNoValue()
                        ? false
                        : context.ContainsProperty(parameter.Name);

                    if (containsMetadataProperty)
                    {
                        property = context.GetPropertyObjectOrNull(metadata.Name);
                    }
                    else if (containsPropertyName)
                    {
                        property = context.GetPropertyObjectOrNull(parameter.Name);
                    }
                    else
                    {
                        var message = containsErrorMessage ? $"Property [{parameter.Name}] is not found. Skipping method [{method.Name}] in [{this.GetType().Name}]: \"{metadata.ErrorMessage}\"" : $"Property [{parameter.Name}] is not found. Skipping method [{method.Name}] in [{this.GetType().Name}].";

                        if (metadata.AbortIfNotExist)
                        {
                            context.AbortPipelineWithErrorMessage(message);
                        }
                        else
                        {
                            context.AddError(message);
                        }

                        return false;
                    }

                    var val = property.Value.Value;
                    if (val == null || !parameter.ParameterType.IsAssignableFrom(val.GetType()))
                    {
                        var message = containsErrorMessage ? $"Property [{parameter.Name}] is not assignable to type [{parameter.ParameterType}], its value is [{val}]. Skipping method [{method.Name}] in [{this.GetType().Name}]: \"{metadata.ErrorMessage}\"" : $"Property [{parameter.Name}] is not assignable to type [{parameter.ParameterType}], its value is [{val}]. Skipping method [{method.Name}] in [{this.GetType().Name}].";

                        if (metadata.AbortIfNotExist)
                        {
                            context.AbortPipelineWithErrorMessage(message);
                        }
                        else
                        {
                            context.AddError(message);
                        }

                        return false;
                    }
                }
            }

            return true;
        }

        public override async Task SafeExecute(PipelineContext args)
        {
            if (Methods != null)
            {
                foreach (var method in Methods)
                {
                    if (AllParametersAreValid(method, args))
                    {
                        await ExecuteMethod(method, args);
                    }
                }
            }
        }
    }
}
