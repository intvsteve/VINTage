// <copyright file="ReportDialog.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

using System.Linq;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class ReportDialog : Gtk.Dialog, IFakeDependencyObject
    {
        private int _messageAreaWidth;

        private ReportDialog(string title, string message)
                : base(title, INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow, Gtk.DialogFlags.Modal)
        {
            var viewModel = new ReportDialogViewModel();
            viewModel.PropertyChanged += HandlePropertyChanged;
            DataContext = viewModel;
            this.Build();
            _message.SizeAllocated += HandleSizeAllocated;

            if (title != null)
            {
                viewModel.Title = title;
            }
            if (message != null)
            {
                viewModel.Message = message;
            }
            var propertiesToUpdate = new[]
                {
                ReportDialogViewModel.TitlePropertyName,
                ReportDialogViewModel.MessagePropertyName,
                ReportDialogViewModel.CloseDialogButtonTextPropertyName,
                ReportDialogViewModel.CloseDialogButtonEnabledPropertyName,
                ReportDialogViewModel.ShowCopyToClipboardButtonPropertyName,
                ReportDialogViewModel.DoNotShowAgainTextPropertyName,
                ReportDialogViewModel.ShowDoNotShowAgainPropertyName,
                ReportDialogViewModel.SendEmailButtonLabelTextPropertyName,
                ReportDialogViewModel.ShowSendEmailButtonPropertyName,
                ReportDialogViewModel.SendEmailEnabledPropertyName,
                ////ReportDialogViewModel.EmailSenderPropertyName,
                ReportDialogViewModel.HasAttachmentsPropertyName,
            };
            foreach (var propertyName in propertiesToUpdate)
            {
                HandlePropertyChanged(viewModel, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #region IFakeDependencyObject Properties

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        #endregion // IFakeDependencyObject Properties

        private ReportDialogViewModel ViewModel
        {
            get
            {
                var viewModel = DataContext as ReportDialogViewModel;
                if (viewModel == null)
                {
                    var propertyInfo = DataContext.GetType().GetProperties().First(pi => pi.PropertyType == typeof(ReportDialogViewModel));
                    viewModel = (ReportDialogViewModel)propertyInfo.GetValue(DataContext, null);
                }
                return viewModel;
            }
        }

        /// <summary>
        /// Creates a new instance of the ReportDialog.
        /// </summary>
        /// <param name="title">The title for the dialog.</param>
        /// <param name="message">The message to display in the dialog.</param>
        /// <returns>The dialog instance.</returns>
        public static ReportDialog Create(string title, string message)
        {
            var dialog = new ReportDialog(title, message);
            return dialog;
        }

        #region IFakeDependencyObject Methods

        /// <inheritdoc/>
        public object GetValue(string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject Methods

        /// <summary>
        /// Starts the dialog on the applications UI thread.
        /// </summary>
        /// <param name="closeButtonText">The text for the dialog's "close" button.</param>
        /// <param name="onDialogClosed">A method to call when the dialog is dismissed. This method will be invoked on the UI thread.</param>
        /// <remarks>It is important to note that this method immediately returns, as the dialog must run on the main (UI) thread. Any
        /// processing that must be done based upon the response of the dialog should occur on the <paramref name="onDialogClosed"/> delegate.</remarks>
        public void BeginInvokeDialog(string closeButtonText, System.Action<bool?> onDialogClosed)
        {
            if (onDialogClosed == null)
            {
                onDialogClosed = (b) => { };
            }
            INTV.Shared.Utility.SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new System.Action(() => onDialogClosed(ShowDialog(closeButtonText))));
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>A nullable value; if <c>true</c> the OK or equivalent was clicked; if <c>false</c>,
        /// the dialog was cancelled.</returns>
        public bool? ShowDialog()
        {
            return ShowDialog(null);
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="closeButtonText">Close button text; if <c>null</c> or empty, the default is used.</param>
        /// <returns>A nullable value; if <c>true</c> the OK or equivalent was clicked; if <c>false</c>,
        /// the dialog was cancelled.</returns>
        public bool? ShowDialog(string closeButtonText)
        {
            bool? result = null;
            OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
                    if (!string.IsNullOrEmpty(closeButtonText))
                    {
                        ViewModel.CloseDialogButtonText = closeButtonText;
                    }
                    result = VisualHelpers.ShowDialog(this);
                });
            return result;
        }

        private void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandlePropertyChangedCore);
        }

        private void HandlePropertyChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case ReportDialogViewModel.TitlePropertyName:
                    Title = ViewModel.Title;
                    break;
                case ReportDialogViewModel.MessagePropertyName:
                    _message.Text = ViewModel.Message;
                    break;
                case ReportDialogViewModel.SendEmailEnabledPropertyName:
                    _sendErrorReport.Sensitive = ViewModel.SendEmailEnabled;
                    break;
                case ReportDialogViewModel.SendEmailButtonLabelTextPropertyName:
                    _sendErrorReport.Label = ViewModel.SendEmailButtonLabelText;
                    break;
                case ReportDialogViewModel.ShowSendEmailButtonPropertyName:
                    _sendErrorReport.Visible = ViewModel.ShowSendEmailButton;
                    break;
                case ReportDialogViewModel.CloseDialogButtonTextPropertyName:
                    _close.Label = ViewModel.CloseDialogButtonText;
                    break;
                case ReportDialogViewModel.CloseDialogButtonEnabledPropertyName:
                    _close.Sensitive = ViewModel.CloseDialogButtonEnabled;
                    break;
                case ReportDialogViewModel.ShowCopyToClipboardButtonPropertyName:
                    _copyToClipboard.Visible = ViewModel.ShowCopyToClipboardButton;
                    break;
                case ReportDialogViewModel.ShowDoNotShowAgainPropertyName:
                    _doNotShowAgain.Visible = ViewModel.ShowDoNotShowAgain;
                    break;
                case ReportDialogViewModel.HasAttachmentsPropertyName:
                    _showAttachedFiles.Visible = ViewModel.HasAttachments;
                    break;
                case ReportDialogViewModel.EmailSenderPropertyName:
                    // not implemented
                    break;
                case ReportDialogViewModel.ReportTextPropertyName:
                    _reportText.Buffer.Text = ViewModel.ReportText;
                    break;
                case ReportDialogViewModel.TextWrappingPropertyName:
                    _reportText.WrapMode = (Gtk.WrapMode)ViewModel.TextWrapping;
                    break;
                case ReportDialogViewModel.DoNotShowAgainTextPropertyName:
                    _doNotShowAgain.Label = ViewModel.DoNotShowAgainText;
                    break;
            }
        }

        protected void HandleShowAttachedFiles(object sender, System.EventArgs e)
        {
            ReportDialogViewModel.ShowAttachmentsInFileSystemCommand.Execute(ViewModel);
        }

        protected void HandleSendErrorReport(object sender, System.EventArgs e)
        {
#if false // COPIED FROM MonoMac)
            if (ViewModel.ShowEmailSender)
            {
                // Can't get this to work. Using a binding in Interface Builder, the dialog
                // exits as soon as you click in the text field. (Haven't tried w/o using the custom formatter.)
                // The NSFormatter may be wrong, too -- MonoMac doesn't have a good class there, and
                // I can't get the attributed string stuff to work.
                // So, at this point, if you type in an email address and hit 'enter', the dialog
                // exits. If you type in an email address and hit Send, due to the hacky nature of
                // the formatter, the desired email address will be used, BUT the email sender
                // field is reset to empty.
                // Haven't looked into implementing a custom delegate, or doing the same kind of
                // thing as is done for editing table cells. It may be that trying to use
                // a custom NSFormatter in MonoMac is just a lost cause.
                var emailEntry = Window.ContentView.FindChild<NSTextField>(t => t.Tag == 1);
                // emailEntry.StringValue = DialogDataContext.EmailSender;
                emailEntry.ValidateEditing();
            }
#endif // false
            ReportDialogViewModel.SendErrorReportEmailCommand.Execute(ViewModel);
        }

        protected void HandleCopyToClipboard(object sender, System.EventArgs e)
        {
            ReportDialogViewModel.CopyToClipboardCommand.Execute(ViewModel);
        }

        protected void HandleDoNotShowAgainToggled(object sender, System.EventArgs e)
        {
            // TODO: This feature hasn't been implemented
        }

        private void HandleSizeAllocated(object o, Gtk.SizeAllocatedArgs args)
        {
            var message = o as Gtk.Label;
            var newWidth = args.Allocation.Size.Width;
            if (newWidth != _messageAreaWidth)
            {
                // Totally arbitrary fudge. Without it, you can't make the window narrower.
                message.SetSizeRequest(newWidth - 8, -1);
            }
            _messageAreaWidth = newWidth;
        }
    }
}
