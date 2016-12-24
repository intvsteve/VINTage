// <copyright file="SerialPortSelectorViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using System.Collections.ObjectModel;
using System.Linq;
using INTV.Shared.ComponentModel;
using INTV.Shared.Interop.DeviceManagement;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the SerialPortSelector visual.
    /// </summary>
    public partial class SerialPortSelectorViewModel : ViewModelBase
    {
        #region Property Names

        public const string PromptPropertyName = "Prompt";
        public const string SelectedSerialPortPropertyName = "SelectedSerialPort";
        public const string AllowMultipleSelectionPropertyName = "AllowMultipleSelection";
        public const string SelectedBaudRatePropertyName = "SelectedBaudRate";

        #endregion // Property names

        #region Visual Element Text Resources

        public static readonly string BaudRateLabel = Resources.Strings.BaudRateLabel;
        public static readonly string PortColumnTitle = Resources.Strings.PortColumnTitle;

        #endregion // Visual Element Text Resources

        #region PortDoubleClickedCommand

        /// <summary>
        /// Command to execute when a port in the visual is double-clicked.
        /// </summary>
        public static readonly RelayCommand PortDoubleClickedCommand = new RelayCommand(OnPortDoubleClicked)
        {
            UniqueId = "INTV.Shared.ViewModel.SerialPortSelectorViewModel.PortDoubleClickedCommand",
            BlockWhenAppIsBusy = false
        };

        private static void OnPortDoubleClicked(object parameter)
        {
            var viewModel = parameter as SerialPortSelectorViewModel;
            var portSelectionCommitted = viewModel.PortSelectionCommitted;
            if (portSelectionCommitted != null)
            {
                portSelectionCommitted(viewModel, EventArgs.Empty);
            }
        }

        #endregion // PortDoubleClickedCommand

        #region Constructors

        /// <summary>
        /// Initializes a new instances of SerialPortSelectorViewModel.
        /// </summary>
        public SerialPortSelectorViewModel()
            : this(Resources.Strings.SerialPortSelector_Prompt, null, null, null, null, 9600, false)
        {
        }

        /// <summary>
        /// Initializes a new instances of SerialPortSelectorViewModel.
        /// </summary>
        /// <param name="prompt">The prompt to display in the port selection area.</param>
        /// <param name="availablePorts">The available ports.</param>
        /// <param name="disabledPorts">Non-selectable ports.</param>
        /// <param name="selectedSerialPort">The currently selected port.</param>
        /// <param name="baudRates">Available baud rates to choose from.</param>
        /// <param name="defaultBaudRate">The default baud rate.</param>
        /// <param name="checkPortAvailability">If <c>true</c>, check the port to see if it is already in use before adding it to the selection list.</param>
        public SerialPortSelectorViewModel(string prompt, IEnumerable<string> availablePorts, IEnumerable<string> disabledPorts, string selectedSerialPort, IEnumerable<int> baudRates, int defaultBaudRate, bool checkPortAvailability)
        {
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                Prompt = prompt;
            }
            else
            {
                Prompt = Resources.Strings.SelectSerialPortDialog_Message;
            }
            if (baudRates == null)
            {
                BaudRates = new ObservableCollection<BaudRateViewModel>(new[] { new BaudRateViewModel(defaultBaudRate) });
            }
            else
            {
                BaudRates = new ObservableCollection<BaudRateViewModel>(baudRates.Select(r => new BaudRateViewModel(r)));
            }
            var ports = availablePorts;
            if ((ports == null) || !ports.Any())
            {
                ports = INTV.Shared.Model.Device.SerialPortConnection.AvailablePorts;
            }
            if (ports != null)
            {
                AvailableSerialPorts = new ObservableCollection<SerialPortViewModel>(ports.OrderBy(p => p).Select(p => new SerialPortViewModel(p)));
            }
            else
            {
                AvailableSerialPorts = new ObservableCollection<SerialPortViewModel>();
            }
            if (disabledPorts == null)
            {
                DisabledSerialPorts = new ObservableCollection<string>();
            }
            else
            {
                DisabledSerialPorts = new ObservableCollection<string>(disabledPorts);
            }
            if (checkPortAvailability)
            {
                var portsInUse = INTV.Shared.Model.Device.SerialPortConnection.PortsInUse;
                foreach (var portInUse in portsInUse)
                {
                    DisabledSerialPorts.Add(portInUse);
                }
            }
            foreach (var disabledPort in DisabledSerialPorts.OrderBy(p => p).Reverse())
            {
                var viewModel = AvailableSerialPorts.FirstOrDefault(p => p.PortName == disabledPort);
                if (viewModel == null)
                {
                    viewModel = new SerialPortViewModel(disabledPort, false);
                }
                else
                {
                    viewModel.IsSelectable = false;
                    AvailableSerialPorts.Remove(viewModel);
                }
                if (AvailableSerialPorts.Count == 0)
                {
                    AvailableSerialPorts.Add(viewModel);
                }
                else
                {
                    AvailableSerialPorts.Insert(0, viewModel);
                }
            }
            _selectedSerialPort = selectedSerialPort;
            _selectedSerialPortViewModel = AvailableSerialPorts.FirstOrDefault(p => p.PortName == _selectedSerialPort);
            DefaultBaudRate = defaultBaudRate;
            _selectedBaudRate = defaultBaudRate;
            _selectedBaudRateViewModel = BaudRates.FirstOrDefault(b => b.BaudRate == defaultBaudRate);
            INTV.Shared.Interop.DeviceManagement.DeviceChange.DeviceAdded += DeviceAdded;
            INTV.Shared.Interop.DeviceManagement.DeviceChange.DeviceRemoved += DeviceRemoved;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the prompt text.
        /// </summary>
        public string Prompt
        {
            get { return _prompt; }
            set { AssignAndUpdateProperty(PromptPropertyName, value, ref _prompt); }
        }
        private string _prompt;

        /// <summary>
        /// Gets the ports to display in the selection dialog.
        /// </summary>
        public ObservableCollection<SerialPortViewModel> AvailableSerialPorts { get; private set; }

        /// <summary>
        /// Gets the disabled ports.
        /// </summary>
        public ObservableCollection<string> DisabledSerialPorts { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple selection is supported.
        /// </summary>
        public bool AllowMultipleSelection
        {
            get { return _allowMultipleSelection; }
            set { AssignAndUpdateProperty(AllowMultipleSelectionPropertyName, value, ref _allowMultipleSelection); }
        }
        private bool _allowMultipleSelection;

        /// <summary>
        /// Gets or sets the selected serial port.
        /// </summary>
        [INTV.Shared.Utility.OSExport(SelectedSerialPortPropertyName)]
        public string SelectedSerialPort
        {
            get { return _selectedSerialPort; }
            set { AssignAndUpdateProperty(SelectedSerialPortPropertyName, value, ref _selectedSerialPort); }
        }
        private string _selectedSerialPort;

        /// <summary>
        /// Gets or sets the selected baud rate's ViewModel.
        /// </summary>
        /// <remarks>In WPF, weird problems arise with RibbonComboBox unless the ItemsSource and SelectedValue are the same type.
        /// The simple binding for displayed value does not work without adding a bunch of templating in the XAML, which is a royal PITA.
        /// Perhaps writing an IValueConverter would be better.</remarks>
        public SerialPortViewModel SelectedSerialPortViewModel
        {
            get { return _selectedSerialPortViewModel; }
            set { AssignAndUpdateProperty("SelectedSerialPortViewModel", value, ref _selectedSerialPortViewModel, (p, v) => SelectedSerialPort = (v == null) ? null : v.PortName); }
        }
        private SerialPortViewModel _selectedSerialPortViewModel;

        /// <summary>
        /// Gets the set of supported baud rates.
        /// </summary>
        public ObservableCollection<BaudRateViewModel> BaudRates { get; private set; }

        /// <summary>
        /// Gets or sets the selected baud rate.
        /// </summary>
        [INTV.Shared.Utility.OSExport("SelectedBaudRate")]
        public int SelectedBaudRate
        {
            get { return _selectedBaudRate; }
            set { AssignAndUpdateProperty(SelectedBaudRatePropertyName, value, ref _selectedBaudRate); }
        }
        private int _selectedBaudRate;

        /// <summary>
        /// Gets or sets the selected baud rate's ViewModel.
        /// </summary>
        /// <remarks>In WPF, weird problems arise with RibbonComboBox unless the ItemsSource and SelectedValue are the same type.
        /// Just using the raw int enumerable and baud rate works, but you get weird validation errors (red box in dropdown).
        /// Perhaps writing an IValueConverter would be better.</remarks>
        public BaudRateViewModel SelectedBaudRateViewModel
        {
            get { return _selectedBaudRateViewModel; }
            set { AssignAndUpdateProperty("SelectedBaudRateViewModel", value, ref _selectedBaudRateViewModel, (p, v) => SelectedBaudRate = v.BaudRate); }
        }
        private BaudRateViewModel _selectedBaudRateViewModel;

        /// <summary>
        /// Gets or sets the default baud rate.
        /// </summary>
        public int DefaultBaudRate { get; protected set; }

        #endregion // Properties

        #region Events

        /// <summary>
        /// This event is raised to inform interested parties that a serial port has been selected via the selector visual, usually via double-click.
        /// </summary>
        internal event EventHandler PortSelectionCommitted;

        #endregion // Events

        private void DeviceAdded(object sender, INTV.Shared.Interop.DeviceManagement.DeviceChangeEventArgs e)
        {
            if ((e.Type == INTV.Core.Model.Device.ConnectionType.Serial) && DeviceChange.IsDeviceChangeFromSystem(e.State))
            {
                if (!AvailableSerialPorts.Any(p => p.PortName == e.Name))
                {
                    AvailableSerialPorts.Add(new SerialPortViewModel(e.Name));
                }
                INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
            }
        }

        private void DeviceRemoved(object sender, INTV.Shared.Interop.DeviceManagement.DeviceChangeEventArgs e)
        {
            if ((e.Type == INTV.Core.Model.Device.ConnectionType.Serial) && DeviceChange.IsDeviceChangeFromSystem(e.State))
            {
                var removedDevice = AvailableSerialPorts.FirstOrDefault(p => p.PortName == e.Name);
                if ((removedDevice != null) && AvailableSerialPorts.Remove(removedDevice))
                {
                    INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
                }
            }
        }
    }
}
