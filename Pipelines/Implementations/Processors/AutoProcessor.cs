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

        public virtual IEnumerable<MethodInfo> GetMethodsToExecute()
        {
            var type = this.GetType();
            var allAttributes = GetMethodBindingAttributes();

            if (allAttributes.HasNoValue())
            {
                return Enumerable.Empty<MethodInfo>();
            }

            var bindingAttr = allAttributes.Aggregate((l, r) => l | r);
            return type.GetMethods(bindingAttr).Where(AcceptableByFilter).OrderBy(GetOrderOfExecution);
        }

        public virtual IEnumerable<BindingFlags> GetMethodBindingAttributes()
        {
            return new[] { BindingFlags.Public, BindingFlags.NonPublic, BindingFlags.Instance, BindingFlags.Static };
        }

        public virtual bool AcceptableByFilter(MethodInfo method)
        {
            return method?.GetCustomAttribute<ExecuteMethodAttribute>(false) != null;
        }

        public virtual int GetOrderOfExecution(MethodInfo method)
        {
            return method?.GetCustomAttribute<ExecuteMethodAttribute>()?.Order ?? default;
        }

        public virtual async Task ExecuteMethod(MethodInfo method, PipelineContext context)
        {
            var values = GetExecutionParameters(method, context);
            var result = method.Invoke(this, values.ToArray());
            await ProcessResult(context, result);
        }

        public virtual async Task ProcessResult(PipelineContext context, object methodResult)
        {
            if (methodResult.HasNoValue())
            {
                return;
            }

            if (methodResult is Task task)
            {
                await ProcessTask(context, task);
            }

            if (methodResult is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    await ProcessResult(context, item);
                }
                return;
            }

            if (methodResult is Action<PipelineContext> action)
            {
                action(context);
                return;
            }

            if (methodResult is Func<PipelineContext, object> functionContext)
            {
                var functionResult = functionContext(context);
                await ProcessResult(context, functionResult);
                return;
            }

            ProcessObjectProperties(context, methodResult);
        }

        public virtual void ProcessObjectProperties(PipelineContext context, object propertyContainer)
        {
            if (propertyContainer.HasNoValue() || context.HasNoValue())
            {
                return;
            }

            foreach (var prop in propertyContainer.GetType().GetProperties())
            {
                var contextProperty = new PipelineProperty(prop.Name, prop.GetValue(propertyContainer, null));
                context.SetOrAddProperty(contextProperty.Name, contextProperty.Value);
            }
        }

        public virtual async Task ProcessTask(PipelineContext context, Task task)
        {
            if (task.HasNoValue())
            {
                return;
            }

            await task;

            var property = task.GetType().GetProperty(nameof(Task<object>.Result));

            if (property.HasNoValue())
            {
                return;
            }

            var result = property.GetValue(task);
            if (result.HasValue())
            {
                await ProcessResult(context, result);
            }
        }

        protected virtual Action<PipelineContext> AddInformationMessage(string message)
        {
            return context => context.AddInformation(message);
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

        public virtual IEnumerable<object> GetExecutionParameters(MethodInfo method, PipelineContext context)
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
                            yield return metadata?.DefaultValue;
                        }
                    }
                    else
                    {
                        yield return metadata?.DefaultValue;
                    }
                }
            }
        }

        public virtual bool AllParametersAreValid(MethodInfo method, PipelineContext context)
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
                        var messageTemplate = "Property [{0}] is not found. Skipping method [{1}] in [{2}].";
                        var formattedMessage = string.Format(messageTemplate, parameter.Name, method.Name, method.DeclaringType.Name);
                        var message = containsErrorMessage ? formattedMessage + $" {metadata.ErrorMessage}" : formattedMessage;

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
                        var messageTemplate = "Property [{0}] is not assignable to type [{1}], its value is [{2}]. Skipping method [{3}] in [{4}].";
                        var formattedMessage = string.Format(messageTemplate, parameter.Name, parameter.ParameterType, val, method.Name, method.DeclaringType.Name);
                        var message = containsErrorMessage ? formattedMessage + $" {metadata.ErrorMessage}" : formattedMessage;

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
