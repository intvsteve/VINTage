// <copyright file="ReportDialog.Mac.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class ReportDialog : NSPanel, System.ComponentModel.INotifyPropertyChanged, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public ReportDialog(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public ReportDialog(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ReportDialog"/>.
        /// </summary>
        /// <param name="title">The title to display in the dialog.</param>
        /// <param name="message">A brief message to display.</param>
        /// <returns>A new instance of the dialog, which can be further configured before display.</returns>
        public static ReportDialog Create(string title, string message)
        {
            var dialogController = new ReportDialogController();
            var dialog = dialogController.Window;
            dialog.Controller = dialogController;
            if (!string.IsNullOrWhiteSpace(title))
            {
                dialog.ViewModel.Title = title;
            }
            if (!string.IsNullOrWhiteSpace(message))
            {
                dialog.ViewModel.Message = message.SafeString();
            }
            return dialog;
        }

        #region INotifyPropertyChanged Implementation

        /// <inheritdoc/>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged Implementation

        #region IFakeDependencyObject Properties

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value, PropertyChanged); }
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

        private NSWindowController Controller { get; set; }

        /// <summary>
        /// Gets or sets the dialog response.
        /// </summary>
        internal NSRunResponse Response { get; set; }

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
                    result = this.ShowDialog(false);
                });
            return result;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (Controller != null))
            {
                Controller.Dispose();
                Controller = null;
            }

            // MonoMac has some problems w/ lifetime. This was an attempt to prevent leaking dialogs.
            // However, there are cases that result in over-release that are not easily identified.
            // So, leak it is! :(
            // base.Dispose(disposing);
        }
    }
}
