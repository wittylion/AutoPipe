using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// An abstract processor that can be derived to implement
    /// processor with several methods to execute.
    /// The methods that are marked with <see cref="RunAttribute"/>
    /// in the derived type will be executed based on 
    /// <see cref="RunAttribute.Order"/>.
    /// </summary>
    public abstract class AutoProcessor : SafeProcessor
    {
        public static readonly string SkipMethodOnMissingPropertyMessage = "Property [{0}] is not found. Skipping method [{1}] in [{2}].";
        public static readonly string SkipMethodOnWrongTypeMessage = "Property [{0}] is not assignable to type [{1}], its value is [{2}]. Skipping method [{3}] in [{4}].";

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
        protected virtual IEnumerable<BindingFlags> GetMethodBindingAttributes()
        {
            return new[] { BindingFlags.Public, BindingFlags.NonPublic, BindingFlags.Instance, BindingFlags.Static };
        }

        /// <summary>
        /// A condition that checks for <see cref="RunAttribute"/> presence.
        /// </summary>
        /// <param name="method">
        /// A method to be checked for acceptance criteria.
        /// </param>
        /// <returns>
        /// Value indicating whether method should be added to <see cref="Methods"/> collection.
        /// </returns>
        public virtual bool AcceptableByFilter(MethodInfo method)
        {
            return method?.GetCustomAttribute<RunAttribute>(false) != null;
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
            var order = method?.GetCustomAttribute<OrderAttribute>()?.Order;
            if (order != null)
            {
                return order.Value;
            }

            return default;
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
        public virtual async Task Run(MethodInfo method, Bag context)
        {
            var values = GetExecutionParameters(method, context);
            var result = method.Invoke(this, values.ToArray());
            await ProcessResult(method, context, result, skipNameBasedActions: false).ConfigureAwait(false);
        }

        protected virtual IEnumerable<string> GetPropertyUpdateIdentifiers()
        {
            yield return "Set";
            yield return "Update";
            yield return "Overwrite";
        }

        protected virtual IEnumerable<string> GetPropertyEnsureIdentifiers()
        {
            yield return "Get";
            yield return "Ensure";
            yield return "Add";
        }

        protected virtual void ProcessBasedOnName(MethodInfo method, IEnumerable<string> actions, Action<string> executor)
        {
            foreach (var identifier in actions)
            {
                if (method.Name.StartsWith(identifier))
                {
                    if (method.Name.Length == identifier.Length)
                    {
                        break;
                    }

                    var property = method.Name.Substring(identifier.Length);
                    if (property.Length == 0)
                    {
                        continue;
                    }

                    executor(property);
                    return;
                }
            }
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
        protected virtual async Task ProcessResult(MethodInfo method, Bag context, object methodResult, bool skipNameBasedActions = true)
        {
            if (methodResult.HasNoValue())
            {
                return;
            }

            if (methodResult is Task task)
            {
                await ProcessTask(method, context, task).ConfigureAwait(false);
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
                await ProcessResult(method, context, functionResult, skipNameBasedActions: false).ConfigureAwait(false);
                return;
            }

            if (!skipNameBasedActions)
            {
                bool handled = false;
                ProcessBasedOnName(method, GetPropertyUpdateIdentifiers(), property => { context.Set(property, methodResult); handled = true; });
                if (handled) return;

                ProcessBasedOnName(method, GetPropertyEnsureIdentifiers(), property => { context.Set(property, methodResult, skipIfExists: true); handled = true; });
                if (handled) return;
            }

            if (methodResult is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    await ProcessResult(method, context, item, skipNameBasedActions: true).ConfigureAwait(false);
                }
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
                context.Set(prop.Name, prop.GetValue(propertyContainer, null));
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
        protected virtual async Task ProcessTask(MethodInfo method, Bag context, Task task)
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
                await ProcessResult(method, context, result, skipNameBasedActions: false).ConfigureAwait(false);
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
        protected virtual Action<Bag> Info(string message)
        {
            return context => context.Info(message);
        }

        protected virtual Action<Bag> Warning(string message)
        {
            return context => context.Warning(message);
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
        protected virtual Action<Bag> Error(string message)
        {
            return context => context.Error(message);
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
        protected virtual Action<Bag> AddMessages(params PipelineMessage[] messages)
        {
            return context => context.AddMessages(messages);
        }

        /// <summary>
        /// A quick reference to be returned in custom methods
        /// to execute <see cref="PipelineContext.EndPipelineWithErrorMessage(string)"/> method.
        /// </summary>
        /// <param name="message">
        /// A message to be passed to <see cref="PipelineContext.EndPipelineWithErrorMessage(string)"/> method.
        /// </param>
        /// <returns>
        /// An action that will be executed in <see cref="AutoProcessor"/> return handler.
        /// </returns>
        protected virtual Action<Bag> ErrorEnd(string message)
        {
            return context => context.ErrorEnd(message);
        }

        protected virtual Action<Bag> WarningEnd(string message)
        {
            return context => context.WarningEnd(message);
        }

        protected virtual Action<Bag> InfoEnd(string message)
        {
            return context => context.InfoEnd(message);
        }

        protected virtual Action<Bag> End()
        {
            return context => context.End();
        }

        protected virtual Action<Bag> EndResult(object result)
        {
            return context => context.EndResult(result);
        }

        protected virtual Action<Bag> Result(object result)
        {
            return context => context.SetResult(result);
        }

        protected virtual Action<Bag> InfoEndResult(object result, string message)
        {
            return context => context.InfoEndResult(result, message);
        }

        protected virtual Action<Bag> WarningEndResult(object result, string message)
        {
            return context => context.WarningEndResult(result, message);
        }

        protected virtual Action<Bag> ErrorEndResult(object result, string message)
        {
            return context => context.ErrorEndResult(result, message);
        }

        protected virtual Action<Bag> InfoEndNoResult(string message)
        {
            return context => context.InfoEndNoResult(message);
        }

        protected virtual Action<Bag> WarningEndNoResult(string message)
        {
            return context => context.WarningEndNoResult(message);
        }

        protected virtual Action<Bag> ErrorEndNoResult(string message)
        {
            return context => context.ErrorEndNoResult(message);
        }

        protected virtual Action<Bag> InfoResult(object result, string message)
        {
            return context => context.InfoResult(result, message);
        }

        protected virtual Action<Bag> WarningResult(object result, string message)
        {
            return context => context.WarningResult(result, message);
        }

        protected virtual Action<Bag> ErrorResult(object result, string message)
        {
            return context => context.ErrorResult(result, message);
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
            var parameters = method.GetParameters().Where(x => x.GetCustomAttribute<SkipAttribute>() == null);
            var bagTypes = context.GroupBy(x => x.Value.GetType()).Where(x => x.Count() == 1).ToDictionary(x => x.Key, x => x.First().Value);

            foreach (var parameter in parameters)
            {
                if (typeof(Bag).IsAssignableFrom(parameter.ParameterType))
                {
                    yield return context;
                    continue;
                }

                var names = GetParameterNames(parameter);

                if (context.ContainsAny(names, out object val))
                {
                    if (parameter.ParameterType.IsAssignableFrom(val.GetType()))
                    {
                        yield return val;
                    }
                    else
                    {
                        var defaultValueAttribute = parameter.GetCustomAttribute<OrAttribute>();
                        yield return defaultValueAttribute?.DefaultValue;
                    }
                }
                else
                {
                    if (bagTypes.TryGetValue(parameter.ParameterType, out var valueOfType))
                    {
                        yield return valueOfType;
                    }
                    else
                    {
                        var defaultValueAttribute = parameter.GetCustomAttribute<OrAttribute>();
                        yield return defaultValueAttribute?.DefaultValue;
                    }
                }
            }
        }

        protected virtual IEnumerable<string> GetParameterNames(ParameterInfo parameter)
        {
            yield return parameter.Name;

            var nameAttribute = parameter.GetCustomAttribute<AkaAttribute>();
            if (nameAttribute != null)
            {
                foreach (var alias in nameAttribute.Aliases)
                {
                    yield return alias;
                }
            }

        }

        /// <summary>
        /// Does a predefined check to validate execution
        /// possibility of the of the <paramref name="method"/>.
        /// Uses <see cref="RequiredAttribute"/> to do some
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
            var parameters = method.GetParameters().Where(x => x.GetCustomAttribute<SkipAttribute>() == null);
            var bagTypes = context.GroupBy(x => x.Value.GetType()).Where(x => x.Count() == 1).ToDictionary(x => x.Key, x => x.First().Value);

            foreach (var parameter in parameters)
            {
                if (typeof(Bag).IsAssignableFrom(parameter.ParameterType))
                {
                    continue;
                }

                var metadata = parameter.GetCustomAttribute<RequiredAttribute>();

                if (metadata == null)
                {
                    continue;
                }

                var orAttribute = parameter.GetCustomAttribute<OrAttribute>();

                if (orAttribute != null)
                {
                    continue;
                }

                var names = GetParameterNames(parameter);
                if (!context.ContainsAny(names, out object property))
                {
                    if (!bagTypes.TryGetValue(parameter.ParameterType, out property))
                    {
                        var formattedMessage = SkipMethodOnMissingPropertyMessage.FormatWith(parameter.Name, method.GetName(), method.DeclaringType.GetName());
                        var message = metadata.Message.HasValue() ? formattedMessage + $" {metadata.Message}" : formattedMessage;

                        context.Error(message, metadata.End);

                        return false;
                    }
                }

                var val = property;
                if (val == null || !parameter.ParameterType.IsAssignableFrom(val.GetType()))
                {
                    var formattedMessage = SkipMethodOnWrongTypeMessage.FormatWith(parameter.Name, parameter.ParameterType, val, method.GetName(), method.DeclaringType.GetName());
                    var message = metadata.Message.HasValue() ? formattedMessage + $" {metadata.Message}" : formattedMessage;

                    context.Error(message, metadata.End);

                    return false;
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
        public override async Task SafeRun(Bag args)
        {
            if (Methods != null)
            {
                foreach (var method in Methods)
                {
                    if (args.Ended)
                    {
                        break;
                    }

                    if (AllParametersAreValid(method, args))
                    {
                        await Run(method, args).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
