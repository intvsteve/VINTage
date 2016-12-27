// <copyright file="SerialPortSelectorController.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.Shared.ViewModel;
using INTV.Shared.Utility;

namespace INTV.Shared.View
{
    /// <summary>
    /// Controller for the <see cref=">SerialPortSelector"/> visual.
    /// </summary>
    public partial class SerialPortSelectorController : NSViewController
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SerialPortSelectorController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SerialPortSelectorController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public SerialPortSelectorController()
            : base("SerialPortSelector", NSBundle.MainBundle)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        /// <param name="viewModel">The ViewModel to use.</param>
        public SerialPortSelectorController(SerialPortSelectorViewModel viewModel)
            : base("SerialPortSelector", NSBundle.MainBundle)
        {
            DataContext = viewModel;
            Prompt = DataContext.Prompt;
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
            _selectedBaudRate = new NSNumber(-1);
        }

        #endregion // Constructors

        #region Events

        /// <summary>
        /// Raised when a port is double-clicked.
        /// </summary>
        public event System.EventHandler SelectionDoubleClicked;

        #endregion // Events

        #region Properties

        /// <summary>
        /// Strongly-typed accessor for the View being controlled.
        /// </summary>
        public new SerialPortSelector View
        {
            get { return (SerialPortSelector)base.View; }
        }

        /// <summary>
        /// Gets the prompt.
        /// </summary>
        [INTV.Shared.Utility.OSExport("Prompt")]
        public string Prompt { get; private set; }

        /// <summary>
        /// Gets or sets the selected baud rate.
        /// </summary>
        [INTV.Shared.Utility.OSExport("SelectedBaudRate")]
        public NSNumber SelectedBaudRate
        {
            get
            {
                if (!_initializedBaudRates && (DataContext.DefaultBaudRate > 0))
                {
                    var selection = DataContext.BaudRates.First(b => b.BaudRate == DataContext.DefaultBaudRate);
                    var selectionIndex = DataContext.BaudRates.IndexOf(selection);
                    if (selectionIndex < BaudRatesArrayController.ArrangedObjects().Length)
                    {
                        _selectedBaudRate = selectionIndex;
                    }
                }
                return _selectedBaudRate;
            }

            set
            {
                _selectedBaudRate = value;
                DataContext.SelectedBaudRate = DataContext.BaudRates[_selectedBaudRate.Int32Value].BaudRate;
            }
        }
        private NSNumber _selectedBaudRate;

        private bool _initializedBaudRates;

        /// <summary>
        /// Gets or sets the index of the selected port.
        /// </summary>
        [INTV.Shared.Utility.OSExport("SelectedPortIndex")]
        public NSIndexSet SelectedPortIndex
        {
            get
            {
                if (!_initializedPortsList && !string.IsNullOrWhiteSpace(DataContext.SelectedSerialPort))
                {
                    var selection = DataContext.AvailableSerialPorts.FirstOrDefault(p => p.PortName == DataContext.SelectedSerialPort);
                    if (selection != null)
                    {
                        var selectionIndex = DataContext.AvailableSerialPorts.IndexOf(selection);
                        if ((selectionIndex >= 0) && (selectionIndex < SerialPortsListArrayController.ArrangedObjects().Length))
                        {
                            _selectedPort = new NSIndexSet((uint)selectionIndex);
                            SerialPortsListArrayController.SelectionIndex = selectionIndex;
                        }
                    }
                }
                return _selectedPort;
            }

            set
            {
                _selectedPort = value;
                if (_selectedPort.Count > 0)
                {
                    DataContext.SelectedSerialPort = DataContext.AvailableSerialPorts[(int)_selectedPort.FirstIndex].PortName;
                }
                else
                {
                    DataContext.SelectedSerialPort = null;
                }
            }
        }
        private NSIndexSet _selectedPort;

        private bool _initializedPortsList;

        private SerialPortSelectorViewModel DataContext { get; set; }

        #endregion // Properties

        ///<inheritdoc/>
        public override void AwakeFromNib()
        {
            this.SerialPortsListArrayController.SynchronizeCollection(DataContext.AvailableSerialPorts);
            this.SerialPortsTableView.Delegate = new SerialPortsListTableViewDelegate(DataContext, SerialPortsListArrayController);
            _initializedPortsList = true;
            this.BaudRatesArrayController.SynchronizeCollection(DataContext.BaudRates);
            _initializedBaudRates = true;
            if (DataContext.BaudRates.Count == 1)
            {
                this.BaudRateLabel.Enabled = false;
                this.BaudRatePopUpButton.Enabled = false;
            }
            if (DataContext.SelectedSerialPort != null)
            {
                var selectedPort = DataContext.AvailableSerialPorts.FirstOrDefault(p => p.PortName == DataContext.SelectedSerialPort);
                if (selectedPort != null)
                {
                    var index = DataContext.AvailableSerialPorts.IndexOf(selectedPort);
                    if (index >= 0)
                    {
                        this.SerialPortsListArrayController.SelectionIndex = index;
                    }
                }
            }
            if (DataContext.DefaultBaudRate > 0)
            {
                var selection = DataContext.BaudRates.First(b => b.BaudRate == DataContext.DefaultBaudRate);
                if (selection != null)
                {
                    var selectedIndex = DataContext.BaudRates.IndexOf(selection);
                    if (selectedIndex >= 0)
                    {
                        BaudRatesArrayController.SelectionIndex = selectedIndex;
                    }
                }
            }
            DataContext.AvailableSerialPorts.CollectionChanged += AvalailablePortsChanged;
            DataContext.DisabledSerialPorts.CollectionChanged += DisabledPortsChanged;
        }

        private void HandleWillDisplayCell (object sender, NSTableViewCellEventArgs e)
        {
            var cell = e.Cell as NSTextFieldCell;
            if (cell != null)
            {
                if (DataContext.DisabledSerialPorts.Contains(cell.StringValue))
                {
                    cell.Enabled = false;
                    cell.TextColor = NSColor.DisabledControlText;
                }
            }
        }

        [Export("SerialPortDoubleClick:")]
        private void HandlePortDoubleClicked (NSArray doubleClickData)
        {
            if (doubleClickData.Count > 0)
            {
                var raiseEvent = true;
                foreach (var port in NSArray.FromArray<SerialPortViewModel>(doubleClickData))
                {
                    raiseEvent = !DataContext.DisabledSerialPorts.Contains(port.PortName);
                    if (!raiseEvent)
                        break;
                }
                if (raiseEvent)
                {
                    var doubleClick = SelectionDoubleClicked;
                    if (doubleClick != null)
                    {
                        doubleClick(this, System.EventArgs.Empty);
                    }
                }
            }
        }

        private  NSCell GetCell(NSTableView tableView, NSTableColumn tableColumn, int row)
        {
            var cell = tableColumn.DataCell as NSTextFieldCell;
            if (cell != null)
            {
                if (DataContext.DisabledSerialPorts.Contains(cell.StringValue))
                {
                    cell.Enabled = false;
                }
            }
            return cell;
        }

        private  NSIndexSet GetSelectionIndexesFilter(NSTableView tableView, NSIndexSet proposedSelectionIndexes)
        {
            var selectionIndexes = new NSMutableIndexSet(proposedSelectionIndexes);
            foreach (var index in proposedSelectionIndexes)
            {
                if (index < DataContext.AvailableSerialPorts.Count())
                {
                    var port = DataContext.AvailableSerialPorts[(int)index];
                    if (DataContext.DisabledSerialPorts.Contains(port.PortName))
                    {
                        selectionIndexes.Remove(index);
                    }
                }
            }
            return selectionIndexes;
        }

        private bool SelectionShouldChangePredicate(NSTableView tableView)
        {
            return true;
        }

        private bool ShouldSelectRowPredicate(NSTableView tableView, int row)
        {
            return true;
        }

        private void AvalailablePortsChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SerialPortsListArrayController.SynchronizeCollection<SerialPortViewModel>(e);
        }

        private void DisabledPortsChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }

        ///<summary>
        /// This is necessary to work around a bug in the delegate implementation from Xamarin.
        /// See: https://bugzilla.xamarin.com/show_bug.cgi?id=12467
        /// </summary>
        private class SerialPortsListTableViewDelegate : NSTableViewDelegate
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="INTV.Shared.View.SerialPortSelectorController+SerialPortsListTableViewDelegate"/> class.
            /// </summary>
            /// <param name="dataContext">The data context.</param>
            /// <param name="ports">The ports that can be selected.</param>
            internal SerialPortsListTableViewDelegate(SerialPortSelectorViewModel dataContext, NSArrayController ports)
            {
                DataContext = dataContext;
                Ports = ports;
            }

            /// <summary>
            /// Gets the data context.
            /// </summary>
            internal SerialPortSelectorViewModel DataContext { get; private set; }

            ///<inheritdoc/>
            public override NSCell GetDataCell(NSTableView tableView, NSTableColumn tableColumn, int row)
            {
                NSTextFieldCell cell = null;
                if (tableColumn != null)
                {
                    cell = tableColumn.DataCellForRow(row) as NSTextFieldCell;
                    var element = DataContext.AvailableSerialPorts[row];
                    if (DataContext.DisabledSerialPorts.Contains(element.PortName))
                    {
                        cell.Enabled = false;
                        cell.TextColor = NSColor.DisabledControlText;
                    }
                    else
                    {
                        cell.Enabled = true;
                        cell.TextColor = NSColor.ControlText;
                    }
                }
                return cell;
            }

            ///<inheritdoc/>
            public override void WillDisplayCell(NSTableView tableView, NSObject cell, NSTableColumn tableColumn, int row)
            {
                var textCell = cell as NSTextFieldCell;
                if (textCell != null)
                {
                }
            }

            ///<inheritdoc/>
            public override NSIndexSet GetSelectionIndexes(NSTableView tableView, NSIndexSet proposedSelectionIndexes)
            {
                var selectionIndexes = new NSMutableIndexSet(proposedSelectionIndexes);
                if (proposedSelectionIndexes.Count > 0)
                {
                    foreach (var index in proposedSelectionIndexes)
                    {
                        if ((int)index < DataContext.AvailableSerialPorts.Count())
                        {
                            var port = DataContext.AvailableSerialPorts[(int)index];
                            if (DataContext.DisabledSerialPorts.Contains(port.PortName))
                            {
                                selectionIndexes.Remove(index);
                            }
                        }
                    }
                }
                return selectionIndexes;
            }

            private NSArrayController Ports { get; set; }

            ///<inheritdoc/>
            public override void SelectionDidChange (NSNotification notification)
            {
            }
        }
    }
}
