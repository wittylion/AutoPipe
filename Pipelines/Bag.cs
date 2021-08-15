using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Pipelines
{
    /// <summary>
    /// Introduces possibility to keep context information
    /// about the flow of the pipeline. By default it has
    /// messages collection which can be accessed by using
    /// <see cref="GetMessages"/> method and a flag
    /// <see cref="IsAborted"/> identifying whether pipeline was aborted.
    /// </summary>
    [Serializable]
    public class Bag : ISerializable, IDictionary<string, object>
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

        public static Bag Copy(Bag bag)
        {
            return CreateFromDictionary(bag);
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
        public static Bag Create<TProperties>(TProperties propertyContainer)
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
        /// <typeparam name="TProperties">The type of property container.</typeparam>
        /// <param name="propertyContainer">
        /// Object which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an object passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static Bag CreateFromProperties<TProperties>(TProperties propertyContainer)
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

        /// <summary>
        /// Flag identifying whether pipeline must be aborted/stopped,
        /// it can be used as a cancelation identifier for the execution flow.
        /// </summary>
        public bool IsAborted { get; set; }

        /// <summary>
        /// Collection of messages that keeps all the text passed to the
        /// context during the pipeline execution.
        /// </summary>
        protected Lazy<ICollection<PipelineMessage>> Messages { get; } =
            new Lazy<ICollection<PipelineMessage>>(() => new PipelineMessageCollection());

        /// <summary>
        /// Collection of the properties that contains all the collected
        /// or obtained values during pipeline execution or before
        /// execution is started <see cref="PipelineContext(object)"/>.
        /// </summary>
        protected Lazy<Dictionary<string, PipelineProperty>> Properties { get; } = new Lazy<Dictionary<string, PipelineProperty>>(() =>
            new Dictionary<string, PipelineProperty>(StringComparer.InvariantCultureIgnoreCase));

        public ICollection<string> Keys => throw new NotImplementedException();

        public ICollection<object> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public object this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
        /// Adds the property to the collection <see cref="Properties"/>
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
            var property = new PipelineProperty(name, value);
            var dictionary = Properties.Value;
            if (!dictionary.ContainsKey(name))
            {
                dictionary.Add(name, property);
            }
            else
            {
                if (!skipIfExists)
                {
                    dictionary[name] = property;
                }
            }
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
            var propertyHolder = GetPropertyObjectOrNull(name);
            if (propertyHolder.HasValue)
            {
                if (propertyHolder.Value.Value is TValue value)
                {
                    return value;
                }
            }

            var summaryMessage = Summary();
            throw new ArgumentOutOfRangeException(nameof(name), $"The property \"{name}\" was not added to the Pipeline context. Try to go through messages:\r\n{summaryMessage}");
        }

        public virtual string StringOrEmpty(string name)
        {
            var propertyHolder = GetPropertyObjectOrNull(name);
            if (propertyHolder.HasValue)
            {
                if (propertyHolder.Value.Value is string value)
                {
                    return value;
                }
            }

            return string.Empty;
        }

        public virtual string StringOrThrow(string name)
        {
            return GetOrThrow<string>(name);
        }

        public virtual List<TElement> ListOrThrow<TElement>(string name)
        {
            return GetOrThrow<IEnumerable<TElement>>(name).ToList();
        }

        public virtual List<TElement> ListOrEmpty<TElement>(string name)
        {
            var propertyHolder = GetPropertyObjectOrNull(name);
            if (propertyHolder.HasValue)
            {
                if (propertyHolder.Value.Value is IEnumerable<TElement> value)
                {
                    return value.ToList();
                }
            }

            return new List<TElement>();
        }

        public virtual TValue Get<TValue>(string name, Func<TValue> or)
        {
            var propertyHolder = GetPropertyObjectOrNull(name);
            if (propertyHolder.HasValue)
            {
                if (propertyHolder.Value.Value is TValue value)
                {
                    return value;
                }
            }

            return or();
        }

        /// <summary>
        /// Retrieves a <see cref="PipelineProperty"/> that
        /// is kept in this context or if property does not
        /// exist retrieves <c>null</c>.
        /// </summary>
        /// <param name="name">
        /// The name of the property to be retrieved.
        /// </param>
        /// <returns>
        /// Pipeline property object of the requested
        /// <paramref name="name"/> or null if property does not exist.
        /// </returns>
        protected virtual PipelineProperty? GetPropertyObjectOrNull(string name)
        {
            if (Properties.IsValueCreated)
            {
                var dictionary = Properties.Value;
                if (dictionary.ContainsKey(name))
                {
                    return dictionary[name];
                }
            }

            return null;
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
        public virtual bool HasProperty<TProperty>(string name)
        {
            return ContainsProperty<TProperty>(name);
        }

        public virtual bool HasProperty<TProperty>(string name, out TProperty property)
        {
            return ContainsProperty(name, out property);
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
        public virtual bool ContainsProperty<TProperty>(string name)
        {
            var property = GetPropertyObjectOrNull(name);
            return property?.Value is TProperty;
        }

        public virtual bool ContainsProperty<TProperty>(string name, out TProperty value)
        {
            value = default(TProperty);
            var property = GetPropertyObjectOrNull(name);
            var success = property?.Value is TProperty;
            if (success)
            {
                value = (TProperty)property.Value.Value;
            }

            return success;
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
        public virtual bool DoesNotContainProperty<TProperty>(string name)
        {
            return !ContainsProperty<TProperty>(name);
        }

        /// <summary>
        /// Deletes a property with the
        /// specified <paramref name="name"/> from the context.
        /// </summary>
        /// <param name="name">
        /// The name of the property to be deleted.
        /// </param>
        public virtual void DeleteProperty(string name)
        {
            if (Properties.IsValueCreated)
            {
                var dictionary = Properties.Value;
                dictionary.Remove(name);
            }
        }

        /// <summary>
        /// Retrieves all the property objects
        /// contained in the context.
        /// </summary>
        /// <returns>
        /// An array of property objects that
        /// are contained in context.
        /// </returns>
        public virtual PipelineProperty[] GetAllPropertyObjects()
        {
            if (this.Properties.IsValueCreated)
            {
                return this.Properties.Value.Values.ToArray();
            }

            return new PipelineProperty[0];
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
        public virtual PipelineMessage[] GetMessages(MessageFilter filter)
        {
            if (Messages.IsValueCreated && Messages.Value.Count > 0)
            {
                if (filter == MessageFilter.All)
                {
                    return Messages.Value.ToArray();
                }
                return Messages.Value.Where(message => ((int)message.MessageType & (int)filter) > 0).ToArray();
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
        public virtual PipelineMessage[] GetAllMessages()
        {
            return this.GetMessages(MessageFilter.All);
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
        public virtual string[] GetAllMessageTexts()
        {
            return this.GetAllMessages()
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
        public virtual string[] GetMessageTexts(MessageFilter filter)
        {
            return this.GetMessages(filter)
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
        public virtual string[] GetMessageTexts(MessageFilter filter, Func<string, MessageType, string> format)
        {
            if (format.HasNoValue())
            {
                return this.GetMessageTexts(filter);
            }

            return this.GetMessages(filter)
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
        public virtual string[] GetMessageTexts(MessageFilter filter, Func<PipelineMessage, string> format)
        {
            if (format.HasNoValue())
            {
                return this.GetMessageTexts(filter);
            }

            return this.GetMessages(filter)
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
        public virtual string[] GetAllMessageTexts(Func<string, MessageType, string> format)
        {
            if (format.HasNoValue())
            {
                return this.GetAllMessageTexts();
            }

            return this.GetAllMessages()
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
        public virtual string[] GetAllMessageTexts(Func<PipelineMessage, string> format)
        {
            if (format.HasNoValue())
            {
                return this.GetAllMessageTexts();
            }

            return this.GetAllMessages()
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
            string[] texts = this.GetMessageTexts(filter, format);

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
        public virtual PipelineMessage[] GetInformationsAndWarnings()
        {
            return this.GetMessages(MessageFilter.Informations | MessageFilter.Warnings);
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
        public virtual PipelineMessage[] GetWarningsAndErrors()
        {
            return this.GetMessages(MessageFilter.Warnings | MessageFilter.Errors);
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
        public virtual PipelineMessage[] GetInformationMessages()
        {
            return this.GetMessages(MessageFilter.Informations);
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
        public virtual PipelineMessage[] GetWarningMessages()
        {
            return this.GetMessages(MessageFilter.Warnings);
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
        public virtual PipelineMessage[] GetErrorMessages()
        {
            return this.GetMessages(MessageFilter.Errors);
        }

        /// <summary>
        /// Aborts pipeline by setting a flag <see cref="IsAborted"/> to true.
        /// It allows to tell all the other users of this context that pipeline
        /// cannot be run further.
        /// </summary>
        public virtual void AbortPipeline()
        {
            IsAborted = true;
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
            Messages.Value.Add(message);
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
        public Bag()
        {
        }

        public Bag(object propertyContainer)
        {
            if (propertyContainer.HasValue())
            {
                foreach (var prop in propertyContainer.GetType().GetProperties())
                {
                    var contextProperty = new PipelineProperty(prop.Name, prop.GetValue(propertyContainer, null));
                    this.Properties.Value.Add(contextProperty.Name, contextProperty);
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

        public Bag MakeCopy()
        {
            return CreateFromDictionary(this);
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
            info.AddValue($"{nameof(Bag)}.{nameof(IsAborted)}", IsAborted);
            info.AddValue($"{nameof(Bag)}.{nameof(Messages)}", Messages, typeof(ICollection<PipelineMessage>));
        }

        public static string ResultProperty = "result";

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

        public string StringResultOrThrow()
        {
            return this.StringOrThrow(ResultProperty);
        }

        public string StringResultOrEmpty()
        {
            return this.StringOrEmpty(ResultProperty);
        }

        public List<TElement> ListResultOrThrow<TElement>()
        {
            return this.ListOrThrow<TElement>(ResultProperty);
        }

        public List<TElement> ListResultOrEmpty<TElement>()
        {
            return this.ListOrEmpty<TElement>(ResultProperty);
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
            return this.ContainsProperty<TResult>(ResultProperty);
        }

        public virtual bool ContainsResult<TResult>(out TResult result)
        {
            return this.ContainsProperty(ResultProperty, out result);
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
            this.Set(key, value);
        }

        public bool ContainsKey(string key)
        {
            if (Properties.IsValueCreated)
            {
                return Properties.Value.ContainsKey(key);
            }

            return false;
        }

        public bool Remove(string key)
        {
            this.DeleteProperty(key);
            return true;
        }

        public bool TryGetValue(string key, out object value)
        {
            return ContainsProperty(key, out value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.Set(item.Key, item.Value);
        }

        public void Clear()
        {
            if (Properties.IsValueCreated)
            {
                Properties.Value.Clear();
            }
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return TryGetValue(item.Key, out object val) && val == item.Value;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            if (array == null) return;

            if (Properties.IsValueCreated)
            {
                var props = Properties.Value.Skip(arrayIndex).Select(x => new KeyValuePair<string, object>(x.Key, x.Value.Value));
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
            if (Properties.IsValueCreated)
            {
                foreach (var item in Properties.Value)
                {
                    yield return new KeyValuePair<string, object>(item.Key, item.Value.Value);
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