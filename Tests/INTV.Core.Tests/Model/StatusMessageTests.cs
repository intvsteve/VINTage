// <copyright file="StatusMessageTests.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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
using System.Linq;
using INTV.Core.Model;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class StatusMessageTests
    {
        [Fact]
        public void StatusMessage_AddOneMessage_MessageAddedAndHandlerCalled()
        {
            var numMessagesReportedArrived = 0;
            StatusMessage receivedMessage = null;
            var handler = new EventHandler<StatusMessageEventArgs>((s, e) =>
            {
                numMessagesReportedArrived = e.Messages.Count();
                receivedMessage = e.Messages.FirstOrDefault();
            });

            var severity = MessageSeverity.Status;
            var message = "Howdy!";
            StatusMessage.MessagesArrived += handler;
            StatusMessage.AddMessage(this, MessageSeverity.Status, message);

            Assert.Equal(1, numMessagesReportedArrived);
            Assert.Equal(severity, receivedMessage.Severity);
            Assert.Equal(message, receivedMessage.Message);

            StatusMessage.MessagesArrived -= handler;
            StatusMessage.ClearMessages();
        }

        [Fact]
        public void StatusMessage_AddMultipleMessages_AllMessagesContainsThem()
        {
            var messages = new[]
            {
                new StatusMessage(MessageSeverity.None, "psst... What are you doing here?"),
                new StatusMessage(MessageSeverity.Warning, "Step away before somebody gets hurt!"),
                new StatusMessage(MessageSeverity.Status, "Hello, Commander. Computer reporting!"),
                new StatusMessage(MessageSeverity.Error, "The battle is over."),
                new StatusMessage(MessageSeverity.None, "Who are you?"),
            };

            StatusMessage.AddMessages(this, messages);

            Assert.Equal(messages.Length, StatusMessage.AllMessages.Count());
            Assert.Empty(messages.Except(StatusMessage.AllMessages));

            StatusMessage.ClearMessages();
        }

        [Fact]
        public void StatusMessage_AddMultipleMessages_GetHighestSeverity()
        {
            var messages = new[]
            {
                new StatusMessage(MessageSeverity.None, "How many say Sharp Shot is teh roXXo0rz?"),
                new StatusMessage(MessageSeverity.Warning, "You'll never do it in time!"),
                new StatusMessage(MessageSeverity.Error, "END OF LINE"),
                new StatusMessage(MessageSeverity.Status, "The code! The code! Figure out the code!"),
                new StatusMessage(MessageSeverity.None, "Flak! Flack! Watch for flak!"),
            };

            StatusMessage.AddMessages(this, messages);

            Assert.Equal(MessageSeverity.Error, StatusMessage.GetOverallSeverity());

            StatusMessage.ClearMessages();
        }

        [Fact]
        public void StatusMessage_CompareToNull_GetsExpectedResult()
        {
            var message = new StatusMessage(MessageSeverity.None, "nertz");

            Assert.Equal(1, message.CompareTo((object)null));
        }

        [Fact]
        public void StatusMessage_CompareToNonMessage_ThrowsArgumentException()
        {
            var message = new StatusMessage(MessageSeverity.Status, "By your command!");

            Assert.Throws<ArgumentException>(() => message.CompareTo("Hiya, Bugs!"));
        }

        [Fact]
        public void StatusMessage_CompareToMessageAsObject_GetsExpectedResult()
        {
            var message0 = new StatusMessage(MessageSeverity.None, "Ye Read, Ye Move");
            object message1 = new StatusMessage(MessageSeverity.None, "Ye Read, Ye Move");

            Assert.Equal(0, message0.CompareTo(message1));
        }

        [Fact]
        public void StatusMessage_CompareToAnotherMessageWithDifferentSeverity_GetsExpectedResult()
        {
            var message0 = new StatusMessage(MessageSeverity.Warning, "Eating live bumblebees may sting a bit.");
            var message1 = new StatusMessage(MessageSeverity.Error, "They'll be looking for us!");

            Assert.Equal((int)message0.Severity - (int)message1.Severity, message0.CompareTo(message1));
        }

        [Fact]
        public void StatusMessage_CompareToAnotherMessageWithSameSeverityAndDifferentMessage_GetsExpectedResult()
        {
            var message0 = new StatusMessage(MessageSeverity.Status, "Cut this one out first.");
            var message1 = new StatusMessage(MessageSeverity.Status, "This fourth");

            Assert.Equal(StringComparer.CurrentCultureIgnoreCase.Compare(message0.Message, message1.Message), message0.CompareTo(message1));
        }

        [Fact]
        public void StatusMessage_CompareToAnotherMessageWithSameSeverityAndDifferentMessageWithNull_GetsExpectedResult()
        {
            var message0 = new StatusMessage(MessageSeverity.Status, "I got nuthin'.");
            var message1 = new StatusMessage(MessageSeverity.Status, null);

            Assert.Equal(StringComparer.CurrentCultureIgnoreCase.Compare(message0.Message, message1.Message), message0.CompareTo(message1));
        }

        [Fact]
        public void StatusMessage_CompareToAnotherMessageWithSameSeverityAndMessage_GetsExpectedResult()
        {
            var message0 = new StatusMessage(MessageSeverity.Warning, "If once you start down the dark path, forever will it dominate your destiny.");
            var message1 = new StatusMessage(MessageSeverity.Warning, "If once you start down the dark path, forever will it dominate your destiny.");

            Assert.Equal(0, message0.CompareTo(message1));
        }

        [Fact]
        public void StatusMessage_CompareToAnotherMessageWithSameSeverityAndMessageInDifferentCase_GetsExpectedResult()
        {
            var message0 = new StatusMessage(MessageSeverity.Error, "I'm your density!");
            var message1 = new StatusMessage(MessageSeverity.Error, "I'M YOUR DENSITY!");

            Assert.Equal(0, message0.CompareTo(message1));
        }

        [Fact]
        public void StatusMessage_ToString_IsCorrect()
        {
            var message = new StatusMessage(MessageSeverity.Error, "Whattaheck?");

            Assert.Equal("Error: Whattaheck?", message.ToString());
        }
    }
}
