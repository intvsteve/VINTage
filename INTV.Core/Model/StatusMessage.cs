// <copyright file="StatusMessage.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

using System.Collections.Generic;
using System.Linq;

namespace INTV.Core.Model
{
    /// <summary>
    /// Describes a status message.
    /// </summary>
    public class StatusMessage : INTV.Core.ComponentModel.ModelBase
    {
        private static readonly StatusMessage _dummyMessage = new StatusMessage(MessageSeverity.None, string.Empty);
        private static readonly List<StatusMessage> _messages = new List<StatusMessage>();

        private MessageSeverity _severity;
        private string _message;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the StatusMessage class.
        /// </summary>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="message">The message text.</param>
        public StatusMessage(MessageSeverity severity, string message)
        {
            _severity = severity;
            _message = message;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the currently active messages.
        /// </summary>
        public static IEnumerable<StatusMessage> AllMessages
        {
            get { return _messages; }
        }

        /// <summary>
        /// Gets the severity of a message.
        /// </summary>
        public MessageSeverity Severity
        {
            get { return _severity; }
            private set { _severity = value; }
        }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message
        {
            get { return _message; }
            private set { _message = value; }
        }

        #endregion // Properties

        /// <summary>
        /// Add a message to the list of active messages.
        /// </summary>
        /// <param name="severity">The severity of the message to add.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="messageReporter">The entity reporting the message.</param>
        public static void AddMessage(MessageSeverity severity, string message, object messageReporter)
        {
            AddMessage(severity, message, messageReporter, true);
        }

        /// <summary>
        /// Add multiple messages to the list of active messages.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        /// <param name="messageReporter">The entity reporting the messages.</param>
        public static void AddMessages(IEnumerable<StatusMessage> messages, object messageReporter)
        {
            AddMessages(messages, messageReporter, true);
        }

        /// <summary>
        /// Clear the list of active messages.
        /// </summary>
        public static void ClearMessages()
        {
            ClearMessages(true);
        }

        /// <summary>
        /// Get the highest severity of the active messages.
        /// </summary>
        /// <returns>The highest severity in the active message list.</returns>
        public static MessageSeverity GetOverallSeverity()
        {
            MessageSeverity severity = MessageSeverity.None;
            if (AllMessages.Where(m => m.Severity == MessageSeverity.Error).Any())
            {
                severity = MessageSeverity.Error;
            }
            else if (AllMessages.Where(m => m.Severity == MessageSeverity.Warning).Any())
            {
                severity = MessageSeverity.Warning;
            }
            else if (AllMessages.Where(m => m.Severity == MessageSeverity.Status).Any())
            {
                severity = MessageSeverity.Status;
            }
            return severity;
        }

        /// <summary>
        /// Add multiple messages to the list of active messages.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        /// <param name="messageReporter">The entity reporting the messages.</param>
        /// <param name="raisePropertyChanged">If <c>true</c>, fire the PropertyChanged event.</param>
        public static void AddMessages(IEnumerable<StatusMessage> messages, object messageReporter, bool raisePropertyChanged)
        {
            _messages.AddRange(messages);
            if (raisePropertyChanged)
            {
                _dummyMessage.RaisePropertyChanged(messageReporter, "AllMessages");
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: {1}", Severity, Message);
        }

        /// <summary>
        /// Add a message to the list of active messages.
        /// </summary>
        /// <param name="severity">The severity of the message to add.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="messageReporter">The entity reporting the message.</param>
        /// <param name="raisePropertyChanged">If <c>true</c>, fire the PropertyChanged event.</param>
        internal static void AddMessage(MessageSeverity severity, string message, object messageReporter, bool raisePropertyChanged)
        {
            _messages.Add(new StatusMessage(severity, message));
            if (raisePropertyChanged)
            {
                _dummyMessage.RaisePropertyChanged(messageReporter, "AllMessages");
            }
        }

        /// <summary>
        /// Clear the list of active messages.
        /// </summary>
        /// <param name="raisePropertyChanged">If <c>true</c>, fire the PropertyChanged event.</param>
        internal static void ClearMessages(bool raisePropertyChanged)
        {
            _messages.Clear();
            if (raisePropertyChanged)
            {
                _dummyMessage.RaisePropertyChanged(null, "AllMessages");
            }
        }
    }
}
