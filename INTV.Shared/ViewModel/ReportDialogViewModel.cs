// <copyright file="ReportDialogViewModel.cs" company="INTV Funhouse">
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

////#define SMTP_SUPPORT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the crash report dialog.
    /// </summary>
    public partial class ReportDialogViewModel : ViewModelBase
    {
        #region Property Names

        public const string TitlePropertyName = "Title";
        public const string MessagePropertyName = "Message";
        public const string CloseDialogButtonTextPropertyName = "CloseDialogButtonText";
        public const string CloseDialogButtonEnabledPropertyName = "CloseDialogButtonEnabled";
        public const string ShowCopyToClipboardButtonPropertyName = "ShowCopyToClipboardButton";
        public const string TextWrappingPropertyName = "TextWrapping";
        public const string ReportTextPropertyName = "ReportText";
        public const string ExceptionPropertyName = "Exception";
        public const string DoNotShowAgainTextPropertyName = "DoNotShowAgainText";
        public const string ShowDoNotShowAgainPropertyName = "ShowDoNotShowAgain";
        public const string SendEmailButtonLabelTextPropertyName = "SendEmailButtonLabelText";
        public const string ShowSendEmailButtonPropertyName = "ShowSendEmailButton";
        public const string SendEmailEnabledPropertyName = "SendEmailEnabled";
        public const string EmailSenderPropertyName = "EmailSender";
        public const string ShowEmailSenderPropertyName = "ShowEmailSender";
        public const string HasAttachmentsPropertyName = "HasAttachments";

        #endregion // Property Names

        #region UI Strings

        /// <summary>
        /// The text for the 'Copy to Clipboard' button.
        /// </summary>
        public static readonly string CopyToClipboardButtonText = Resources.Strings.ReportDialog_CopyToClipboard;

        /// <summary>
        /// The text for the 'Send Error Report' button.
        /// </summary>
        public static readonly string SendEmailButtonText = Resources.Strings.ReportDialog_SendErrorReport;

        /// <summary>
        /// The text for the 'email' label.
        /// </summary>
        public static readonly string EmailLabel = Resources.Strings.ReportDialog_EmailLabel;

        /// <summary>
        /// The text to assuage the user that email address is only for debug report, nothing more.
        /// </summary>
        public static readonly string EmailReminderText = Resources.Strings.ReportDialog_EmailReminder;

        /// <summary>
        /// The text for the 'Show Files' button.
        /// </summary>
        public static readonly string ShowFilesText = Resources.Strings.ReportDialog_ShowFilesButtonText;

        #endregion // UI Strings

        #region CopyToClipboardCommand

        /// <summary>
        /// The command to copy the report contents to the clipboard.
        /// </summary>
        public static readonly ICommand CopyToClipboardCommand = new RelayCommand(CopyToClipboard)
        {
            UniqueId = "INTV.Shared.ViewModel.ReportDialogViewModel.CopyToClipboardCommand",
            PreferredParameterType = typeof(ReportDialogViewModel),
            BlockWhenAppIsBusy = false
        };

        private static void CopyToClipboard(object parameter)
        {
            PlatformCopyToClipboard(parameter);
        }

        #endregion CopyToClipboardCommand

        #region CloseDialogCommand

        /// <summary>
        /// The command that closes the dialog box window.
        /// </summary>
        public static readonly RelayCommand CloseDialogCommand = new RelayCommand(OnCloseDialogCommand)
        {
            UniqueId = "INTV.Shared.ViewModel.ReportDialogViewModel.CloseDialogCommand",
            BlockWhenAppIsBusy = false
        };

        private static void OnCloseDialogCommand(object parameter)
        {
            PlaformOnCloseDialog(parameter);
        }

        #endregion CloseDialogCommand

        #region SendErrorReportEmailCommand

        /// <summary>
        /// The command that sends an error report email to support@intvfunhouse.com.
        /// </summary>
        public static readonly RelayCommand SendErrorReportEmailCommand = new RelayCommand(OnSendErrorReportEmail)
        {
            UniqueId = "INTV.Shared.ViewModel.ReportDialogViewModel.SendErrorReportEmailCommand",
            BlockWhenAppIsBusy = false,
            PreferredParameterType = typeof(ReportDialogViewModel)
        };

        private static void OnSendErrorReportEmail(object parameter)
        {
            var viewModel = parameter as ReportDialogViewModel;
            viewModel.SendEmailEnabled = false;
            viewModel.SendEmailButtonLabelText = Resources.Strings.ReportDialog_SendingErrorReport;
#if SMTP_SUPPORT
            // While sending email, disable the close button until we return. Hope this never hangs!
            viewModel.CloseDialogButtonEnabled = false;
#endif
            try
            {
                // If the address isn't valid, just use Anonymous.
                var sender = viewModel.EmailSender;
                try
                {
                    if (!string.IsNullOrWhiteSpace(sender))
                    {
                        var address = new System.Net.Mail.MailAddress(sender);
                        sender = address.Address;
                    }
                    else
                    {
                        sender = null;
                    }
                }
                catch (FormatException)
                {
                    sender = null;
                }
                sender = sender ?? "ltoflash@intvfunhouse.com";
                var subject = "LTO Flash UI Crash Report";
                var message = new System.Text.StringBuilder(subject);
                message.AppendLine().AppendLine("-----------------------------------------------");
                message.AppendFormat("{0} (64-bit: {1})", Environment.OSVersion, Environment.Is64BitOperatingSystem).AppendLine();
                message.AppendFormat("CLR Version: {0}", Environment.Version).AppendLine();
                var attachments = new List<string>(viewModel.Attachments.Distinct(PathComparer.Instance).Where(a => System.IO.File.Exists(a)));

                var errorLogPath = viewModel.GetErrorLog(false);
                if (System.IO.File.Exists(errorLogPath) && !attachments.Contains(errorLogPath, PathComparer.Instance))
                {
                    attachments.Add(errorLogPath);
                }
                if (attachments.Any())
                {
                    message.AppendLine().AppendLine("PLEASE INCLUDE THE FOLLOWING FILES ATTACHED TO YOUR MESSAGE!");
                    message.AppendLine("These may assist in diagnosing the problem.").AppendLine();
                    var fileNumber = 0;
                    foreach (var attachment in attachments)
                    {
                        message.AppendFormat("{0}: {1}", ++fileNumber, attachment).AppendLine();
                    }
                }

                message.AppendLine().AppendLine("Please include any further information, such as what you were doing when you encountered the problem.");
                message.AppendLine("--------------------------------------------------------------------").AppendLine();
                message.AppendLine("PLACE YOUR COMMENTS HERE");
                message.AppendLine().AppendLine("--------------------------------------------------------------------");
                message.AppendLine("Please do NOT remove or edit information below this point.");
                message.AppendLine("--------------------------------------------------------------------").AppendLine();
                message.AppendLine(viewModel.Message).AppendLine();
                message.AppendLine(viewModel.ReportText);

                var reportRecipient = "support@intvfunhouse.com";
#if SMTP_SUPPORT
                RunExternalProgram.SendEmailUsingSMTP(sender, reportRecipient, viewModel.EmailCCRecipients, subject, message.ToString(), attachments, viewModel, SendErrorReportComplete);
#else
                RunExternalProgram.SendEmail(sender, reportRecipient, viewModel.EmailCCRecipients, subject, message.ToString(), attachments);
#endif // SMTP_SUPPORT
            }
            catch (Exception e)
            {
                ReportSendErrorReportFailure(viewModel, e);
                viewModel.CloseDialogButtonEnabled = true;
            }
        }

#if SMTP_SUPPORT

        private static void SendErrorReportComplete(Exception exception, bool cancelled, object token)
        {
            var viewModel = token as ReportDialogViewModel;
            if (exception != null)
            {
                ReportSendErrorReportFailure(viewModel, exception);
            }
            else
            {
                viewModel.SendEmailButtonLabelText = "Error Report Sent";
            }
            viewModel.CloseDialogButtonEnabled = true;
        }

#endif

        private static void ReportSendErrorReportFailure(ReportDialogViewModel viewModel, Exception exception)
        {
            viewModel.SendEmailButtonLabelText = Resources.Strings.ReportDialog_SendMessageFailed;
            var firstLineOfError = string.IsNullOrEmpty(exception.Message) ? string.Empty : exception.Message.Split(System.Environment.NewLine.ToCharArray()).First();
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ReportDialog_UnableToSendReport_MessageFormat, firstLineOfError);
            PlatformPanic(message, Resources.Strings.ReportDialog_UnableToSendReport_Title);
        }

        #endregion // SendErrorReportEmailCommand

        #region ShowAttachmentsInFileSystemCommand

        /// <summary>
        /// Command to show suggested attached files in the file system.
        /// </summary>
        public static readonly ICommand ShowAttachmentsInFileSystemCommand = new RelayCommand(ShowAttachmentsInFileSystem, CanShowAttachmentsInFileSystem)
        {
            UniqueId = "INTV.Shared.ViewModel.ReportDialogViewModel.ShowAttachmentsInFileSystemCommand",
            PreferredParameterType = typeof(ReportDialogViewModel),
        };

        private static void ShowAttachmentsInFileSystem(object parameter)
        {
            var viewModel = parameter as ReportDialogViewModel;
            viewModel.Attachments.Distinct(PathComparer.Instance).Where(a => System.IO.File.Exists(a) || System.IO.Directory.Exists(a)).RevealInFileSystem();
        }

        private static bool CanShowAttachmentsInFileSystem(object parameter)
        {
            var viewModel = parameter as ReportDialogViewModel;
            return (viewModel != null) && viewModel.HasAttachments;
        }

        #endregion // ShowAttachmentsInFileSystemCommand

        private static bool _unableToLogErrors;

        /// <summary>
        /// Initializes a new instance of the ReportDialogViewModel class.
        /// </summary>
        public ReportDialogViewModel()
        {
            Title = Resources.Strings.ReportDialog_Title;
            Message = Resources.Strings.ReportDialog_Apology;
            CloseDialogButtonText = Resources.Strings.ReportDialog_Exit;
            CloseDialogButtonEnabled = true;
            ShowCopyToClipboardButton = true;
            DoNotShowAgainText = Resources.Strings.DoNotShowAgain;
            ShowDoNotShowAgain = false;
            SendEmailButtonLabelText = SendEmailButtonText;
            SendEmailEnabled = true;
            ShowSendEmailButton = true;
#if SMTP_SUPPORT
            ShowEmailSender = true;
#endif
            TextWrapping = INTV.Shared.View.OSTextWrapping.NoWrap;
            Attachments = new ObservableCollection<string>();
            Attachments.CollectionChanged += (s, e) => RaisePropertyChanged(HasAttachmentsPropertyName);
            Initialize();
        }

        /// <summary>
        /// Gets or sets the title for the dialog.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { AssignAndUpdateProperty(TitlePropertyName, value, ref _title); }
        }
        private string _title;

        /// <summary>
        /// Gets or sets the general message for the report.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { AssignAndUpdateProperty(MessagePropertyName, value, ref _message); }
        }
        private string _message;

        /// <summary>
        /// Gets or sets the text for the 'Close' button on the dialog.
        /// </summary>
        public string CloseDialogButtonText
        {
            get { return _closeDialogButtonText; }
            set { AssignAndUpdateProperty(CloseDialogButtonTextPropertyName, value, ref _closeDialogButtonText); }
        }
        private string _closeDialogButtonText;

        /// <summary>
        /// Gets a value indicating whether the "Close" button should be enabled.
        /// </summary>
        public bool CloseDialogButtonEnabled
        {
            get { return _closeDialogButtonEnabled; }
            private set { AssignAndUpdateProperty(CloseDialogButtonEnabledPropertyName, value, ref _closeDialogButtonEnabled); }
        }
        private bool _closeDialogButtonEnabled;

        /// <summary>
        /// Gets or sets a value indicating whether to show the 'Copy to Clipboard' button.
        /// </summary>
        public bool ShowCopyToClipboardButton
        {
            get { return _showCopyToClipboardButton; }
            set { AssignAndUpdateProperty(ShowCopyToClipboardButtonPropertyName, value, ref _showCopyToClipboardButton); }
        }
        private bool _showCopyToClipboardButton;

        /// <summary>
        /// Gets or sets the text wrapping mode for the report area.
        /// </summary>
        public INTV.Shared.View.OSTextWrapping TextWrapping
        {
            get { return _textWrapping; }
            set { AssignAndUpdateProperty(TextWrappingPropertyName, value, ref _textWrapping); }
        }
        private INTV.Shared.View.OSTextWrapping _textWrapping;

        /// <summary>
        /// Gets or sets text of the report.
        /// </summary>
        public string ReportText
        {
            get { return GetReportText(); }
            set { AssignAndUpdateProperty(ReportTextPropertyName, value, ref _reportText); }
        }
        private string _reportText;

        /// <summary>
        /// Gets or sets the exception to report.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
            set { AssignAndUpdateProperty(ExceptionPropertyName, value, ref _exception, (p, v) => SetException(v)); }
        }
        private Exception _exception;

        /// <summary>
        /// Gets a list of the files to attach to an email message that the dialog may send.
        /// </summary>
        public ObservableCollection<string> Attachments { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has attachments.
        /// </summary>
        public bool HasAttachments
        {
            get { return Attachments.Distinct(PathComparer.Instance).Any(f => System.IO.File.Exists(f) || System.IO.Directory.Exists(f)); }
        }

        /// <summary>
        /// Gets or sets the text for the Do Not Show Again checkbox.
        /// </summary>
        public string DoNotShowAgainText
        {
            get { return _doNotShowAgainText; }
            set { AssignAndUpdateProperty(DoNotShowAgainTextPropertyName, value, ref _doNotShowAgainText); }
        }
        private string _doNotShowAgainText;

        /// <summary>
        /// Gets or sets a value indicating whether to show a 'Do Not Show Again' checkbox.
        /// </summary>
        public bool ShowDoNotShowAgain
        {
            get { return _showDoNotShowAgain; }
            set { AssignAndUpdateProperty(ShowDoNotShowAgainPropertyName, value, ref _showDoNotShowAgain); }
        }
        private bool _showDoNotShowAgain;

        /// <summary>
        /// Gets or sets the text for the 'Send Email' button.
        /// </summary>
        public string SendEmailButtonLabelText
        {
            get { return _sendEmailButtonLabelText; }
            set { AssignAndUpdateProperty(SendEmailButtonLabelTextPropertyName, value, ref _sendEmailButtonLabelText); }
        }
        private string _sendEmailButtonLabelText;

        /// <summary>
        /// Gets or sets a value indicating whether to show the 'Send Error Report' button.
        /// </summary>
        public bool ShowSendEmailButton
        {
            get { return _showSendEmailButton; }
            set { AssignAndUpdateProperty(ShowSendEmailButtonPropertyName, value, ref _showSendEmailButton); }
        }
        private bool _showSendEmailButton;

        /// <summary>
        /// Gets or sets a value indicating whether the 'Send Error Report' button should be enabled.
        /// </summary>
        public bool SendEmailEnabled
        {
            get { return _sendEmailEnabled; }
            set { AssignAndUpdateProperty(SendEmailEnabledPropertyName, value, ref _sendEmailEnabled); }
        }
        private bool _sendEmailEnabled;

        /// <summary>
        /// Gets or sets the email address of the message sender, if applicable.
        /// </summary>
        public string EmailSender
        {
            get { return _emailSender; }
            set { AssignAndUpdateProperty(EmailSenderPropertyName, value, ref _emailSender); }
        }
        private string _emailSender;

        /// <summary>
        /// Gets or sets a value indicating whether to show the field for entering an email address.
        /// </summary>
        public bool ShowEmailSender
        {
            get { return _showEmailSender; }
            set { AssignAndUpdateProperty(ShowEmailSenderPropertyName, value, ref _showEmailSender); }
        }
        private bool _showEmailSender;

        /// <summary>
        /// Gets or sets the email CC recipients.
        /// </summary>
        public IEnumerable<string> EmailCCRecipients { get; set; }

        private string ErrorLogPath
        {
            get
            {
                if (string.IsNullOrEmpty(_errorLogPath))
                {
                    var fileName = "ErrorLog_" + PathUtils.GetTimeString() + ".txt";
                    _errorLogPath = System.IO.Path.Combine(INTV.Shared.Model.RomListConfiguration.Instance.ErrorLogDirectory, fileName);
                }
                return _errorLogPath;
            }
        }
        private string _errorLogPath;

        private Logger Logger { get; set; }

        /// <summary>
        /// Operating system-specific code to execute when the dialog closes.
        /// </summary>
        /// <param name="parameter">Custom data for the OS-specific method.</param>
        static partial void PlaformOnCloseDialog(object parameter);

        private string GetReportText()
        {
            var text = new System.Text.StringBuilder(_reportText);
            if (_exception != null)
            {
                if (!string.IsNullOrEmpty(_reportText))
                {
                    text.AppendLine().AppendLine();
                }
                text.AppendLine(_exception.ToString());
            }
            return text.ToString();
        }

        private void SetException(Exception exception)
        {
            if (exception != null)
            {
                var path = GetErrorLog(true);
                Attachments.Add(path);
                RaisePropertyChanged(HasAttachmentsPropertyName);
            }
            RaisePropertyChanged(ReportTextPropertyName);
        }

        private string GetErrorLog(bool writeIfNeeded)
        {
            string errorLogPath = ErrorLogPath;
            try
            {
                if (Logger == null)
                {
                    Logger = new Logger(errorLogPath);
                }
                if (writeIfNeeded)
                {
                    Logger.Log(ReportText);
                }
            }
            catch (Exception e)
            {
                if (!_unableToLogErrors)
                {
                    errorLogPath = null;
                    _unableToLogErrors = true;
                    var firstLineOfError = string.IsNullOrEmpty(e.Message) ? string.Empty : e.Message.Split(System.Environment.NewLine.ToCharArray()).First();
                    var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ReportDialog_LogErrorFailed_MessageFormat, firstLineOfError);
                    PlatformPanic(message, Resources.Strings.ReportDialog_UnableToSendReport_Title);
                }
            }
            return errorLogPath;
        }
    }
}
