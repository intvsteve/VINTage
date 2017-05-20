// <copyright file="SerialPortSelectorDialog.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Device;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Shared implementation for SerialPortSelectorDialog.
    /// </summary>
    public partial class SerialPortSelectorDialog
    {
        /// <summary>
        /// Gets the serial port selected when the dialog was exited.
        /// </summary>
        public string SelectedPort
        {
            get { return ViewModel.SelectedSerialPort; }
        }

        /// <summary>
        /// Gets the baud rate selected when the dialog was exited.
        /// </summary>
        public int SelectedBaudRate
        {
            get { return ViewModel.SelectedBaudRate; }
        }

        /// <summary>
        /// Creates an instance of SerialPortSelectorDialog with given parameters.
        /// </summary>
        /// <param name="title">Title of the dialog.</param>
        /// <param name="message">Prompt message for the dialog (e.g. Select port for peripheral)</param>
        /// <param name="ports">The ports to select from.</param>
        /// <param name="disabledPorts">Ports in the list to disable.</param>
        /// <param name="baudRate">The default baud rate.</param>
        /// <returns>A new instance of the dialog.</returns>
        /// <param name="inclusionFilter">If <c>null</c> or it returns <c>true</c>, ports are included.</param>
        public static SerialPortSelectorDialog Create(string title, string message, IEnumerable<string> ports, IEnumerable<string> disabledPorts, int baudRate, System.Predicate<IConnection> inclusionFilter)
        {
            return Create(title, message, ports, disabledPorts, null, null, baudRate, false, inclusionFilter);
        }

        /// <summary>
        /// Creates an instance of SerialPortSelectorDialog with given parameters.
        /// </summary>
        /// <param name="title">Title of the dialog.</param>
        /// <param name="message">Prompt message for the dialog (e.g. Select port for peripheral)</param>
        /// <param name="selectedPort">The currently selected port.</param>
        /// <param name="baudRates">The supported baud rates.</param>
        /// <param name="defaultBaudRate">The default baud rate.</param>
        /// <returns>A new instance of the dialog.</returns>
        /// <param name="inclusionFilter">If <c>null</c> or it returns <c>true</c>, ports are included.</param>
        public static SerialPortSelectorDialog Create(string title, string message, string selectedPort, IEnumerable<int> baudRates, int defaultBaudRate, System.Predicate<IConnection> inclusionFilter)
        {
            return Create(title, message, null, null, selectedPort, baudRates, defaultBaudRate, false, inclusionFilter);
        }

        /// <summary>
        /// Creates an instance of SerialPortSelectorDialog with given parameters.
        /// </summary>
        /// <param name="title">Title of the dialog.</param>
        /// <param name="message">Prompt message for the dialog (e.g. Select port for peripheral)</param>
        /// <param name="ports">The ports to select from.</param>
        /// <param name="disabledPorts">Ports in the list to disable.</param>
        /// <param name="selectedPort">The currently selected port.</param>
        /// <param name="baudRates">The supported baud rates.</param>
        /// <param name="defaultBaudRate">The default baud rate.</param>
        /// <param name="checkPortAvailability">If <c>true</c>, check the port to see if it is already in use before adding it to the selection list.</param>
        /// <returns>A new instance of the dialog.</returns>
        /// <param name="inclusionFilter">If <c>null</c> or it returns <c>true</c>, ports are included.</param>
        public static SerialPortSelectorDialog Create(string title, string message, IEnumerable<string> ports, IEnumerable<string> disabledPorts, string selectedPort, IEnumerable<int> baudRates, int defaultBaudRate, bool checkPortAvailability, System.Predicate<IConnection> inclusionFilter)
        {
            var selectPortViewModel = new SerialPortSelectorViewModel(message, ports, disabledPorts, selectedPort, baudRates, defaultBaudRate, checkPortAvailability, inclusionFilter);
            var viewModel = new SerialPortSelectorDialogViewModel(selectPortViewModel);
            if (!string.IsNullOrWhiteSpace(title))
            {
                viewModel.Title = title;
            }
            var dialog = Create(viewModel);
            return dialog;
        }
    }
}
