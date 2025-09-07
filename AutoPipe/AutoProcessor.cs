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
    /// Provides a base processor that automatically discovers and executes methods marked with <see cref="RunAttribute"/>.
    /// Methods are executed in the order specified by <see cref="OrderAttribute"/> or by dependency analysis.
    /// </summary>
    public class AutoProcessor : IProcessor
    {
        /// <summary>
        /// Message format for skipping a method due to missing property.
        /// </summary>
        public static readonly string SkipMethodOnMissingPropertyMessage = "Property [{0}] is not found. Skipping method [{1}] in [{2}].";
        /// <summary>
        /// Message indicating that strict mode requires all parameters to be present for method execution.
        /// </summary>
        public static readonly string MethodClaimsAllParameters = "Run attribute of the current execution method contains attribute [Strict], which requires all parameters to be declared before execution.";
        /// <summary>
        /// Message indicating that strict mode requires all parameters to be present for class execution.
        /// </summary>
        public static readonly string ClassClaimsAllParameters = "Run attribute of your processor class contains attribute [Strict], which requires all parameters to be declared before execution.";

        /// <summary>
        /// Gets or sets the collection of methods to be executed by the processor.
        /// </summary>
        public IEnumerable<MethodInfo> Methods { get; set; }
        /// <summary>
        /// Gets the underlying processor instance.
        /// </summary>
        public object Processor { get; }

        /// <summary>
        /// Creates an <see cref="AutoProcessor"/> from an existing processor class instance.
        /// </summary>
        /// <param name="processorClass">The processor class instance.</param>
        /// <returns>An <see cref="IProcessor"/> wrapping the given instance.</returns>
        public static IProcessor From(object processorClass)
        {
            return new AutoProcessor(processorClass);
        }

        /// <summary>
        /// Creates an <see cref="AutoProcessor"/> from a new instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The processor class type.</typeparam>
        /// <returns>An <see cref="IProcessor"/> wrapping the new instance.</returns>
        public static IProcessor From<T>() where T : class, new()
        {
            return new AutoProcessor(new T());
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AutoProcessor"/> with itself as the processor.
        /// </summary>
        protected AutoProcessor()
        {
            Processor = this;
            Methods = GetMethodsToExecute();
            IsStrict = Processor.GetType().IsStrict();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AutoProcessor"/> with the specified processor object.
        /// </summary>
        /// <param name="processor">The processor object to wrap.</param>
        public AutoProcessor(object processor)
        {
            Processor = processor;
            Methods = GetMethodsToExecute();
            IsStrict = Processor?.GetType().IsStrict() ?? false;
        }

        /// <summary>
        /// Discovers and returns the methods to be executed by this processor, based on attributes and filters.
        /// </summary>
        /// <returns>A sequence of <see cref="MethodInfo"/> objects to execute.</returns>
        public virtual IEnumerable<MethodInfo> GetMethodsToExecute()
        {
            if (Processor.HasNoValue() || Processor.GetType() == typeof(AutoProcessor))
            {
                return Enumerable.Empty<MethodInfo>();
            }

            var type = Processor.GetType();
            var allAttributes = GetMethodBindingAttributes().ToArray();

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

        /// <summary>
        /// Orders the discovered methods according to their dependencies and <see cref="OrderAttribute"/> values.
        /// </summary>
        /// <param name="methods">The methods to order.</param>
        /// <returns>An ordered sequence of <see cref="MethodInfo"/>.</returns>
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

        /// <summary>
        /// Calculates the order of a method based on its attributes and dependencies.
        /// </summary>
        /// <param name="method">The method to order.</param>
        /// <param name="orderedMethods">The current ordered list.</param>
        /// <param name="namesDictionary">Dictionary of method names to MethodInfo.</param>
        /// <param name="visitedMethods">Set of visited methods for cycle detection.</param>
        /// <param name="paramsDictionary">Dictionary of parameter names to generating methods.</param>
        /// <param name="methodParamsDictionary">Dictionary of methods to their parameter names.</param>
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
        /// <summary>
        /// Gets a value indicating whether all methods should be run regardless of attributes.
        /// </summary>
        protected virtual bool RunAll
        {
            get
            {
                return (runAll ?? (runAll = this.Processor.GetType().ShouldRunAll())).Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether strict parameter validation is enabled for this processor.
        /// </summary>
        protected virtual bool IsStrict { get; }

        /// <summary>
        /// Returns the binding flags used to discover executable methods in <see cref="GetMethodsToExecute"/>.
        /// </summary>
        /// <returns>A sequence of <see cref="BindingFlags"/> for method discovery.</returns>
        protected virtual IEnumerable<BindingFlags> GetMethodBindingAttributes()
        {
            yield return Repository.RunningMethodsFlags;
        }

        /// <summary>
        /// Determines if a method is eligible for execution based on attributes and filters.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <returns><c>true</c> if the method should be executed; otherwise, <c>false</c>.</returns>
        public virtual bool AcceptableByFilter(MethodInfo method)
        {
            return (this.RunAll || method.ShouldRun()) && !method.ShouldSkip();
        }

        /// <summary>
        /// Gets the explicit order value for a method, if specified by <see cref="OrderAttribute"/>.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <returns>The order value, or <c>null</c> if not specified.</returns>
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
        /// Executes a method using parameters resolved from the pipeline context.
        /// Handles the result and updates the context as needed.
        /// </summary>
        /// <param name="method">The method to execute.</param>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual async Task RunMethod(MethodInfo method, Bag context)
        {
            var values = GetExecutionParameters(method, context);
            var result = method.Invoke(Processor, values.ToArray());
            await ProcessResult(method, context, result, skipNameBasedActions: false).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets identifiers for methods that update properties in the context.
        /// </summary>
        /// <returns>A sequence of update identifiers.</returns>
        protected virtual IEnumerable<string> GetPropertyUpdateIdentifiers()
        {
            yield return "Set";
            yield return "Update";
            yield return "Overwrite";
        }

        /// <summary>
        /// Gets identifiers for methods that ensure properties exist in the context.
        /// </summary>
        /// <returns>A sequence of ensure identifiers.</returns>
        protected virtual IEnumerable<string> GetPropertyEnsureIdentifiers()
        {
            yield return "Get";
            yield return "Ensure";
            yield return "Add";
        }

        /// <summary>
        /// Executes an action based on method name prefixes and provided identifiers.
        /// </summary>
        /// <param name="method">The method to check.</param>
        /// <param name="actions">The list of action identifiers.</param>
        /// <param name="executor">The action to execute if a match is found.</param>
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
        /// Processes the result of a method execution, handling various result types and updating the context.
        /// </summary>
        /// <param name="method">The method that produced the result.</param>
        /// <param name="context">The pipeline context.</param>
        /// <param name="methodResult">The result to process.</param>
        /// <param name="skipNameBasedActions">Whether to skip name-based property actions.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Processes a lambda expression result and updates the context accordingly.
        /// </summary>
        /// <param name="method">The method that produced the expression.</param>
        /// <param name="bag">The pipeline context.</param>
        /// <param name="expression">The lambda expression to process.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual async Task ProcessExpression(MethodInfo method, Bag bag, LambdaExpression expression)
        {
            var result = bag.Map(expression);

            await this.ProcessResult(method, bag, result, false);
        }

        /// <summary>
        /// Adds or updates properties in the context from the given object's properties.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <param name="propertyContainer">The object whose properties are added.</param>
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
        /// Awaits a task and processes its result, updating the context as needed.
        /// </summary>
        /// <param name="method">The method that produced the task.</param>
        /// <param name="context">The pipeline context.</param>
        /// <param name="task">The task to process.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// Returns an action that adds an informational message to the context.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <returns>An action for adding information.</returns>
        protected virtual Action<Bag> Info(string message)
        {
            return context => context.Info(message);
        }

        /// <summary>
        /// Returns an action that adds a warning message to the context.
        /// </summary>
        /// <param name="message">The warning message.</param>
        /// <returns>An action for adding a warning.</returns>
        protected virtual Action<Bag> Warning(string message)
        {
            return context => context.Warning(message);
        }

        /// <summary>
        /// Returns an action that adds an error message to the context.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>An action for adding an error.</returns>
        protected virtual Action<Bag> Error(string message)
        {
            return context => context.Error(message);
        }

        /// <summary>
        /// Returns an action that adds multiple messages to the context.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        /// <returns>An action for adding messages.</returns>
        protected virtual Action<Bag> AddMessages(params PipelineMessage[] messages)
        {
            return context => context.AddMessages(messages);
        }

        /// <summary>
        /// Returns an action that ends the pipeline with an error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>An action for ending the pipeline with error.</returns>
        protected virtual Action<Bag> ErrorEnd(string message)
        {
            return context => context.ErrorEnd(message);
        }

        /// <summary>
        /// Returns an action that ends the pipeline with a warning message.
        /// </summary>
        /// <param name="message">The warning message.</param>
        /// <returns>An action for ending the pipeline with warning.</returns>
        protected virtual Action<Bag> WarningEnd(string message)
        {
            return context => context.WarningEnd(message);
        }

        /// <summary>
        /// Returns an action that ends the pipeline with an informational message.
        /// </summary>
        /// <param name="message">The informational message.</param>
        /// <returns>An action for ending the pipeline with information.</returns>
        protected virtual Action<Bag> InfoEnd(string message)
        {
            return context => context.InfoEnd(message);
        }

        /// <summary>
        /// Returns an action that ends the pipeline.
        /// </summary>
        /// <returns>An action for ending the pipeline.</returns>
        protected virtual Action<Bag> End()
        {
            return context => context.End();
        }

        /// <summary>
        /// Returns an action that ends the pipeline and sets a result value.
        /// </summary>
        /// <param name="result">The result to set.</param>
        /// <returns>An action for ending the pipeline with a result.</returns>
        protected virtual Action<Bag> EndResult(object result)
        {
            return context => context.EndResult(result);
        }

        /// <summary>
        /// Returns an action that sets a result value in the context.
        /// </summary>
        /// <param name="result">The result to set.</param>
        /// <returns>An action for setting the result.</returns>
        protected virtual Action<Bag> Result(object result)
        {
            return context => context.SetResult(result);
        }

        /// <summary>
        /// Returns an action that ends the pipeline with an informational message and result.
        /// </summary>
        /// <param name="result">The result to set.</param>
        /// <param name="message">The informational message.</param>
        /// <returns>An action for ending with info and result.</returns>
        protected virtual Action<Bag> InfoEndResult(object result, string message)
        {
            return context => context.InfoEndResult(result, message);
        }

        /// <summary>
        /// Returns an action that ends the pipeline with a warning message and result.
        /// </summary>
        /// <param name="result">The result to set.</param>
        /// <param name="message">The warning message.</param>
        /// <returns>An action for ending with warning and result.</returns>
        protected virtual Action<Bag> WarningEndResult(object result, string message)
        {
            return context => context.WarningEndResult(result, message);
        }

        /// <summary>
        /// Returns an action that ends the pipeline with an error message and result.
        /// </summary>
        /// <param name="result">The result to set.</param>
        /// <param name="message">The error message.</param>
        /// <returns>An action for ending with error and result.</returns>
        protected virtual Action<Bag> ErrorEndResult(object result, string message)
        {
            return context => context.ErrorEndResult(result, message);
        }

        /// <summary>
        /// Returns an action that ends the pipeline with an informational message and no result.
        /// </summary>
        /// <param name="message">The informational message.</param>
        /// <returns>An action for ending with info and no result.</returns>
        protected virtual Action<Bag> InfoEndNoResult(string message)
        {
            return context => context.InfoEndNoResult(message);
        }

        /// <summary>
        /// Returns an action that ends the pipeline with a warning message and no result.
        /// </summary>
        /// <param name="message">The warning message.</param>
        /// <returns>An action for ending with warning and no result.</returns>
        protected virtual Action<Bag> WarningEndNoResult(string message)
        {
            return context => context.WarningEndNoResult(message);
        }

        /// <summary>
        /// Returns an action that ends the pipeline with an error message and no result.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>An action for ending with error and no result.</returns>
        protected virtual Action<Bag> ErrorEndNoResult(string message)
        {
            return context => context.ErrorEndNoResult(message);
        }

        /// <summary>
        /// Returns an action that sets an informational result in the context.
        /// </summary>
        /// <param name="result">The result to set.</param>
        /// <param name="message">The informational message.</param>
        /// <returns>An action for setting info result.</returns>
        protected virtual Action<Bag> InfoResult(object result, string message)
        {
            return context => context.InfoResult(result, message);
        }

        /// <summary>
        /// Returns an action that sets a warning result in the context.
        /// </summary>
        /// <param name="result">The result to set.</param>
        /// <param name="message">The warning message.</param>
        /// <returns>An action for setting warning result.</returns>
        protected virtual Action<Bag> WarningResult(object result, string message)
        {
            return context => context.WarningResult(result, message);
        }

        /// <summary>
        /// Returns an action that sets an error result in the context.
        /// </summary>
        /// <param name="result">The result to set.</param>
        /// <param name="message">The error message.</param>
        /// <returns>An action for setting error result.</returns>
        protected virtual Action<Bag> ErrorResult(object result, string message)
        {
            return context => context.ErrorResult(result, message);
        }

        /// <summary>
        /// Gets the primary name of the processor.
        /// </summary>
        public virtual string Name => this.Names.First();

        /// <summary>
        /// Gets all names (aliases) of the processor.
        /// </summary>
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

        /// <summary>
        /// Gets the description of the processor.
        /// </summary>
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
        /// Resolves parameter values for a method from the pipeline context, using reflection and service provider.
        /// </summary>
        /// <param name="method">The method to resolve parameters for.</param>
        /// <param name="context">The pipeline context.</param>
        /// <returns>A sequence of parameter values in method parameter order.</returns>
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

        /// <summary>
        /// Gets all possible names (including aliases) for a method parameter.
        /// </summary>
        /// <param name="parameter">The parameter to get names for.</param>
        /// <returns>A sequence of parameter names and aliases.</returns>
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
        /// Validates that all required parameters for a method are present in the pipeline context.
        /// </summary>
        /// <param name="method">The method to validate.</param>
        /// <param name="context">The pipeline context.</param>
        /// <returns><c>true</c> if all parameters are valid; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Checks parameters and executes the method if valid, logging debug information as appropriate.
        /// </summary>
        /// <param name="method">The method to check and run.</param>
        /// <param name="bag">The pipeline context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// Executes all discovered methods in order, using the pipeline context for parameter resolution and result handling.
        /// </summary>
        /// <param name="bag">The pipeline context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
