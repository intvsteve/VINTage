// <copyright file="ReportDialogController.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

#if __UNIFIED__
using nfloat = System.nfloat;
using nint = System.nint;
#else
using nfloat = System.Single;
using nint = System.Int32;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// View controller for <see cref="ReportDialog"/> visual.
    /// </summary>
    public partial class ReportDialogController : NSWindowController
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public ReportDialogController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public ReportDialogController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public ReportDialogController()
            : base("ReportDialog")
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
            DialogDataContext = new ReportDialogViewModel();
            DialogDataContext.PropertyChanged += HandlePropertyChanged;
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the window as a strongly typed value.
        /// </summary>
        public new ReportDialog Window { get { return (ReportDialog)base.Window; } }

        /// <summary>
        /// Gets the message to display.
        /// </summary>
        [OSExport(ReportDialogViewModel.MessagePropertyName)]
        public string Message { get { return DialogDataContext.Message; } }

        /// <summary>
        /// Gets the text to show on the cancel / close dialog button.
        /// </summary>
        [OSExport(ReportDialogViewModel.CloseDialogButtonTextPropertyName)]
        public string CloseDialogButtonText { get { return DialogDataContext.CloseDialogButtonText; } }

        /// <summary>
        /// Gets a value indicating whether the close dialog button should be enabled.
        /// </summary>
        [OSExport(ReportDialogViewModel.CloseDialogButtonEnabledPropertyName)]
        public bool CloseDialogButtonEnabled { get { return DialogDataContext.CloseDialogButtonEnabled; } }

        /// <summary>
        /// Gets a value indicating whether to show the 'do not show again' checkbox.
        /// </summary>
        [OSExport(ReportDialogViewModel.ShowDoNotShowAgainPropertyName)]
        public bool ShowDoNotShowAgain { get { return DialogDataContext.ShowDoNotShowAgain; } }

        /// <summary>
        /// Gets a value indicating whether to show the Copy to Clipboard button.
        /// </summary>
        [OSExport(ReportDialogViewModel.ShowCopyToClipboardButtonPropertyName)]
        public bool ShowCopyToClipboardButton { get { return DialogDataContext.ShowCopyToClipboardButton; } }

        /// <summary>
        /// Gets the text to show on the 'Send Email' button.
        /// </summary>
        [OSExport(ReportDialogViewModel.SendEmailButtonLabelTextPropertyName)]
        public string SendEmailButtonLabelText { get { return DialogDataContext.SendEmailButtonLabelText; } }

        /// <summary>
        /// Gets a value indicating whether to enable the send email button.
        /// </summary>
        [OSExport(ReportDialogViewModel.SendEmailEnabledPropertyName)]
        public bool SendEmailEnabled
        {
            get { return DialogDataContext.SendEmailEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether to show the Send Email button.
        /// </summary>
        [OSExport(ReportDialogViewModel.ShowSendEmailButtonPropertyName)]
        public bool ShowSendEmailButton
        {
            get { return DialogDataContext.ShowSendEmailButton; }
        }

        /// <summary>
        /// Gets or sets the email address of the sender of a report.
        /// </summary>
        [OSExport(ReportDialogViewModel.EmailSenderPropertyName)]
        public NSString EmailSender
        {
            get { return string.IsNullOrEmpty(DialogDataContext.EmailSender) ? null : (NSString)DialogDataContext.EmailSender; }
            set { DialogDataContext.EmailSender = value; }
        }

        /// <summary>
        /// Gets or sets the color of the text for the email address field.
        /// </summary>
        [OSExport("EmailTextColor")]
        public NSColor EmailTextColor
        {
            get { return _emailTextColor; }
            set { _emailTextColor = value; }
        }
        private NSColor _emailTextColor = NSColor.ControlText;

        /// <summary>
        /// Gets whether there should be email attachments.
        /// </summary>
        [OSExport(ReportDialogViewModel.HasAttachmentsPropertyName)]
        public bool HasAttachments { get { return DialogDataContext.HasAttachments; } }

        private ReportDialogViewModel DialogDataContext { get; set; }

        /// <inheritdoc/>
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            Window.DataContext = DialogDataContext;
            Window.WindowShouldClose = ShouldClose;

            var propertiesToUpdate = new[] {
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
                //ReportDialogViewModel.EmailSenderPropertyName,
                ReportDialogViewModel.HasAttachmentsPropertyName,
            };
            foreach (var propertyName in propertiesToUpdate)
            {
                HandlePropertyChanged(DialogDataContext, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
            var split = Window.ContentView.FindChild<MySplitView>();
            if (!DialogDataContext.ShowDoNotShowAgain)
            {
                split.CollapsePart((int)Parts.DoNotShowAgainCheckbox);
            }
            if (DialogDataContext.ShowEmailSender)
            {
                var emailEntry = Window.ContentView.FindChild<NSTextField>(t => t.Tag == 1);
                var formatter = emailEntry.Formatter as INTV.Shared.Converter.ValidEmailAddressFormatter;
                formatter.TextUpdateCallback = UpdateEmailSenderTextColor;
            }
            else
            {
                split.CollapsePart((int)Parts.EmailAddressArea);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (DialogDataContext != null)
            {
                DialogDataContext.PropertyChanged -= HandlePropertyChanged;
            }
            // MonoMac has some problems w/ lifetime. This was an attempt to prevent leaking dialogs.
            // However, there are cases that result in over-release that are not easily identified.
            // So, leak it is! :(
            // base.Dispose(disposing);
        }

        private void UpdateEmailSenderTextColor(string newValue, bool isValid)
        {
            DialogDataContext.EmailSender = newValue;
            EmailTextColor = isValid ? NSColor.ControlText : NSColor.Red;
            this.RaiseChangeValueForKey("EmailTextColor");
        }

        private void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case ReportDialogViewModel.TitlePropertyName:
                    Window.Title = DialogDataContext.Title;
                    break;
                case ReportDialogViewModel.MessagePropertyName:
                case ReportDialogViewModel.SendEmailEnabledPropertyName:
                case ReportDialogViewModel.SendEmailButtonLabelTextPropertyName:
                case ReportDialogViewModel.ShowSendEmailButtonPropertyName:
                case ReportDialogViewModel.CloseDialogButtonTextPropertyName:
                case ReportDialogViewModel.CloseDialogButtonEnabledPropertyName:
                case ReportDialogViewModel.ShowCopyToClipboardButtonPropertyName:
                case ReportDialogViewModel.ShowDoNotShowAgainPropertyName:
                case ReportDialogViewModel.HasAttachmentsPropertyName:
                    this.RaiseChangeValueForKey(e.PropertyName);
                    break;
                case ReportDialogViewModel.EmailSenderPropertyName:
                    break;
                case ReportDialogViewModel.ReportTextPropertyName:
                    ReportText.Value = DialogDataContext.ReportText;
                    break;
                case ReportDialogViewModel.TextWrappingPropertyName:
                    break;
                case ReportDialogViewModel.DoNotShowAgainTextPropertyName:
                    break;
            }
        }

        private bool ShouldClose(NSObject sender)
        {
            return DialogDataContext.CloseDialogButtonEnabled;
        }

        partial void OnShowAttachments(NSObject sender)
        {
            ReportDialogViewModel.ShowAttachmentsInFileSystemCommand.Execute(DialogDataContext);
        }

        partial void OnSendErrorReport(NSObject sender)
        {
            if (DialogDataContext.ShowEmailSender)
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
            ReportDialogViewModel.SendErrorReportEmailCommand.Execute(DialogDataContext);
        }

        partial void OnCopyToClipboard(NSObject sender)
        {
            ReportDialogViewModel.CopyToClipboardCommand.Execute(DialogDataContext);
        }

        partial void OnExit(NSObject sender)
        {
            Window.EndDialog(NSRunResponse.Stopped);
        }

        private enum Parts
        {
            TextArea = 0,
            DoNotShowAgainCheckbox,
            EmailAddressArea
        }

        /// <summary>
        /// Subclass of SplitView to get desired behaviors.
        /// </summary>
        [Register ("MySplitView")]
        public class MySplitView : NSSplitView
        {
            #region Constructors

            /// <summary>
            /// Called when created from unmanaged code.
            /// </summary>
            /// <param name="handle">Native pointer to NSView.</param>
            public MySplitView(System.IntPtr handle)
                : base (handle)
            {
                Initialize();
            }

            /// <summary>
            /// Called when created directly from a XIB file.
            /// </summary>
            /// <param name="coder">Used to deserialize from a XIB.</param>
            [Export ("initWithCoder:")]
            public MySplitView(NSCoder coder)
                : base (coder)
            {
                Initialize();
            }

            /// <summary>Shared initialization code.</summary>
            private void Initialize()
            {
            }

            #endregion // Constructors

            /// <inheritdoc/>
            /// <remarks>We don't want a visible divider.</remarks>
            public override nfloat DividerThickness
            {
                get { return 0; }
            }

            /// <inheritdoc/>
            public override nint Tag
            {
                get { return 0; }
            }

            private NSView[] OriginalParts { get; set; }

            /// <inheritdoc/>
            public override void AwakeFromNib()
            {
                base.AwakeFromNib();
                OriginalParts = Subviews;
            }

            /// <summary>
            /// Collapse the part completely by removing from the view. It works. Maybe there's a better way?
            /// </summary>
            /// <param name="index">Index of the part to collapse.</param>
            public void CollapsePart(int index)
            {
                var part = OriginalParts[index];
                if (part.Superview != null)
                {
                    part.RemoveFromSuperview();
                }
            }
        }

        /// <summary>
        /// Custom delegate for the split view to ensure desired behaviors.
        /// </summary>
        private class SplitDelegate : NSSplitViewDelegate
        {
            /// <inheritdoc/>
            public override bool ShouldHideDivider(NSSplitView splitView, nint dividerIndex)
            {
                return true;
            }

            /// <inheritdoc/>
            public override bool CanCollapse(NSSplitView splitView, NSView subview)
            {
                return true;
            }

            /// <inheritdoc/>
            public override nfloat ConstrainSplitPosition(NSSplitView splitView, nfloat proposedPosition, nint subviewDividerIndex)
            {
                return proposedPosition;
            }
        }
    }
}
