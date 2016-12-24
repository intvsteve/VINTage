// <copyright file="SerialPortSelectorDialogViewModel.cs" company="INTV Funhouse">
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

using INTV.Shared.ComponentModel;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the dialog used to select a serial port.
    /// </summary>
    public partial class SerialPortSelectorDialogViewModel : ViewModelBase
    {
        #region CancelSelectPortCommand

        /// <summary>
        /// The command that cancels port selection dialog.
        /// </summary>
        public static readonly RelayCommand CancelSelectPortCommand = new RelayCommand(OnCancel)
        {
            UniqueId = "INTV.Shared.ViewModel.SerialPortSelectorDialogViewModel.CancelSelectPortCommand",
            BlockWhenAppIsBusy = false
        };

        private static void OnCancel(object parameter)
        {
            var viewModel = parameter as SerialPortSelectorDialogViewModel;
            viewModel.DialogResult = false;
        }

        #endregion // CancelSelectPortCommand

        #region SelectPortCommand

        /// <summary>
        /// The command to select a port and dismiss the dialog.
        /// </summary>
        public static readonly RelayCommand SelectPortCommand = new RelayCommand(OnSelectPort, CanSelectPort)
        {
            UniqueId = "INTV.Shared.ViewModel.SerialPortSelectorDialogViewModel.SelectPortCommand",
            BlockWhenAppIsBusy = false
        };

        private static void OnSelectPort(object parameter)
        {
            var viewModel = parameter as SerialPortSelectorDialogViewModel;
            viewModel.DialogResult = true;
        }

        private static bool CanSelectPort(object parameter)
        {
            var viewModel = parameter as SerialPortSelectorDialogViewModel;
            return (viewModel != null) && !string.IsNullOrEmpty(viewModel.SelectedSerialPort);
        }

        #endregion // SelectPortCommand

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.ViewModel.SerialPortSelectorDialogViewModel"/> class.
        /// </summary>
        /// <param name="selectPortViewModel">The ViewModel to use for the dialog.</param>
        public SerialPortSelectorDialogViewModel(SerialPortSelectorViewModel selectPortViewModel)
        {
            PortSelectorViewModel = selectPortViewModel;
            PortSelectorViewModel.PortSelectionCommitted += PortSelectionCommitted;
            Initialize();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the title for the dialog.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the text for the 'select port' button.
        /// </summary>
        public string SelectButtonText { get; set; }

        /// <summary>
        /// Gets or sets the text for the cancel button.
        /// </summary>
        public string CancelButtonText { get; set; }

        /// <summary>
        /// Gets or sets whether the cancel button should be visible.
        /// </summary>
        public string ShowCancelButton { get; set; }

        /// <summary>
        /// Gets or sets the dialog's result. OK (<c>true</c>), Cancel (<c>false</c>), or abort (<c>null</c>).
        /// </summary>
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { AssignAndUpdateProperty("DialogResult", value, ref _dialogResult); }
        }
        private bool? _dialogResult;

        /// <summary>
        /// Gets or sets the ViewModel for the port selection.
        /// </summary>
        public SerialPortSelectorViewModel PortSelectorViewModel { get; set; }

        #region PortSelectorViewModel Properties

        /// <summary>
        /// Gets or sets the prompt text.
        /// </summary>
        public string Prompt
        {
            get { return PortSelectorViewModel.Prompt; }
            set { PortSelectorViewModel.Prompt = value; }
        }

        /// <summary>
        /// Gets or sets the selected serial port.
        /// </summary>
        public string SelectedSerialPort
        {
            get { return PortSelectorViewModel.SelectedSerialPort; }
            set { PortSelectorViewModel.SelectedSerialPort = value; }
        }

        /// <summary>
        /// Gets or sets the selected baud rate.
        /// </summary>
        public int SelectedBaudRate
        {
            get { return PortSelectorViewModel.SelectedBaudRate; }
            set { PortSelectorViewModel.SelectedBaudRate = value; }
        }

        #endregion // PortSelectorViewModel

        #endregion // Properties

        private void Initialize()
        {
            Title = Resources.Strings.SelectSerialPortDialog_Title;
            SelectButtonText = Resources.Strings.SelectSerialPortDialog_Select;
            CancelButtonText = Resources.Strings.ProgressIndicator_CancelButtonText;
        }

        private void PortSelectionCommitted(object sender, System.EventArgs e)
        {
            if (SelectPortCommand.CanExecute(this))
            {
                SelectPortCommand.Execute(this);
            }
        }
    }
}
