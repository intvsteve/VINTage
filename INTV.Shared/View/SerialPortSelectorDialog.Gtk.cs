// <copyright file="SerialPortSelectorDialog.Gtk.cs" company="INTV Funhouse">
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

using INTV.Shared.Commands;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class SerialPortSelectorDialog : Gtk.Dialog, IFakeDependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.SerialPortSelectorDialog"/> class.
        /// </summary>
        /// <param name="viewModel">The data context for the dialog.</param>
        private SerialPortSelectorDialog(SerialPortSelectorDialogViewModel viewModel)
            : base(string.Empty, INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow, Gtk.DialogFlags.Modal)
        {
            DataContext = viewModel;
            this.Build();
            _serialPortSelector.Initialize(viewModel.PortSelectorViewModel);
            buttonOk.Label = viewModel.SelectButtonText;
            buttonOk.Data[CommandGroup.AttachedCommandPropertyName] = SerialPortSelectorDialogViewModel.SelectPortCommand;
            HandleCanSelectPortChanged(null, null);
            INTV.Shared.ComponentModel.CommandManager.RequerySuggested += HandleCanSelectPortChanged;
            SerialPortSelectorDialogViewModel.SelectPortCommand.CanExecuteChanged += HandleCanSelectPortChanged;
            buttonCancel.Label = viewModel.CancelButtonText;
            buttonCancel.Data[CommandGroup.AttachedCommandPropertyName] = SerialPortSelectorDialogViewModel.CancelSelectPortCommand;
        }

        #region IFakeDependencyObject Properties

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        #endregion // IFakeDependencyObject Properties

        private SerialPortSelectorDialogViewModel ViewModel
        {
            get { return DataContext as SerialPortSelectorDialogViewModel; }
        }

        #region IFakeDependencyObject

        /// <inheritdoc/>
        public object GetValue(string propertyName)
        {
            return this.GetValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue(string propertyName, object value)
        {
            this.SetValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        /// <summary>
        /// For compatibility with the Mac version of the dialog.
        /// </summary>
        /// <returns>Returns <c>true</c> if the "OK" button was clicked.</returns>
        public bool? ShowDialog()
        {
            return VisualHelpers.ShowDialog(this);
        }

        private static SerialPortSelectorDialog Create(SerialPortSelectorDialogViewModel viewModel)
        {
            var dialog = new SerialPortSelectorDialog(viewModel);
            return dialog;
        }

        private void HandleCanSelectPortChanged(object sender, System.EventArgs e)
        {
            var canExecute = SerialPortSelectorDialogViewModel.SelectPortCommand.CanExecute(DataContext);
            buttonOk.Sensitive = canExecute;
        }
    }
}
