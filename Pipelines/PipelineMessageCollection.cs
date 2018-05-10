using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Pipelines.ExtensionMethods;

namespace Pipelines
{
    [System.Runtime.InteropServices.ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    public class PipelineMessageCollection : ICollection<PipelineMessage>, IReadOnlyCollection<PipelineMessage>
    {
        protected ICollection<PipelineMessage> Collection { get; } = new LinkedList<PipelineMessage>();

        public IEnumerator<PipelineMessage> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(PipelineMessage item)
        {
            Collection.Add(item);
        }

        public void Clear()
        {
            Collection.Clear();
        }

        public bool Contains(PipelineMessage item)
        {
            return Collection.Contains(item);
        }

        public void CopyTo(PipelineMessage[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(PipelineMessage item)
        {
            return Collection.Remove(item);
        }

        int ICollection<PipelineMessage>.Count => Collection.Count;

        public bool IsReadOnly => Collection.IsReadOnly;

        int IReadOnlyCollection<PipelineMessage>.Count => Collection.Count;
        
        public virtual void AddMessageObject(PipelineMessage message)
        {
            this.Add(message);
        }

        public virtual void AddMessageObjects(IEnumerable<PipelineMessage> messages)
        {
            foreach (var message in messages.EnsureAtLeastEmpty())
            {
                this.AddMessageObject(message);
            }
        }

        public virtual void AddMessage(string message, MessageType messageType = MessageType.Information)
        {
            AddMessageObject(new PipelineMessage(message, messageType));
        }

        public virtual void AddInformation(string message)
        {
            AddMessage(message, MessageType.Information);
        }

        public virtual void AddWarning(string message)
        {
            AddMessage(message, MessageType.Warning);
        }

        public virtual void AddError(string message)
        {
            AddMessage(message, MessageType.Error);
        }

        public virtual PipelineMessage[] GetMessages(MessageFilter filter)
        {
            if (filter == MessageFilter.All)
            {
                return this.Collection.ToArray();
            }

            return this.Collection.Where(message => ((int) message.MessageType & (int) filter) > 0).ToArray();
        }

        public virtual PipelineMessage[] GetAllMessages()
        {
            return this.GetMessages(MessageFilter.All);
        }

        public virtual PipelineMessage[] GetInformationsAndWarnings()
        {
            return this.GetMessages(MessageFilter.Informations | MessageFilter.Warnings);
        }

        public virtual PipelineMessage[] GetWarningsAndErrors()
        {
            return this.GetMessages(MessageFilter.Warnings | MessageFilter.Errors);
        }

        public virtual PipelineMessage[] GetInformationMessages()
        {
            return this.GetMessages(MessageFilter.Informations);
        }

        public virtual PipelineMessage[] GetWarningMessages()
        {
            return this.GetMessages(MessageFilter.Warnings);
        }

        public virtual PipelineMessage[] GetErrorMessages()
        {
            return this.GetMessages(MessageFilter.Errors);
        }
    }
}