// <copyright file="StatusMessage.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace INTV.Core.Model
{
    /// <summary>
    /// Describes a status message.
    /// </summary>
    public class StatusMessage : IComparable, IComparable<StatusMessage>, IEquatable<StatusMessage>
    {
        private static readonly Lazy<ConcurrentBag<StatusMessage>> TheMessages = new Lazy<ConcurrentBag<StatusMessage>>(() => new ConcurrentBag<StatusMessage>());

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the StatusMessage class.
        /// </summary>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="message">The message text.</param>
        public StatusMessage(MessageSeverity severity, string message)
        {
            Severity = severity;
            Message = message;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the currently active messages.
        /// </summary>
        public static IEnumerable<StatusMessage> AllMessages
        {
            get { return Messages; }
        }

        private static ConcurrentBag<StatusMessage> Messages
        {
            get { return TheMessages.Value; }
        }

        /// <summary>
        /// Gets the severity of a message.
        /// </summary>
        public MessageSeverity Severity { get; private set; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message { get; private set; }

        #endregion // Properties

        /// <summary>
        /// This event is raised when messages are reported.
        /// </summary>
        public static event EventHandler<StatusMessageEventArgs> MessagesArrived;

        /// <summary>
        /// Add a message to the list of active messages.
        /// </summary>
        /// <param name="messageReporter">The entity reporting the message.</param>
        /// <param name="severity">The severity of the message to add.</param>
        /// <param name="message">The text of the message.</param>
        public static void AddMessage(object messageReporter, MessageSeverity severity, string message)
        {
            AddMessages(messageReporter, new[] { new StatusMessage(severity, message) });
        }

        /// <summary>
        /// Add multiple messages to the list of active messages.
        /// </summary>
        /// <param name="messageReporter">The entity reporting the messages.</param>
        /// <param name="messages">The messages to add.</param>
        public static void AddMessages(object messageReporter, IEnumerable<StatusMessage> messages)
        {
            foreach (var message in messages)
            {
                Messages.Add(message);
            }
            OnMessagesArrived(messageReporter, messages);
        }

        /// <summary>
        /// Clear the list of active messages.
        /// </summary>
        public static void ClearMessages()
        {
            while (!Messages.IsEmpty)
            {
                StatusMessage dontCare;
                Messages.TryTake(out dontCare);
            }
        }

        /// <summary>
        /// Get the highest severity of the active messages.
        /// </summary>
        /// <returns>The highest severity in the active message list.</returns>
        public static MessageSeverity GetOverallSeverity()
        {
            MessageSeverity severity = MessageSeverity.None;
            foreach (var message in AllMessages)
            {
                if (message.Severity > severity)
                {
                    severity = message.Severity;
                }
                if (severity == MessageSeverity.MaximumSeverity)
                {
                    break;
                }
            }
            return severity;
        }

        #region IComparable

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            var result = -1;
            if (obj is StatusMessage)
            {
                result = CompareTo((StatusMessage)obj);
            }

            return result;
        }

        #endregion // IComparable

        #region IComparable<StatusMessage>

        /// <inheritdoc />
        public int CompareTo(StatusMessage other)
        {
            var result = -1;
            if (other != null)
            {
                result = (int)Severity - (int)other.Severity;
                if (result == 0)
                {
                    result = StringComparer.CurrentCultureIgnoreCase.Compare(Message, other.Message);
                }
            }

            return result;
        }

        #endregion // IComparable<StatusMessage>

        #region IEquatable<StatusMessage>

        /// <inheritdoc />
        public bool Equals(StatusMessage other)
        {
            return CompareTo(other) == 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: {1}", Severity, Message);
        }

        #endregion // IEquatable<StatusMessage>

        private static void OnMessagesArrived(object reporter, IEnumerable<StatusMessage> messages)
        {
            var messagesArrived = MessagesArrived;
            if (messagesArrived != null)
            {
                messagesArrived(reporter, new StatusMessageEventArgs(messages));
            }
        }
    }
}
