using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pipelines.Implementations.Processors
{

    public class ExplanatoryProcessor : SafeProcessor
    {
        public IEnumerable<MethodInfo> Methods { get; set; }

        public ExplanatoryProcessor()
        {
            FindMethodsToExecute();
        }

        public void FindMethodsToExecute()
        {
            var type = this.GetType();
            const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            Methods = type.GetMethods(bindingAttr).Where(MethodFilter).OrderBy(OrderByAttributeProperty);
        }

        public bool MethodFilter(MethodInfo method)
        {
            return method.GetCustomAttribute<ExecuteMethodAttribute>(false) != null;
        }

        public int OrderByAttributeProperty(MethodInfo method)
        {
            return method.GetCustomAttribute<ExecuteMethodAttribute>().Order;
        }

        public async Task ExecuteMethod(MethodInfo method, PipelineContext context)
        {
            var values = PrepareParameters(method, context);
            var result = method.Invoke(this, values.ToArray());
            object propertyContainer = null;

            if (result is Task task)
            {
                await task;

                var property = task.GetType().GetProperty("Result");

                if (property != null)
                {
                    var value = property.GetValue(task);
                    if (value != null)
                    {
                        propertyContainer = value;
                    }
                }
            }
            else
            {
                propertyContainer = result;
            }

            if (propertyContainer != null)
            {
                if (propertyContainer is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        foreach (var prop in item.GetType().GetProperties())
                        {
                            var contextProperty = new PipelineProperty(prop.Name, prop.GetValue(item, null));
                            context.SetOrAddProperty(contextProperty.Name, contextProperty.Value);
                        }
                    }
                }
                else
                {
                    foreach (var prop in propertyContainer.GetType().GetProperties())
                    {
                        var contextProperty = new PipelineProperty(prop.Name, prop.GetValue(propertyContainer, null));
                        context.SetOrAddProperty(contextProperty.Name, contextProperty.Value);
                    }
                }
            }
        }

        public IEnumerable<object> PrepareParameters(MethodInfo method, PipelineContext context)
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

                        if (parameter.ParameterType.IsAssignableFrom(val.GetType()))
                        {
                            yield return val;
                        }
                        else
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }

        public bool AllParametersAreValid(MethodInfo method, PipelineContext context)
        {
            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                var metadata = parameter.GetCustomAttribute<ContextParameterAttribute>();

                if (metadata == null)
                {
                    continue;
                }

                if (metadata.Required)
                {
                    PipelineProperty? property;
                    bool containsMetadataProperty =
                        string.IsNullOrWhiteSpace(metadata.Name)
                            ? false
                            : context.ContainsProperty(metadata.Name);

                    bool containsPropertyName =
                        string.IsNullOrWhiteSpace(parameter.Name)
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
                        return false;
                    }

                    if (property == null || !parameter.ParameterType.IsAssignableFrom(property.Value.Value.GetType()))
                    {
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
