using Pipelines.Implementations.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Pipelines.ExtensionMethods;

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
    public class Bag : ISerializable
    {
        public static ChainingContext<Bag> Build() =>
            Build<Bag>();

        public static ChainingContext<TContext> Build<TContext>() where TContext : Bag, new() =>
            Build(new TContext());

        public static ChainingContext<TContext> Build<TContext>(TContext context) where TContext : Bag =>
            new ChainingContext<TContext>(context);

        public static ChainingContext<Backpack<TValue>> BuildQuery<TValue>() where TValue : class =>
            Build<Backpack<TValue>>();

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
                    context.SetOrAddProperty(item.Key, item.Value);
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
                    context.SetOrAddProperty(item.Key, item.Value);
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
                case PropertyModificator.SkipIfExists:
                    this.AddOrSkipPropertyIfExists(name, value);
                    break;
                case PropertyModificator.UpdateValue:
                    this.UpdateOrAddProperty(name, value);
                    break;
                default:
                    this.AddOrSkipPropertyIfExists(name, value);
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
        public virtual void SetOrAddProperty<TValue>(string name, TValue value)
        {
            this.UpdateOrAddProperty(name, value);
        }

        /// <summary>
        /// Adds the property to the collection <see cref="Properties"/>
        /// or updates the value if key of parameter <paramref name="name"/>
        /// has been added previously.
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
        public virtual void UpdateOrAddProperty<TValue>(string name, TValue value)
        {
            var property = new PipelineProperty(name, value);
            var dictionary = Properties.Value;
            if (dictionary.ContainsKey(name))
            {
                dictionary[name] = property;
            }
            else
            {
                dictionary.Add(name, property);
            }
        }

        /// <summary>
        /// Adds the property to the collection <see cref="Properties"/>
        /// or if the key of the parameter <paramref name="name"/>
        /// has been added previously skips adding.
        /// </summary>
        /// <remarks>
        /// Parameter name will be used in case-insensetive way.
        /// It means that if you previously added property name "MESSAGE"
        /// it will be skipped if you pass to this method property name "message".
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
        public virtual void AddOrSkipPropertyIfExists<TValue>(string name, TValue value)
        {
            var dictionary = Properties.Value;
            if (!dictionary.ContainsKey(name))
            {
                var property = new PipelineProperty(name, value);
                dictionary.Add(name, property);
            }
        }

        /// <summary>
        /// Adds the property to the collection <see cref="Properties"/>
        /// or if the key of the parameter <paramref name="property"/>
        /// has been added previously skips adding.
        /// </summary>
        /// <remarks>
        /// Parameter name will be used in case-insensetive way.
        /// It means that if you previously added property name "MESSAGE"
        /// it will be skipped if you pass to this method property name "message".
        /// </remarks>
        /// <param name="property">
        /// The property object to be added to the collection.
        /// </param>
        public virtual void AddOrSkipPropertyIfExists(PipelineProperty property)
        {
            var dictionary = Properties.Value;
            if (!dictionary.ContainsKey(property.Name))
            {
                dictionary.Add(property.Name, property);
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
        public virtual PipelineProperty? GetPropertyObjectOrNull(string name)
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
        /// The function returns boolean value that defines
        /// whether property was added to the context or not (analog to <see cref="ContainsProperty"/>).
        /// </summary>
        /// <param name="name">
        /// The name of the property to be checked for presence.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> in case property exists,
        /// otherwise returns <c>false</c>.
        /// </returns>
        public virtual bool HasProperty(string name)
        {
            return ContainsProperty(name);
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

        /// <summary>
        /// The function returns boolean value that defines
        /// whether property was added to the context or not.
        /// </summary>
        /// <param name="name">
        /// The name of the property to be checked for presence.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> in case property exists,
        /// otherwise returns <c>false</c>.
        /// </returns>
        public virtual bool ContainsProperty(string name)
        {
            return GetPropertyObjectOrNull(name) != null;
        }

        /// <summary>
        /// The function returns boolean value that defines
        /// whether property is missing in context or not.
        /// </summary>
        /// <param name="name">
        /// The name of the property to be checked for absence.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> in case property missing,
        /// otherwise returns <c>false</c>.
        /// </returns>
        public virtual bool DoesNotContainProperty(string name)
        {
            return !ContainsProperty(name);
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
        /// Retrieves the value that is defined under the property
        /// of parameter <paramref name="name"/> or if was not added
        /// or the type of the contained property is different than
        /// <see cref="TValue"/>, the <c>null</c> will be retrieved.
        /// </summary>
        /// <typeparam name="TValue">
        /// The type of the retrieved value.
        /// </typeparam>
        /// <param name="name">
        /// Key to identify the property (case-insensetive).
        /// </param>
        /// <returns>
        /// The value kept under the <paramref name="name"/> of the property
        /// or <c>null</c> if property was not added or the type
        /// of the value is different than <see cref="TValue"/>.
        /// </returns>
        public virtual TValue Get<TValue>(string name) where TValue : class
        {
            return this.Get<TValue>(name, null);
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
        public virtual string GetSummaryMessage(
            string separator = null, MessageFilter filter = MessageFilter.All,
            Func<PipelineMessage, string> format = null)
        {
            string[] texts = this.GetMessageTexts(filter, format);

            separator = separator.Ensure(Environment.NewLine);

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
        public virtual void AddMessageObject(PipelineMessage message)
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
        public virtual void AddMessageObjects(IEnumerable<PipelineMessage> messages)
        {
            foreach (var message in messages.EnsureAtLeastEmpty())
            {
                this.AddMessageObject(message);
            }
        }

        /// <summary>
        /// Adds a message to the pipeline execution context, which allows
        /// other users of the context see what happens in current operation.
        /// This method differs from other adding message methods by having
        /// <paramref name="message"/> text for the message and an optional
        /// <paramref name="messageType"/> allowing to specify kind of
        /// the message and by default is set to information.
        /// </summary>
        /// <param name="message">The text of the context message.</param>
        /// <param name="messageType">
        /// Message type indicating status of the operation. Default is information.
        /// </param>
        public virtual void AddMessage(string message, MessageType messageType = MessageType.Information)
        {
            AddMessageObject(new PipelineMessage(message, messageType));
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

        /// <summary>
        /// Executes two actions: aborts pipeline and adds a default message
        /// by using <see cref="AddMessage"/> method.
        /// </summary>
        /// <param name="message">
        /// Text that describes a cause of the abortion.
        /// </param>
        public virtual void AbortPipelineWithMessage(string message)
        {
            AbortPipeline();
            AddMessage(message);
        }

        /// <summary>
        /// Executes two actions: aborts pipeline and adds a message
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="message">
        /// Text that describes a cause of the abortion.
        /// </param>
        /// <param name="type">
        /// A type of the message, it will help you to find message
        /// by using <see cref="GetMessages"/> method.
        /// </param>
        public virtual void AbortPipelineWithTypedMessage(string message, MessageType type)
        {
            AbortPipeline();
            AddMessage(message, type);
        }

        /// <summary>
        /// Executes two actions: aborts pipeline and adds an error message
        /// which signals about wrong pipeline abortion.
        /// </summary>
        /// <param name="message">
        /// Error message text that describes a cause of the abortion.
        /// </param>
        public virtual void AbortPipelineWithErrorMessage(string message)
        {
            AbortPipelineWithTypedMessage(message, MessageType.Error);
        }

        /// <summary>
        /// Executes two actions: aborts pipeline and adds a warning message
        /// which signals about wrong pipeline execution.
        /// </summary>
        /// <param name="message">
        /// Warning message text that describes a cause of the abortion.
        /// </param>
        public virtual void AbortPipelineWithWarningMessage(string message)
        {
            AbortPipelineWithTypedMessage(message, MessageType.Warning);
        }

        /// <summary>
        /// Executes two actions: aborts pipeline and adds an information message
        /// which signals about early pipeline end.
        /// </summary>
        /// <param name="message">
        /// Information message text that describes a cause of the abortion.
        /// </param>
        public virtual void AbortPipelineWithInformationMessage(string message)
        {
            AbortPipelineWithTypedMessage(message, MessageType.Information);
        }

        /// <summary>
        /// Adds an information message. Useful method to track what happens
        /// during pipeline execution. It should be used often to provide
        /// clear and understandable flow.
        /// </summary>
        /// <param name="message">
        /// An information message, used to describe execution status.
        /// </param>
        public virtual void AddInformation(string message)
        {
            AddMessage(message, MessageType.Information);
        }

        /// <summary>
        /// Adds a warning message. Useful method to track some unoptimized
        /// pieces or things which could have cause an error.
        /// </summary>
        /// <param name="message">
        /// Warning message, used to describe warning status.
        /// </param>
        public virtual void AddWarning(string message)
        {
            AddMessage(message, MessageType.Warning);
        }

        /// <summary>
        /// Adds an error message. Use this method when during the pipeline
        /// execution, something goes wrong, and pipeline cannot proceed,
        /// so it must be stopped.
        /// </summary>
        /// <param name="message">
        /// Error message, used to describe an error occured during execution.
        /// </param>
        public virtual void AddError(string message)
        {
            AddMessage(message, MessageType.Error);
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
    }
}