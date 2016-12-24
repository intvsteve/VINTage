// <copyright file="ReportDialog.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
using System.Collections.Generic;

namespace INTV.Shared.View
{
    /// <summary>
    /// Common implementation for the general-purpose report dialog. This dialog may be used to simply report
    /// detailed results from an operation, or to report run-time errors to the user. It offers the ability to
    /// copy text to the clipboard and send an email to support@intvfunhouse.com.
    /// </summary>
    public partial class ReportDialog
    {
        #region Properties

        /// <summary>
        /// Gets or sets the text wrapping mode for the report text.
        /// </summary>
        public OSTextWrapping TextWrapping
        {
            get { return ViewModel.TextWrapping; }
            set { ViewModel.TextWrapping = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="INTV.Shared.View.ReportDialog"/>
        /// should show the Copy to Clipboard button.
        /// </summary>
        public bool ShowCopyToClipboardButton
        {
            get { return ViewModel.ShowCopyToClipboardButton; }
            set { ViewModel.ShowCopyToClipboardButton = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="INTV.Shared.View.ReportDialog"/>
        /// should show the Send Email button.
        /// </summary>
        public bool ShowSendEmailButton
        {
            get { return ViewModel.ShowSendEmailButton; }
            set { ViewModel.ShowSendEmailButton = value; }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get { return ViewModel.Message; }
            set { ViewModel.Message = value; }
        }

        /// <summary>
        /// Gets or sets the report text.
        /// </summary>
        public string ReportText
        {
            get { return ViewModel.ReportText; }
            set { ViewModel.ReportText = value; }
        }

        /// <summary>
        /// Gets or sets the exception reported by the dialog.
        /// </summary>
        public Exception Exception
        {
            get { return ViewModel.Exception; }
            set { ViewModel.Exception = value; }
        }

        /// <summary>
        /// Gets the files to attach to an email message that the dialog may send.
        /// </summary>
        public IList<string> Attachments
        {
            get { return ViewModel.Attachments; }
        }

        /// <summary>
        /// Gets or sets the email CC recipients.
        /// </summary>
        public IEnumerable<string> EmailCCRecipients
        {
            get { return ViewModel.EmailCCRecipients; }
            set { ViewModel.EmailCCRecipients = value; }
        }

        #endregion // Properties
    }
}
