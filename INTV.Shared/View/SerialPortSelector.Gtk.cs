// <copyright file="SerialPortSelector.Gtk.cs" company="INTV Funhouse">
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
    [System.ComponentModel.ToolboxItem(true)]
    public partial class SerialPortSelector : Gtk.Bin, IFakeDependencyObject
    {
        private int _promptAreaWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.SerialPortSelector"/> class.
        /// </summary>
        public SerialPortSelector()
        {
            this.Build();
            _prompt.SizeAllocated += HandleSizeAllocated;
        }

        #region IFakeDependencyObject

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

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
        /// Initialize the visual's ViewModel (DataContext).
        /// </summary>
        /// <param name="viewModel">The data context for the visual.</param>
        internal void Initialize(SerialPortSelectorViewModel viewModel)
        {
            DataContext = viewModel;
            // TODO: CollectionChanged handler -- see Synchronize calls
            // TODO: PropertyChanged handler
            // TODO: Handle item disabled / enabled in list -- see ShouldSelect
            _prompt.Text = viewModel.Prompt;
            InitializePortsList(_ports);
            InitializeBaudRates(_baudRates);
        }

        private void InitializePortsList(Gtk.TreeView portsList)
        {
            var viewModel = DataContext as SerialPortSelectorViewModel;

            var column = new Gtk.TreeViewColumn() { Title = SerialPortSelectorViewModel.PortColumnTitle };
            var cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l,c,m,i) => VisualHelpers.CellTextColumnRenderer<SerialPortViewModel>(l,c,m,i, p => p.PortName));
//            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
//            column.FixedWidth = Properties.Settings.Default.MenuLayoutLongNameColWidth;
//            column.Resizable = true;
            portsList.AppendColumn(column);

            portsList.Selection.Changed += HandleSelectedPortChanged;
            portsList.Selection.SelectFunction = ShouldSelectPort;

            var serialPortsModel = new Gtk.ListStore(typeof(SerialPortViewModel));
            serialPortsModel.SynchronizeCollection(viewModel.AvailableSerialPorts);
            // TODO: Disable ports appropriately -- see ShouldSelect
            // TODO: Customize renderer somehow for disabled items to draw differently --
            // Tinkering with CellRendererText.Foreground does not work as desired.
            portsList.Model = serialPortsModel;

            viewModel.AvailableSerialPorts.CollectionChanged += HandleAvailablePortsChanged;
        }

        private void InitializeBaudRates(Gtk.ComboBox baudRates)
        {
            var viewModel = DataContext as SerialPortSelectorViewModel;

            _baudRateLabel.Text = SerialPortSelectorViewModel.BaudRateLabel;

            Gtk.CellRenderer cellRenderer = new Gtk.CellRendererCombo(); // { Xalign = 0, Xpad = 4 };
            baudRates.PackStart(cellRenderer, false);
            //            baudRates.PackEnd(cellRenderer, true);
            baudRates.SetCellDataFunc(cellRenderer, (l,e,m,i) => VisualHelpers.CellTextRenderer<BaudRateViewModel>(l,e,m,i, c => c.BaudRate.ToString()));

            var baudRatesModel = new Gtk.ListStore(typeof(BaudRateViewModel));
            baudRatesModel.SynchronizeCollection(viewModel.BaudRates);
            baudRates.Model = baudRatesModel;

            if (viewModel.DefaultBaudRate > 0)
            {
                var defaultBaudRate = viewModel.BaudRates.FirstOrDefault(b => b.BaudRate == viewModel.DefaultBaudRate);
                var activeItemIndex = viewModel.BaudRates.IndexOf(defaultBaudRate);
                baudRates.Active = activeItemIndex;
            }

            baudRates.Changed += HandleSelectedBaudRateChanged;
            INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
        }

        private void HandleSizeAllocated(object o, Gtk.SizeAllocatedArgs args)
        {
            var prompt = o as Gtk.Label;
            var newWidth = args.Allocation.Size.Width;
            if (newWidth != _promptAreaWidth)
            {
                // Totally arbitrary fudge. Without it, you can't make the window narrower.
                prompt.SetSizeRequest(newWidth - 8, -1);
            }
            _promptAreaWidth = newWidth;
        }

        private bool ShouldSelectPort(Gtk.TreeSelection selection, Gtk.TreeModel model, Gtk.TreePath path, bool path_currently_selected)
        {
            var shouldSelect = true;
            Gtk.TreeIter iter;
            if (selection.GetSelected(out iter))
            {
                var viewModel = DataContext as SerialPortSelectorViewModel;
                var item = model.GetValue(iter, 0) as SerialPortViewModel;
                shouldSelect = (item != null) && !viewModel.DisabledSerialPorts.Contains(item.PortName);
            }
            return shouldSelect;
        }

        private void HandleAvailablePortsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _ports.Model.SynchronizeCollection<SerialPortViewModel>(e);
        }

        private void HandleSelectedPortChanged(object sender, System.EventArgs e)
        {
            var viewModel = DataContext as SerialPortSelectorViewModel;
            var viewModelSelection = viewModel.SelectedSerialPortViewModel;
            var selection = sender as Gtk.TreeSelection;
            var selectedItemPaths = selection.GetSelectedRows();
            SerialPortViewModel newSelectedItem = null;
            foreach (var path in selectedItemPaths)
            {
                Gtk.TreeIter iter;
                if (selection.TreeView.Model.GetIter(out iter, path))
                {
                    var item = selection.TreeView.Model.GetValue(iter, 0) as SerialPortViewModel;
                    newSelectedItem = item;
                    if (newSelectedItem != null)
                    {
                        break;
                    }
                }
            }
            viewModel.SelectedSerialPortViewModel = newSelectedItem;
            INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
        }

        private void HandleSelectedBaudRateChanged(object sender, System.EventArgs e)
        {
            var baudRates = sender as Gtk.ComboBox;
            var viewModel = baudRates.GetValue(IFakeDependencyObjectHelpers.DataContextPropertyName) as SerialPortSelectorViewModel;
            var currentBaudRateIndex = viewModel.BaudRates.IndexOf(viewModel.SelectedBaudRateViewModel);
            if ((baudRates.Active != currentBaudRateIndex) && (baudRates.Active >= 0))
            {
                var newBaudRate = viewModel.BaudRates[baudRates.Active];
                viewModel.SelectedBaudRateViewModel = newBaudRate;
            }
        }
    }
}
