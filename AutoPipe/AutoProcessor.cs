using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoPipe
{
    /// <summary>
    /// An abstract processor that can be derived to implement
    /// processor with several methods to execute.
    /// The methods that are marked with <see cref="RunAttribute"/>
    /// in the derived type will be executed based on 
    /// <see cref="OrderAttribute"/>.
    /// </summary>
    public class AutoProcessor : IProcessor
    {
        public static readonly string SkipMethodOnMissingPropertyMessage = "Property [{0}] is not found. Skipping method [{1}] in [{2}].";
        public static readonly string SkipMethodOnWrongTypeMessage = "Property [{0}] is not assignable to type [{1}], its value is [{2}]. Skipping method [{3}] in [{4}].";
        public static readonly string ProcessorMustNotBeNull = "Processor passed to the constructor is null. Please provide an object.";
        public static readonly string MethodClaimsAllParameters = "Run attribute of the current execution method contains attribute [Strict], which requires all parameters to be declared before execution.";
        public static readonly string ClassClaimsAllParameters = "Run attribute of your processor class contains attribute [Strict], which requires all parameters to be declared before execution.";

        /// <summary>
        /// Collection of methods that will be executed one by one.
        /// </summary>
        public IEnumerable<MethodInfo> Methods { get; set; }
        public object Processor { get; }

        public static IProcessor From(object processorClass)
        {
            return new AutoProcessor(processorClass);
        }

        public static IProcessor From<T>() where T : class, new()
        {
            return new AutoProcessor(new T());
        }

        /// <summary>
        /// A simple parameterless constructor.
        /// </summary>
        protected AutoProcessor()
        {
            Processor = this;
            Methods = GetMethodsToExecute();
            IsStrict = Processor.GetType().IsStrict();
        }

        public AutoProcessor(object processor)
        {
            Processor = processor;
            Methods = GetMethodsToExecute();
            IsStrict = Processor.GetType().IsStrict();
        }

        /// <summary>
        /// Finds methods to be executed in scope of this processor.
        /// </summary>
        /// <returns>
        /// Found methods that will be executed in <see cref="SafeExecute(PipelineContext)"/>.
        /// </returns>
        public virtual IEnumerable<MethodInfo> GetMethodsToExecute()
        {
            if (Processor.HasNoValue() || Processor.GetType() == typeof(AutoProcessor))
            {
                return Enumerable.Empty<MethodInfo>();
            }

            var type = Processor.GetType();
            var allAttributes = GetMethodBindingAttributes();

            if (allAttributes.HasNoValue())
            {
                return Enumerable.Empty<MethodInfo>();
            }

            var bindingAttr = allAttributes.Aggregate((l, r) => l | r);
            var methods = type.GetMethods(bindingAttr);
            var filteredMethods = methods.Where(AcceptableByFilter);
            var orderedMethods = OrderMethods(filteredMethods);

            return orderedMethods;
        }

        protected virtual IEnumerable<MethodInfo> OrderMethods(IEnumerable<MethodInfo> methods)
        {
            var methodsDictionary = new Dictionary<MethodInfo, int?>();
            var namesDictionary = new Dictionary<string, MethodInfo>();

            // param, List of generating methods
            var paramsDictionary = new Dictionary<string, List<MethodInfo>>();
            var methodParamsDictionary = new Dictionary<MethodInfo, List<string>>();

            var allIdentifiers = GetPropertyUpdateIdentifiers().Concat(GetPropertyEnsureIdentifiers());

            foreach (var method in methods)
            {
                var order = GetOrderOfExecution(method);
                methodsDictionary.Add(method, order);
                foreach (var name in method.GetNames())
                {
                    if (!namesDictionary.TryGetValue(name, out MethodInfo existingMethod))
                    {
                        namesDictionary.Add(name, method);
                    }
                    else
                    {
                        throw new Exception($"The same alias [{name}] was applied for methods [{method.Name}] and [{existingMethod.Name}]. Please use unique names for each method.");
                    }
                }

                var parameters = method.GetParameters().Select(x => x.Name.ToLower()).ToList();
                methodParamsDictionary.Add(method, parameters);
                foreach (var parameter in parameters)
                {
                    if (!paramsDictionary.ContainsKey(parameter))
                    {
                        paramsDictionary.Add(parameter.ToLower(), new List<MethodInfo>());
                    }
                }

                var returningParameter = allIdentifiers.FirstOrDefault(x => method.Name.StartsWith(x));
                if (returningParameter != null)
                {
                    var parameterName = method.Name.Substring(returningParameter.Length).ToLower();
                    if (paramsDictionary.TryGetValue(parameterName, out var data))
                    {
                        data.Add(method);
                    }
                    else
                    {
                        paramsDictionary.Add(parameterName, new List<MethodInfo>() { method });
                    }
                }
            }

            var reviewMethods = methodsDictionary.Where(x => x.Value.HasNoValue()).Select(x => x.Key).ToList();
            var orderedMethods = methodsDictionary.Where(x => x.Value != null).OrderBy(x => x.Value).ThenBy(x => x.Key.Name).Select(x => x.Key).ToList();
            foreach (var method in reviewMethods)
            {
                CalculateOrderBasedOnAttributes(method, orderedMethods, namesDictionary, new HashSet<MethodInfo>(), paramsDictionary, methodParamsDictionary);
            }

            return orderedMethods;
        }

        protected virtual void CalculateOrderBasedOnAttributes(MethodInfo method, List<MethodInfo> orderedMethods, Dictionary<string, MethodInfo> namesDictionary, HashSet<MethodInfo> visitedMethods, Dictionary<string, List<MethodInfo>> paramsDictionary, Dictionary<MethodInfo, List<string>> methodParamsDictionary)
        {
            if (orderedMethods.Contains(method)) return;

            visitedMethods.Add(method);

            var previous = method.GetCustomAttribute<AfterAttribute>()?.PreviousName;

            if (previous != null && namesDictionary.TryGetValue(previous, out MethodInfo previousMethod))
            {
                if (previousMethod == method)
                {
                    var currentName = method.GetName();
                    throw new Exception($"The [{previous}] and [{currentName}] are names of the same method. After attribute cannot be applied to the same method. Check [Aka] attribute for duplicates.");
                }

                if (visitedMethods.Contains(previousMethod))
                {
                    var currentName = method.GetName();
                    throw new Exception($"Circular dependency detected. The name [{previous}] in [After] attribute of [{currentName}] method is already used by one of the methods in the chain. Check the order of the methods execution.");
                }

                if (!orderedMethods.Contains(previousMethod))
                {
                    CalculateOrderBasedOnAttributes(previousMethod, orderedMethods, namesDictionary, visitedMethods, paramsDictionary, methodParamsDictionary);
                }

                var index = orderedMethods.FindIndex(x => previousMethod == x);
                orderedMethods.Insert(index + 1, method);
                visitedMethods.Remove(method);
                return;
            }

            var requiredParameters = methodParamsDictionary[method];
            if (requiredParameters.Any())
            {
                var precedentList = new List<MethodInfo>();
                foreach (var requiredParameter in requiredParameters)
                {
                    var allGenerators = paramsDictionary[requiredParameter];
                    foreach (var generator in allGenerators)
                    {
                        if (generator == method) continue;
                        if (visitedMethods.Contains(generator)) continue;

                        CalculateOrderBasedOnAttributes(generator, orderedMethods, namesDictionary, visitedMethods, paramsDictionary, methodParamsDictionary);
                        precedentList.Add(generator);
                    }
                }

                var index = orderedMethods.FindLastIndex(x => precedentList.Contains(x));
                orderedMethods.Insert(index + 1, method);
                visitedMethods.Remove(method);
                return;
            }

            orderedMethods.Add(method);
            visitedMethods.Remove(method);
        }

        private bool? runAll;
        protected virtual bool RunAll
        {
            get
            {
                return (runAll ?? (runAll = this.Processor.GetType().ShouldRunAll())).Value;
            }
        }

        protected virtual bool IsStrict { get; }

        /// <summary>
        /// Returns attributes of methods to be taken into account during methods
        /// search in <see cref="GetMethodsToExecute"/>
        /// </summary>
        /// <returns>
        /// Attributes of the methods to be found during methods search.
        /// </returns>
        protected virtual IEnumerable<BindingFlags> GetMethodBindingAttributes()
        {
            yield return Repository.RunningMethodsFlags;
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
            return (this.RunAll || method.ShouldRun()) && !method.ShouldSkip();
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
        public virtual int? GetOrderOfExecution(MethodInfo method)
        {
            var order = method?.GetCustomAttribute<OrderAttribute>()?.Order;
            if (order != null)
            {
                return order.Value;
            }

            return null;
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
        protected virtual async Task RunMethod(MethodInfo method, Bag context)
        {
            var values = GetExecutionParameters(method, context);
            var result = method.Invoke(Processor, values.ToArray());
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

            if (methodResult is LambdaExpression expression)
            {
                await ProcessExpression(method, context, expression);
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

        protected virtual async Task ProcessExpression(MethodInfo method, Bag bag, LambdaExpression expression)
        {
            var result = bag.Map(expression);

            await this.ProcessResult(method, bag, result, false);
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

            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"During the execution of the task returned by [{method.GetName()}] was thrown an exception:");

                do
                {
                    sb.AppendLine(ex.Message);
                    ex = ex.InnerException;
                }
                while (ex != null);

                context.ErrorEnd(sb.ToString());

                return;
            }

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

        public virtual string Name => this.Names.First();

        public virtual IEnumerable<string> Names
        {
            get
            {
                if (Processor == this)
                {
                    return this.Names();
                }

                return Processor.GetType().GetNames();
            }
        }


        public virtual string Description
        {
            get
            {
                if (Processor == this)
                {
                    return this.Description();
                }

                return Processor.GetType().GetDescription();
            }
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
            var bagTypes = context.GetSingleTypeValues();

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
                        continue;
                    }
                }

                if (context.ServiceProvider != null)
                {
                    var valueFromProvider = context.ServiceProvider.GetService(parameter.ParameterType);
                    if (valueFromProvider != null)
                    {
                        yield return valueFromProvider;
                        continue;
                    }
                }


                if (parameters.Count(param => param.ParameterType == parameter.ParameterType) == 1)
                {
                    if (bagTypes.TryGetValue(parameter.ParameterType, out var valueOfType))
                    {
                        yield return valueOfType;
                        continue;
                    }

                    var singleAssignableType = bagTypes.Where(bagType => parameter.ParameterType.IsAssignableFrom(bagType.Key));

                    if (singleAssignableType.Count() == 1)
                    {
                        yield return singleAssignableType.First().Value;
                        continue;
                    }
                }

                var defaultValueAttribute = parameter.GetCustomAttribute<OrAttribute>();
                yield return defaultValueAttribute?.DefaultValue;
                continue;
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
            var bagTypes = context.GetSingleTypeValues();
            var methodIsStrict = method.IsStrict();

            foreach (var parameter in parameters)
            {
                if (typeof(Bag).IsAssignableFrom(parameter.ParameterType))
                {
                    continue;
                }

                var metadata = parameter.GetCustomAttribute<RequiredAttribute>();

                if (metadata == null)
                {
                    if (!methodIsStrict && !IsStrict)
                    {
                        continue;
                    }
                }

                var orAttribute = parameter.GetCustomAttribute<OrAttribute>();

                if (orAttribute != null)
                {
                    continue;
                }

                var names = GetParameterNames(parameter);
                if (context.ContainsAny(names, out object property))
                {
                    var val = property;
                    if (val != null && parameter.ParameterType.IsAssignableFrom(val.GetType()))
                    {
                        continue;
                    }
                }

                if (context.ServiceProvider != null)
                {
                    var valueFromProvider = context.ServiceProvider.GetService(parameter.ParameterType);
                    if (valueFromProvider != null)
                    {
                        if (context.Debug)
                        {
                            var methodName = method.GetName();
                            context.Debug("There is a property of type {0} found in service provider. It will be used to fill the parameter \"{1}\".".FormatWith(parameter.ParameterType, parameter.Name));
                        }
                        continue;
                    }
                }


                if (parameters.Count(param => param.ParameterType == parameter.ParameterType) == 1)
                {
                    if (bagTypes.ContainsKey(parameter.ParameterType))
                    {
                        if (context.Debug)
                        {
                            var methodName = method.GetName();
                            context.Debug("There is only one property of type {0}. It will be used to fill the parameter \"{1}\".".FormatWith(parameter.ParameterType, parameter.Name));
                        }
                        continue;
                    }

                    var singleAssignableType = bagTypes.Keys.SingleOrDefault(bagType => parameter.ParameterType.IsAssignableFrom(bagType));
                    if (singleAssignableType != null)
                    {
                        if (context.Debug)
                        {
                            var methodName = method.GetName();
                            context.Debug("There is only one property assignable to type {0}. It will be used to fill the parameter \"{1}\".".FormatWith(parameter.ParameterType, parameter.Name));
                        }
                        continue;
                    }
                }

                if (context.Debug)
                {
                    var formattedMessage = SkipMethodOnMissingPropertyMessage.FormatWith(parameter.Name, method.GetName(), method.DeclaringType.GetName());
                    var message = formattedMessage;

                    if (metadata != null)
                    {
                        if (metadata.Message.HasValue()) message = $"{formattedMessage} {metadata.Message}";
                    }
                    else if (methodIsStrict)
                    {
                        message = $"{formattedMessage} {MethodClaimsAllParameters}";
                    }
                    else if (IsStrict)
                    {
                        message = $"{formattedMessage} {ClassClaimsAllParameters}";
                    }

                    context.Debug(message);
                }

                context.End();

                return false;
            }

            return true;
        }

        protected virtual async Task CheckAndRunMethod(MethodInfo method, Bag bag)
        {
            if (bag.Debug)
            {
                var methodName = method.GetName();
                var methodDescription = method.GetDescription();
                if (methodDescription.HasValue())
                {
                    bag.Debug("Verifying parameters of method [{0}]. Method is {1}".FormatWith(methodName, methodDescription.ToLower()));
                }
                else
                {
                    bag.Debug("Verifying parameters of method [{0}].".FormatWith(methodName));
                }

                if (AllParametersAreValid(method, bag))
                {
                    bag.Debug("All parameters are valid. Running method [{0}].".FormatWith(methodName));
                    await RunMethod(method, bag).ConfigureAwait(false);
                    bag.Debug("Completed method [{0}].".FormatWith(methodName));
                }
                else
                {
                    bag.Debug("Method [{0}] cannot be run. Going to the next one.".FormatWith(methodName));
                }
            }
            else
            {
                if (AllParametersAreValid(method, bag))
                {
                    await RunMethod(method, bag).ConfigureAwait(false);
                }
            }
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
        public async Task Run(Bag bag)
        {
            if (Methods == null)
            {
                if (bag.Debug)
                {
                    bag.Debug("Methods collection in the processor [0] is null".FormatWith(this.Name));
                }

                return;
            }

            if (!Methods.Any())
            {
                if (bag.Debug)
                {
                    bag.Debug("Methods collection in the processor [0] is empty. Nothing will be executed.".FormatWith(this.Name));
                }

                return;
            }

            foreach (var method in Methods)
            {
                if (bag.Ended)
                {
                    break;
                }

                await CheckAndRunMethod(method, bag).ConfigureAwait(false);
            }
        }
    }
}
