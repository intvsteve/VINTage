// <copyright file="ReportDialog.xaml.cs" company="INTV Funhouse">
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

using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Interaction logic for ReportDialog.xaml
    /// </summary>
    public partial class ReportDialog : System.Windows.Window
    {
        private ReportDialog()
        {
            InitializeComponent();
        }

        private ReportDialog(string title, string message)
            : this()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            if (title != null)
            {
                ViewModel.Title = title;
            }
            if (message != null)
            {
                ViewModel.Message = message;
            }
        }

        #region Properties

        /// <summary>
        /// Gets the ViewModel for the dialog.
        /// </summary>
        public ReportDialogViewModel ViewModel
        {
            get { return DataContext as ReportDialogViewModel; }
        }

        #endregion

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

        /// <summary>
        /// Delays invoking the dialog to the next opportunity.
        /// </summary>
        /// <param name="closeButtonText">Text for the dialog's close button.</param>
        /// <param name="onDialogClosed">Delegate to call when dialog completes.</param>
        /// <remarks>This mode of invocation is usually used if some form of visual refresh should
        /// happen before the dialog appears.</remarks>
        public void BeginInvokeDialog(string closeButtonText, System.Action<bool?> onDialogClosed)
        {
            if (onDialogClosed == null)
            {
                onDialogClosed = (b) => { };
            }
            Dispatcher.BeginInvoke(new System.Action(() => onDialogClosed(ShowDialog(closeButtonText))));
        }

        /// <summary>
        /// Run the dialog, returning with a result.
        /// </summary>
        /// <param name="closeButtonText">The text to show for the close button. If <c>null</c>, the default is used.</param>
        /// <returns>A nullable value indicating whether the "OK" button was clicked. If the result has a value,
        /// and the value is <c>true</c>, execute the expected action. If the result has a value that is <c>false</c>,
        /// treat the operation as cancel. If the return value is <c>null</c> an error occurred.</returns>
        public bool? ShowDialog(string closeButtonText)
        {
            bool? result = null;
            INTV.Shared.Utility.OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
                    Owner = INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow;
                    ViewModel.CloseDialogButtonText = closeButtonText;
                    result = ShowDialog();
                });
            return result;
        }
    }
}
