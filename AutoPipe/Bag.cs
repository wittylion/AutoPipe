using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace AutoPipe
{
    /// <summary>
    /// Introduces possibility to keep context information
    /// about the flow of the pipeline. By default it has
    /// messages collection which can be accessed by using
    /// <see cref="MessageObjects"/> method and a flag
    /// <see cref="Ended"/> identifying whether pipeline was ended.
    /// </summary>
    [Serializable]
    public class Bag : ISerializable, IDisposable, IDictionary<string, object>
    {
        /// <summary>
        /// Creates a new Bag that has a parameter-less constructor.
        /// </summary>
        /// <returns>
        /// A new pipeline context.
        /// </returns>
        public static Bag Create()
        {
            return new Bag();
        }

        public static Bag Copy(Bag bag, bool includeMessages = false)
        {
            return bag.Copy(includeMessages);
        }

        /// <summary>
        /// Creates a new <see cref="PipelineContext"/> with
        /// properties of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <typeparam name="TProperties">The type of property container.</typeparam>
        /// <param name="propertyContainer">
        /// Object which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an object passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static Bag Create(object propertyContainer)
        {
            return Bag.CreateFromProperties(propertyContainer);
        }

        /// <summary>
        /// Creates a new <see cref="PipelineContext"/> with properties composed from
        /// keys and values of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of values of the dictionary.</typeparam>
        /// <param name="propertyContainer">
        /// Dictionary which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an dictionary passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static Bag Create<TValue>(IDictionary<string, TValue> propertyContainer)
        {
            return Bag.CreateFromDictionary(propertyContainer);
        }

        /// <summary>
        /// Creates a new Bag or derived type that has
        /// a parameter-less constructor.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the pipeline context that is derived from
        /// <see cref="PipelineContext"/> and has a parameter-less constructor.
        /// </typeparam>
        /// <returns>
        /// A new pipeline context.
        /// </returns>
        public static TContext Create<TContext>() where TContext : Bag, new()
        {
            return new TContext();
        }

        /// <summary>
        /// Creates a new <see cref="TContext"/> with properties composed from
        /// keys and values of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the pipeline context that is derived from
        /// <see cref="PipelineContext"/> and has a parameter-less constructor.
        /// </typeparam>
        /// <typeparam name="TValue">The type of values of the dictionary.</typeparam>
        /// <param name="propertyContainer">
        /// Dictionary which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an dictionary passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static TContext Create<TContext, TValue>(IDictionary<string, TValue> propertyContainer) where TContext : Bag, new()
        {
            var context = Create<TContext>();
            if (propertyContainer.HasValue())
            {
                foreach (var item in propertyContainer)
                {
                    context.Set(item.Key, item.Value);
                }
            }
            return context;
        }

        /// <summary>
        /// Creates a new <see cref="PipelineContext"/> with
        /// properties of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <param name="propertyContainer">
        /// Object which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an object passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static Bag CreateFromProperties(object propertyContainer)
        {
            return new Bag(propertyContainer);
        }

        /// <summary>
        /// Creates a new <see cref="PipelineContext"/> with properties composed from
        /// keys and values of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of values of the dictionary.</typeparam>
        /// <param name="propertyContainer">
        /// Dictionary which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an dictionary passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static Bag CreateFromDictionary<TValue>(IDictionary<string, TValue> propertyContainer)
        {
            var context = new Bag();
            if (propertyContainer.HasValue())
            {
                foreach (var item in propertyContainer)
                {
                    context.Set(item.Key, item.Value);
                }
            }
            return context;
        }

        public event EventHandler<PipelineMessage> OnMessage;

        /// <summary>
        /// Flag identifying whether pipeline must be ended/stopped,
        /// it can be used as a cancelation identifier for the execution flow.
        /// </summary>
        public bool Ended
        {
            get => Get(EndedProperty, false);
            set => SetProperty(EndedProperty, value);
        }

        public bool Debug
        {
            get => Get(DebugProperty, DebugDefault);
            set => SetProperty(DebugProperty, value);
        }

        public bool ThrowOnMissing
        {
            get => Get(ThrowOnMissingProperty, ThrowOnMissingDefault);
            set => SetProperty(ThrowOnMissingProperty, value);
        }

        public void Dispose()
        {
            if (PropertiesDictionary.IsValueCreated)
            {
                var disposables = PropertiesDictionary.Value.Values.OfType<IDisposable>();
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }

                PropertiesDictionary.Value.Clear();
            }

            if (MessagesCollection.IsValueCreated)
            {
                MessagesCollection.Value.Clear();
            }

            OnMessage = null;
        }

        /// <summary>
        /// Collection of messages that keeps all the text passed to the
        /// context during the pipeline execution.
        /// </summary>
        protected Lazy<ICollection<PipelineMessage>> MessagesCollection { get; } =
            new Lazy<ICollection<PipelineMessage>>(() => new PipelineMessageCollection());

        /// <summary>
        /// Collection of the properties that contains all the collected
        /// or obtained values during pipeline execution or before
        /// execution is started <see cref="PipelineContext(object)"/>.
        /// </summary>
        protected Lazy<Dictionary<string, object>> PropertiesDictionary { get; } = new Lazy<Dictionary<string, object>>(() =>
            new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase));

        public ICollection<string> Keys => PropertiesDictionary.IsValueCreated ? PropertiesDictionary.Value.Keys : (ICollection<string>)Enumerable.Empty<string>();

        public ICollection<object> Values => PropertiesDictionary.IsValueCreated ? PropertiesDictionary.Value.Values.ToList() : (ICollection<object>)Enumerable.Empty<object>();

        public int Count => PropertiesDictionary.IsValueCreated ? PropertiesDictionary.Value.Count : 0;

        public bool IsReadOnly => false;

        public object this[string key]
        {
            get => this.Get<object>(key);
            set => this.SetProperty(key, value);
        }

        /// <summary>
        /// Applies property to the context. Depending on
        /// <paramref name="modificator"/> adds or updates
        /// value of the property.
        /// </summary>
        /// <typeparam name="TValue">
        /// Parameter of the value to be applied.
        /// </typeparam>
        /// <param name="name">
        /// The name of the property to be applied.
        /// </param>
        /// <param name="value">
        /// The value of the property to be applied.
        /// </param>
        /// <param name="modificator">
        /// Modificator that specifies whether the property has to be
        /// just added without overwriting the existing one or updated
        /// with a new value.
        /// </param>
        public virtual void ApplyProperty<TValue>(string name, TValue value, PropertyModificator modificator)
        {
            switch (modificator)
            {
                case PropertyModificator.UpdateValue:
                    this.Set(name, value);
                    break;

                case PropertyModificator.SkipIfExists:
                default:
                    this.Set(name, value, skipIfExists: true);
                    break;
            }
        }

        /// <summary>
        /// Adds the property to the collection <see cref="PropertiesDictionary"/>
        /// or updates the value if key of parameter <paramref name="name"/>
        /// has been added previously (alias to <see cref="UpdateOrAddProperty{TValue}"/>).
        /// </summary>
        /// <remarks>
        /// Parameter name will be used in case-insensetive way.
        /// It means that if you previously added property name "MESSAGE"
        /// it will be updated if you pass to this method property name "message".
        /// </remarks>
        /// <typeparam name="TValue">
        /// The type of the added value.
        /// </typeparam>
        /// <param name="name">
        /// Key to identify the property (case-insensetive).
        /// </param>
        /// <param name="value">
        /// The value to be kept under the <paramref name="name"/> of the property.
        /// </param>
        public virtual void SetProperty<TValue>(string name, TValue value, bool skipIfExists = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), $"You cannot set null value properties. Please check the \"{name}\" parameter.");
            }

            var dictionary = PropertiesDictionary.Value;
            if (!dictionary.ContainsKey(name))
            {
                dictionary.Add(name, value);
            }
            else
            {
                if (!skipIfExists)
                {
                    dictionary[name] = value;
                }
            }
        }

        public virtual TValue Get<TValue>(string name)
        {
            if (ThrowOnMissing)
            {
                return this.GetOrThrow<TValue>(name);
            }

            return Get(name, or: () => default(TValue));
        }

        /// <summary>
        /// Retrieves the value that is defined under the property
        /// of parameter <paramref name="name"/> or if was not added
        /// or the type of the contained property is different than
        /// <see cref="TValue"/>, the <paramref name="or"/> will be retrieved.
        /// </summary>
        /// <typeparam name="TValue">
        /// The type of the retrieved value.
        /// </typeparam>
        /// <param name="name">
        /// Key to identify the property (case-insensetive).
        /// </param>
        /// <param name="or">
        /// Default value to be retrieved if the value of the property
        /// was not added or the type of the value is different than <see cref="TValue"/>.
        /// </param>
        /// <returns>
        /// The value kept under the <paramref name="name"/> of the property
        /// or <paramref name="or"/> if property was not added or the type
        /// of the value is different than <see cref="TValue"/>.
        /// </returns>
        public virtual TValue Get<TValue>(string name, TValue or)
        {
            return Get(name, or: () => or);
        }

        public virtual TValue GetOrThrow<TValue>(string name)
        {
            return Get<TValue>(name, or: () =>
            {
                var summaryMessage = Summary();
                throw new ArgumentOutOfRangeException(nameof(name), $"The property \"{name}\" was not added to the Pipeline context. Try to go through messages:\r\n{summaryMessage}");
            });
        }

        public virtual string String(string name)
        {
            if (Has(name, out string value))
            {
                return value;
            }

            return string.Empty;
        }

        public virtual int Int(string name)
        {
            if (Has(name, out int value))
            {
                return value;
            }

            return 0;
        }

        public virtual bool Bool(string name)
        {
            if (Has(name, out bool value))
            {
                return value;
            }

            return false;
        }

        public virtual List<TElement> List<TElement>(string name)
        {
            return Get(name, or: Enumerable.Empty<TElement>()).ToList();
        }

        public virtual TElement[] Array<TElement>(string name)
        {
            return Get(name, or: new TElement[0]);
        }

        public virtual TValue Get<TValue>(string name, Func<TValue> or)
        {
            if (PropertiesDictionary.IsValueCreated && PropertiesDictionary.Value.TryGetValue(name, out object maybeValue))
            {
                if (maybeValue is TValue value)
                {
                    return value;
                }

                if (maybeValue is ComputedProperty computed && typeof(TValue).IsAssignableFrom(computed.Lambda.ReturnType))
                {
                    return computed.Invoke<TValue>(this);
                }
            }

            return or();
        }

        /// <summary>
        /// The function returns boolean value of specified type
        /// exists in collection of properties.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Type of the property to be stored in the collection.
        /// </typeparam>
        /// <param name="name">
        /// The name of the property to be checked for presence.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> in case property of specified type exists,
        /// otherwise returns <c>false</c>.
        /// </returns>
        public virtual bool Has<TProperty>(string name)
        {
            return Contains<TProperty>(name);
        }

        public virtual bool Has(string name)
        {
            return Contains(name);
        }

        public virtual bool Has<TProperty>(string name, out TProperty property)
        {
            return Contains(name, out property);
        }

        /// <summary>
        /// The function returns boolean value that indicates whether property
        /// of specified type exists in collection of properties.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Type of the property to be stored in the collection.
        /// </typeparam>
        /// <param name="name">
        /// The name of the property to be checked for presence.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> in case property of specified type exists,
        /// otherwise returns <c>false</c>.
        /// </returns>
        public virtual bool Contains<TProperty>(string name)
        {
            return PropertiesDictionary.IsValueCreated &&
                PropertiesDictionary.Value.TryGetValue(name, out object foundValue) &&
                (foundValue is TProperty || foundValue is ComputedProperty computed && typeof(TProperty).IsAssignableFrom(computed.Lambda.ReturnType));
        }

        public virtual bool Contains(string name)
        {
            return ContainsKey(name);
        }

        public virtual bool ContainsSingle(Type type, out object valueOfType)
        {
            var bagTypes = this.GetSingleTypeValues();

            if (bagTypes.TryGetValue(type, out valueOfType))
            {
                return true;
            }

            return false;
        }

        public virtual bool Contains<TProperty>(string name, out TProperty value)
        {
            value = default(TProperty);
            if (!PropertiesDictionary.IsValueCreated)
            {
                return false;
            }

            if (!PropertiesDictionary.Value.TryGetValue(name, out object foundValue))
            {
                return false;
            }

            if (foundValue is TProperty result)
            {
                value = result;
                return true;
            }

            if (foundValue is ComputedProperty computed && typeof(TProperty).IsAssignableFrom(computed.Lambda.ReturnType))
            {
                value = computed.Invoke<TProperty>(this);
                return true;
            }

            return false;
        }

        public virtual bool ContainsAny<TProperty>(IEnumerable<string> names, out TProperty value)
        {
            value = default;

            foreach (var name in names)
            {
                if (Contains(name, out TProperty val))
                {
                    value = val;
                    return true;
                }
            }

            return false;
        }

        public virtual bool ContainsSingle<TProperty>(out TProperty value)
        {
            value = default;

            if (!PropertiesDictionary.IsValueCreated)
            {
                return false;
            }

            var types = PropertiesDictionary.Value.Values.OfType<TProperty>().Take(2);
            if (types.Count() == 1)
            {
                value = types.First();
                return true;
            }

            return false;
        }

        /// <summary>
        /// The function returns boolean value that indicates whether property
        /// of specified type is missing in collection of properties.
        /// </summary>
        /// <typeparam name="TProperty">
        /// Type of the property to check in the collection.
        /// </typeparam>
        /// <param name="name">
        /// The name of the property to be checked for absence.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> in case property of specified type is missing,
        /// otherwise returns <c>false</c>.
        /// </returns>
        public virtual bool DoesNotContain<TProperty>(string name)
        {
            return !Contains<TProperty>(name);
        }

        public virtual bool DoesNotContain(string name)
        {
            return !Contains(name);
        }

        /// <summary>
        /// Deletes a property with the
        /// specified <paramref name="name"/> from the context.
        /// </summary>
        /// <param name="name">
        /// The name of the property to be deleted.
        /// </param>
        public virtual bool DeleteProperty(string name)
        {
            if (PropertiesDictionary.IsValueCreated)
            {
                var dictionary = PropertiesDictionary.Value;
                return dictionary.Remove(name);
            }

            return false;
        }

        public virtual bool DeleteProperty<TElement>(string name, out TElement element)
        {
            element = default;

            if (PropertiesDictionary.IsValueCreated)
            {
                var dictionary = PropertiesDictionary.Value;
                if (dictionary.TryGetValue(name, out object prop))
                {
                    if (prop is TElement result)
                    {
                        element = result;
                    }
                    else if (prop is ComputedProperty computed && typeof(TElement).IsAssignableFrom(computed.Lambda.ReturnType))
                    {
                        element = computed.Invoke<TElement>(this);
                    }

                    return dictionary.Remove(name);
                }
            }

            return false;
        }

        /// <summary>
        /// Method returns messages of the context according to the passed
        /// parameter <paramref name="filter"/>. This method is more flexible
        /// than other message returning methods of this class because you
        /// can specify your own filter.
        /// </summary>
        /// <param name="filter">
        /// Filter of type <see cref="MessageFilter"/> that allows
        /// to specify what kind of messages will be returned from the method.
        /// </param>
        /// <example>
        ///
        /// Bag context = new Bag();
        /// 
        /// context.AddInformation("The request was sent successfully");
        /// context.AddWarning("Could not recognize the id of the site, continued with the default");
        /// context.AddError("Request to the database failed, review the connection string");
        ///
        /// context.GetMessages(MessageFilter.Informations);
        /// context.GetMessages(MessageFilter.Informations | MessageFilter.Errors);
        /// context.GetMessages(MessageFilter.Errors| MessageFilter.Warnings);
        /// context.GetMessages(MessageFilter.All);
        /// 
        /// </example>
        /// <returns>
        /// An array of messages that belong to the context of pipeline,
        /// filtered specifically to parameter <paramref name="filter"/>.
        /// </returns>
        public virtual PipelineMessage[] MessageObjects(MessageFilter filter)
        {
            if (MessagesCollection.IsValueCreated && MessagesCollection.Value.Count > 0)
            {
                if (filter == MessageFilter.All)
                {
                    return MessagesCollection.Value.ToArray();
                }
                return MessagesCollection.Value.Where(message => ((int)message.MessageType & (int)filter) > 0).ToArray();
            }
            return new PipelineMessage[0];
        }

        /// <summary>
        /// Returns all messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations, warnings and errors.
        /// </summary>
        /// <returns>
        /// All messages of the context, that have been added
        /// during pipeline execution.
        /// </returns>
        public virtual PipelineMessage[] MessageObjects()
        {
            return this.MessageObjects(MessageFilter.All);
        }

        /// <summary>
        /// Returns all text messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations, warnings and errors.
        /// </summary>
        /// <returns>
        /// All text messages of the context, that have been added
        /// during pipeline execution.
        /// </returns>
        public virtual string[] Messages()
        {
            return this.MessageObjects()
                .Select(messageContainer => messageContainer.Message)
                .ToArray();
        }

        /// <summary>
        /// Returns filtered text messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations, warnings and errors.
        /// </summary>
        /// <param name="filter">
        /// A filter for the message collection.
        /// </param>
        /// <returns>
        /// Filtered text messages of the context, that have been added
        /// during pipeline execution.
        /// </returns>
        public virtual string[] Messages(MessageFilter filter)
        {
            return this.MessageObjects(filter)
                .Select(messageContainer => messageContainer.Message)
                .ToArray();
        }

        /// <summary>
        /// Returns filtered text messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations, warnings and errors.
        /// </summary>
        /// <param name="filter">
        /// A filter for the message collection.
        /// </param>
        /// <param name="format">
        /// Function that accepts <see cref="PipelineMessage.Message"/>
        /// and <see cref="PipelineMessage.MessageType"/> and returns a
        /// formatted value to be returned by this function.
        /// </param>
        /// <returns>
        /// Filtered text messages of the context, that have been added
        /// during pipeline execution.
        /// </returns>
        public virtual string[] Messages(MessageFilter filter, Func<string, MessageType, string> format)
        {
            if (format.HasNoValue())
            {
                return this.Messages(filter);
            }

            return this.MessageObjects(filter)
                .Select(messageContainer =>
                    format(messageContainer.Message, messageContainer.MessageType))
                .ToArray();
        }

        /// <summary>
        /// Returns filtered text messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations, warnings and errors.
        /// </summary>
        /// <param name="filter">
        /// A filter for the message collection.
        /// </param>
        /// <param name="format">
        /// Function that accepts <see cref="PipelineMessage.Message"/>
        /// and <see cref="PipelineMessage.MessageType"/> and returns a
        /// formatted value to be returned by this function.
        /// </param>
        /// <returns>
        /// Filtered text messages of the context, that have been added
        /// during pipeline execution.
        /// </returns>
        public virtual string[] Messages(MessageFilter filter, Func<PipelineMessage, string> format)
        {
            if (format.HasNoValue())
            {
                return this.Messages(filter);
            }

            return this.MessageObjects(filter)
                .Select(messageContainer => format(messageContainer))
                .ToArray();
        }

        /// <summary>
        /// Returns all text messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations, warnings and errors.
        /// </summary>
        /// <param name="format">
        /// Function that accepts <see cref="PipelineMessage.Message"/>
        /// and <see cref="PipelineMessage.MessageType"/> and returns a
        /// formatted value to be returned by this function.
        /// </param>
        /// <returns>
        /// All text messages of the context, that have been added
        /// during pipeline execution.
        /// </returns>
        public virtual string[] Messages(Func<string, MessageType, string> format)
        {
            if (format.HasNoValue())
            {
                return this.Messages();
            }

            return this.MessageObjects()
                .Select(messageContainer =>
                    format(messageContainer.Message, messageContainer.MessageType))
                .ToArray();
        }

        /// <summary>
        /// Returns all text messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations, warnings and errors.
        /// </summary>
        /// <param name="format">
        /// Function that accepts <see cref="PipelineMessage.Message"/>
        /// and <see cref="PipelineMessage.MessageType"/> and returns a
        /// formatted value to be returned by this function.
        /// </param>
        /// <returns>
        /// All text messages of the context, that have been added
        /// during pipeline execution.
        /// </returns>
        public virtual string[] Messages(Func<PipelineMessage, string> format)
        {
            if (format.HasNoValue())
            {
                return this.Messages();
            }

            return this.MessageObjects()
                .Select(messageContainer => format(messageContainer))
                .ToArray();
        }

        /// <summary>
        /// Produces a string of joined texts of message collection.
        /// </summary>
        /// <param name="separator">
        /// A separator to join the texts of the messages.
        /// </param>
        /// <returns>
        /// Returns a string of joined texts of message collection.
        /// </returns>
        public virtual string Summary(
            string separator = null, MessageFilter filter = MessageFilter.All,
            Func<PipelineMessage, string> format = null)
        {
            string[] texts = this.Messages(filter, format);

            separator = separator ?? Environment.NewLine;

            return string.Join(separator, texts);
        }

        /// <summary>
        /// Returns messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations and warnings.
        /// </summary>
        /// <returns>
        /// Information and warning messages of the context,
        /// that have been added during pipeline execution.
        /// </returns>
        public virtual string[] InfosAndWarnings()
        {
            return this.Messages(MessageFilter.InfoWarning);
        }

        /// <summary>
        /// Returns messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: warnings and errors.
        /// </summary>
        /// <returns>
        /// Warning and error messages of the context,
        /// that have been added during pipeline execution.
        /// </returns>
        public virtual string[] WarningsAndErrors()
        {
            return this.Messages(MessageFilter.WarningError);
        }

        /// <summary>
        /// Returns information messages that have been added during
        /// pipeline execution to the context. It might contain
        /// some useful information about the status of the
        /// pipeline that has been running.
        /// </summary>
        /// <returns>
        /// Information messages of the pipeline execution context.
        /// </returns>
        public virtual string[] Infos()
        {
            return this.Messages(MessageFilter.Info);
        }

        /// <summary>
        /// Returns warning messages that have been added during
        /// pipeline execution to the context. It might contain
        /// some messages about the behavior that was not expected
        /// while pipeline has been running.
        /// </summary>
        /// <returns>
        /// Warning messages of the pipeline execution context.
        /// </returns>
        public virtual string[] Warnings()
        {
            return this.Messages(MessageFilter.Warning);
        }

        /// <summary>
        /// Returns error messages that have been added during
        /// pipeline execution to the context. It might contain
        /// messages about the things that went wrong and caused
        /// lack of the expected result while pipeline was running.
        /// </summary>
        /// <returns>
        /// Error messages of the pipeline execution context.
        /// </returns>
        public virtual string[] Errors()
        {
            return this.Messages(MessageFilter.Error);
        }

        /// <summary>
        /// Ends pipeline by setting a flag <see cref="Ended"/> to true.
        /// It allows to tell all the other users of this context that pipeline
        /// cannot be run further.
        /// </summary>
        public virtual void EndPipeline()
        {
            Ended = true;
        }

        /// <summary>
        /// Adds message object to the context, which allows all the users
        /// of the context to see the status of the current operation.
        /// This method is more flexible than other methods adding messages,
        /// because allows to specify custom <see cref="PipelineMessage"/> object.
        /// </summary>
        /// <param name="message">
        /// A pipeline message object, that contains a text and a value of the message.
        /// </param>
        public virtual void AddMessage(PipelineMessage message)
        {
            MessagesCollection.Value.Add(message);
            OnMessage?.Invoke(this, message);
        }

        /// <summary>
        /// Adds message objects to the context, which allows all the users
        /// of the context to see the status of the current operation.
        /// This method might be useful when you are copying messages
        /// from another context, or adding messages in scope.
        /// </summary>
        /// <param name="messages">
        /// Pipeline message collection, that contains <see cref="PipelineMessage"/>
        /// objects indicating the status of the operation.
        /// </param>
        public virtual void AddMessages(IEnumerable<PipelineMessage> messages)
        {
            foreach (var message in messages.EnsureAtLeastEmpty())
            {
                this.Message(message);
            }
        }

        /// <summary>
        /// Default parameterless constructor allowing you to create and use pipeline context.
        /// </summary>
        public Bag(bool? debug = null, bool? throwOnMissing = null, EventHandler<PipelineMessage> onMessage = null)
        {
            if (debug != null)
            {
                this.Debug = debug.Value;
            }

            if (throwOnMissing != null)
            {
                this.ThrowOnMissing = throwOnMissing.Value;
            }

            if (onMessage != null)
            {
                this.OnMessage += onMessage;
            }
        }

        public Bag(object propertyContainer, bool? debug = null, bool? throwOnMissing = null, EventHandler<PipelineMessage> onMessage = null) : this(debug: debug, throwOnMissing: throwOnMissing, onMessage: onMessage)
        {
            if (propertyContainer.HasValue())
            {
                foreach (var prop in propertyContainer.GetType().GetProperties())
                {
                    this.PropertiesDictionary.Value.Add(prop.Name, prop.GetValue(propertyContainer, null));
                }
            }
        }

        /// <summary>
        /// Constructor that is used by serialization and can dynamically
        /// recreate your pipeline context from serialization information.
        /// </summary>
        /// <param name="info">
        /// Serialization information, containing data of the serialized object.
        /// </param>
        /// <param name="context">Streaming context.</param>
        protected Bag(SerializationInfo info, StreamingContext context)
        {
        }

        public Bag Copy(bool includeMessages = false)
        {
            var result = CreateFromDictionary(this);
            if (includeMessages && MessagesCollection.IsValueCreated)
            {
                result.AddMessages(MessagesCollection.Value);
            }

            return result;
        }

        /// <summary>
        /// This method is used by serialization to convert an object to a
        /// text representation, including all properties and fields.
        /// </summary>
        /// <param name="info">
        /// Serialization information, containing data of the serialized object.
        /// </param>
        /// <param name="context">
        /// Streaming context.
        /// </param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue($"{nameof(Bag)}.{nameof(Ended)}", Ended);
            info.AddValue($"{nameof(Bag)}.{nameof(MessagesCollection)}", MessagesCollection, typeof(ICollection<PipelineMessage>));
        }

        public static readonly bool DebugDefault = false;
        public static readonly bool ThrowOnMissingDefault = true;

        public static readonly string EndedProperty = "ended";
        public static readonly string DebugProperty = "debug";
        public static readonly string ThrowOnMissingProperty = "throwonmissing";
        public static readonly string ResultProperty = "result";

        /// <summary>
        /// Returns a value of the result property.
        /// </summary>
        /// <remarks>
        /// Can be null. If this property is null, it means that it was not set
        /// or set value was invalid.
        /// </remarks>
        /// <returns>Value of the result property.</returns>
        public TResult ResultOrThrow<TResult>()
        {
            return this.GetOrThrow<TResult>(ResultProperty);
        }

        public string StringResult()
        {
            return this.String(ResultProperty);
        }

        public List<TElement> ListResult<TElement>()
        {
            return this.List<TElement>(ResultProperty);
        }

        /// <summary>
        /// In case the value of the result is null, you can specify a
        /// <paramref name="fallbackValue"/> which will be returned
        /// instead of the value in result property.
        /// </summary>
        /// <returns>
        /// Value of the result property or <paramref name="fallbackValue"/>
        /// if value of the result is null.
        /// </returns>
        public TResult GetResult<TResult>(TResult fallbackValue)
        {
            return this.Get(ResultProperty, fallbackValue);
        }

        /// <summary>
        /// In case the value of the result is null, you can specify a
        /// <paramref name="fallbackValue"/> which will be returned
        /// instead of the value in result property.
        /// </summary>
        /// <returns>
        /// Value of the result property or <paramref name="fallbackValue"/>
        /// if value of the result is null.
        /// </returns>
        public TResult GetResult<TResult>(Func<TResult> or)
        {
            return this.Get(ResultProperty, or);
        }

        /// <summary>
        /// Returns value indicating whether result is set.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> in case value exists,
        /// otherwise <c>false</c>.
        /// </returns>
        public virtual bool ContainsResult<TResult>()
        {
            return this.Contains<TResult>(ResultProperty);
        }

        public virtual bool ContainsResult<TResult>(out TResult result)
        {
            return this.Contains(ResultProperty, out result);
        }

        /// <summary>
        /// Returns value indicating whether result is missing,
        /// the value may be not specified or reset.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> in case result is missing,
        /// otherwise <c>false</c>.
        /// </returns>
        public virtual bool DoesNotContainResult<TResult>()
        {
            return !ContainsResult<TResult>();
        }

        public void Add(string key, object value)
        {
            this.SetProperty(key, value);
        }

        public bool ContainsKey(string key)
        {
            if (PropertiesDictionary.IsValueCreated)
            {
                return PropertiesDictionary.Value.ContainsKey(key);
            }

            return false;
        }

        public bool Remove(string key)
        {
            return this.DeleteProperty(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return Contains(key, out value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.Set(item.Key, item.Value);
        }

        public void Clear()
        {
            if (PropertiesDictionary.IsValueCreated)
            {
                PropertiesDictionary.Value.Clear();
            }
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return TryGetValue(item.Key, out object val) && val == item.Value;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            if (array == null) return;

            if (PropertiesDictionary.IsValueCreated)
            {
                var props = PropertiesDictionary.Value.Skip(arrayIndex);
                for (int i = 0; i < props.Count(); i++)
                {
                    if (i >= array.Length) break;
                    array[i] = props.ElementAt(i);
                }
            }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            if (Contains(item)) DeleteProperty(item.Key);
            return true;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            if (PropertiesDictionary.IsValueCreated)
            {
                foreach (var item in PropertiesDictionary.Value)
                {
                    yield return item;
                }
            }

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}