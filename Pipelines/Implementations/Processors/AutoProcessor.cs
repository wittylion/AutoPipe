using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// An abstract processor that can be derived to implement
    /// processor with several methods to execute.
    /// The methods that are marked with <see cref="ExecuteMethodAttribute"/>
    /// in the derived type will be executed based on 
    /// <see cref="ExecuteMethodAttribute.Order"/>.
    /// </summary>
    public abstract class AutoProcessor : SafeProcessor
    {
        /// <summary>
        /// Collection of methods that will be executed one by one.
        /// </summary>
        public IEnumerable<MethodInfo> Methods { get; set; }

        /// <summary>
        /// A simple parameterless constructor.
        /// </summary>
        public AutoProcessor()
        {
            Methods = GetMethodsToExecute();
        }

        /// <summary>
        /// Finds methods to be executed in scope of this processor.
        /// </summary>
        /// <returns>
        /// Found methods that will be executed in <see cref="SafeExecute(PipelineContext)"/>.
        /// </returns>
        public virtual IEnumerable<MethodInfo> GetMethodsToExecute()
        {
            var type = this.GetType();
            var allAttributes = GetMethodBindingAttributes();

            if (allAttributes.HasNoValue())
            {
                return Enumerable.Empty<MethodInfo>();
            }

            var bindingAttr = allAttributes.Aggregate((l, r) => l | r);
            return type.GetMethods(bindingAttr).Where(AcceptableByFilter).OrderBy(GetOrderOfExecution).ThenBy(method => method.Name);
        }

        /// <summary>
        /// Returns attributes of methods to be taken into account during methods
        /// search in <see cref="GetMethodsToExecute"/>
        /// </summary>
        /// <returns>
        /// Attributes of the methods to be found during methods search.
        /// </returns>
        public virtual IEnumerable<BindingFlags> GetMethodBindingAttributes()
        {
            return new[] { BindingFlags.Public, BindingFlags.NonPublic, BindingFlags.Instance, BindingFlags.Static };
        }

        /// <summary>
        /// A condition that checks for <see cref="ExecuteMethodAttribute"/> presence.
        /// </summary>
        /// <param name="method">
        /// A method to be checked for acceptance criteria.
        /// </param>
        /// <returns>
        /// Value indicating whether method should be added to <see cref="Methods"/> collection.
        /// </returns>
        public virtual bool AcceptableByFilter(MethodInfo method)
        {
            return method?.GetCustomAttribute<ExecuteMethodAttribute>(false) != null;
        }

        /// <summary>
        /// Gets an order of method execution among the <see cref="Methods"/> collection.
        /// </summary>
        /// <param name="method">
        /// A method which order should be determined.
        /// </param>
        /// <returns>
        /// A number indicating the order of methods execution.
        /// </returns>
        public virtual int GetOrderOfExecution(MethodInfo method)
        {
            return method?.GetCustomAttribute<ExecuteMethodAttribute>()?.Order ?? default;
        }

        /// <summary>
        /// Executes a method with all the power of <see cref="PipelineContext"/>.
        /// Checks methods parameters and tries to find names of the parameters in the context.
        /// Handles the returned value to put it in the context.
        /// </summary>
        /// <param name="method">
        /// A method to be executed.
        /// </param>
        /// <param name="context">
        /// A context which properties are searched for methods parameters and
        /// which is used for returned value handling.
        /// </param>
        /// <returns>
        /// A task object indicating whether execution of the method has been completed.
        /// </returns>
        public virtual async Task ExecuteMethod(MethodInfo method, Bag context)
        {
            var values = GetExecutionParameters(method, context);
            var result = method.Invoke(this, values.ToArray());
            await ProcessResult(context, result).ConfigureAwait(false);
        }

        /// <summary>
        /// Tries to process a result of the method. Has checks of:
        /// Task, Task<T>, IEnumerable, Action<Bag>, Func<Bag, object>, object.
        /// All the properties of the object will be added to the Bag as properties.
        /// Task and Task<T> will be awaited and object of Task<T> will be processed as described earlier.
        /// Each object of IEnumerable collection will be processed as described earlier.
        /// </summary>
        /// <param name="context">
        /// A pipeline context to be used in result processing.
        /// </param>
        /// <param name="methodResult">
        /// A result of the executed method to be handled with pipeline context.
        /// </param>
        /// <returns>
        /// A task indicating whether method result has been processed or not.
        /// </returns>
        protected virtual async Task ProcessResult(Bag context, object methodResult)
        {
            if (methodResult.HasNoValue())
            {
                return;
            }

            if (methodResult is Task task)
            {
                await ProcessTask(context, task).ConfigureAwait(false);
                return;
            }

            if (methodResult is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    await ProcessResult(context, item).ConfigureAwait(false);
                }
                return;
            }

            if (methodResult is Action<Bag> action)
            {
                action(context);
                return;
            }

            if (methodResult is Func<Bag, object> functionContext)
            {
                var functionResult = functionContext(context);
                await ProcessResult(context, functionResult).ConfigureAwait(false);
                return;
            }

            ProcessObjectProperties(context, methodResult);
        }

        /// <summary>
        /// For each property of the <paramref name="propertyContainer"/>
        /// adds or updates a property in <paramref name="context"/>.
        /// </summary>
        /// <param name="context">
        /// A context to be used for adding or updating properties.
        /// </param>
        /// <param name="propertyContainer">
        /// An object which properties will be added to the pipeline context.
        /// </param>
        protected virtual void ProcessObjectProperties(Bag context, object propertyContainer)
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

        /// <summary>
        /// Awaits a task and if it has a result value, takes
        /// the result value and puts all its properties to the
        /// <paramref name="context"/>.
        /// </summary>
        /// <param name="context">
        /// A context to be used to set properties of the task result.
        /// </param>
        /// <param name="task">
        /// A task to be awaited.
        /// </param>
        /// <returns>
        /// A task indicating whether the processing of the <paramref name="task"/>
        /// has been completed.
        /// </returns>
        protected virtual async Task ProcessTask(Bag context, Task task)
        {
            if (task.HasNoValue())
            {
                return;
            }

            await task.ConfigureAwait(false);

            var property = task.GetType().GetProperty(nameof(Task<object>.Result));

            if (property.HasNoValue())
            {
                return;
            }

            var result = property.GetValue(task);
            if (result.HasValue())
            {
                await ProcessResult(context, result).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// A quick reference to be returned in custom methods
        /// to execute <see cref="PipelineContext.AddInformation(string)"/> method.
        /// </summary>
        /// <param name="message">
        /// A message to be passed to <see cref="PipelineContext.AddInformation(string)"/> method.
        /// </param>
        /// <returns>
        /// An action that will be executed in <see cref="AutoProcessor"/> return handler.
        /// </returns>
        protected virtual Action<Bag> AddInformationMessage(string message)
        {
            return context => context.AddInformation(message);
        }

        /// <summary>
        /// A quick reference to be returned in custom methods
        /// to execute <see cref="PipelineContext.AddErrorMessage(string)"/> method.
        /// </summary>
        /// <param name="message">
        /// A message to be passed to <see cref="PipelineContext.AddErrorMessage(string)"/> method.
        /// </param>
        /// <returns>
        /// An action that will be executed in <see cref="AutoProcessor"/> return handler.
        /// </returns>
        protected virtual Action<Bag> AddErrorMessage(string message)
        {
            return context => context.AddError(message);
        }

        /// <summary>
        /// A quick reference to be returned in custom methods
        /// to execute <see cref="PipelineContext.AddMessageObjects(IEnumerable{PipelineMessage})"/> method.
        /// </summary>
        /// <param name="message">
        /// A message to be passed to <see cref="PipelineContext.AddMessageObjects(IEnumerable{PipelineMessage})"/> method.
        /// </param>
        /// <returns>
        /// An action that will be executed in <see cref="AutoProcessor"/> return handler.
        /// </returns>
        protected virtual Action<Bag> AddMessageObjects(params PipelineMessage[] messages)
        {
            return context => context.AddMessageObjects(messages);
        }

        /// <summary>
        /// A quick reference to be returned in custom methods
        /// to execute <see cref="PipelineContext.AbortPipelineWithErrorMessage(string)"/> method.
        /// </summary>
        /// <param name="message">
        /// A message to be passed to <see cref="PipelineContext.AbortPipelineWithErrorMessage(string)"/> method.
        /// </param>
        /// <returns>
        /// An action that will be executed in <see cref="AutoProcessor"/> return handler.
        /// </returns>
        protected virtual Action<Bag> AbortPipelineWithErrorMessage(string message)
        {
            return context => context.AbortPipelineWithErrorMessage(message);
        }

        /// <summary>
        /// Tries to define values to pass them to the method.
        /// Uses the reflection to get the names of the parameters
        /// and then searches them in the pipeline conetext.
        /// In case parameter method has a type derived from <see cref="PipelineContext"/>
        /// passes the <paramref name="context"/>.
        /// </summary>
        /// <param name="method">
        /// A method to be used to define parameters.
        /// </param>
        /// <param name="context">
        /// A context to find values to be passed to the method.
        /// </param>
        /// <returns>
        /// Collection of values in order defined in the <paramref name="method"/>.
        /// </returns>
        protected virtual IEnumerable<object> GetExecutionParameters(MethodInfo method, Bag context)
        {
            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                if (typeof(Bag).IsAssignableFrom(parameter.ParameterType))
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

        /// <summary>
        /// Does a predefined check to validate execution
        /// possibility of the of the <paramref name="method"/>.
        /// Uses <see cref="ContextParameterAttribute"/> to do some
        /// parameter validation checks.
        /// </summary>
        /// <param name="method">
        /// A method which parameters should be checked for validity.
        /// </param>
        /// <param name="context">
        /// A context used to do a parameters check.
        /// </param>
        /// <returns>
        /// Value indicating whether all parameters are valid or not.
        /// </returns>
        protected virtual bool AllParametersAreValid(MethodInfo method, Bag context)
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


        /// <summary>
        /// Executes all methods found with <see cref="GetMethodsToExecute"/> 
        /// using all the power of <see cref="PipelineContext"/>.
        /// </summary>
        /// <param name="context">
        /// A context which properties are searched for methods parameters and
        /// which is used for returned value handling.
        /// </param>
        /// <returns>
        /// A task object indicating whether execution of the method has been completed.
        /// </returns>
        public override async Task SafeExecute(Bag args)
        {
            if (Methods != null)
            {
                foreach (var method in Methods)
                {
                    if (args.IsAborted)
                    {
                        break;
                    }

                    if (AllParametersAreValid(method, args))
                    {
                        await ExecuteMethod(method, args).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
