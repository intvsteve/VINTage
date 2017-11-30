// <copyright file="MenuLayoutView.Gtk.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using INTV.Core.Model.Stic;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.LtoFlash.Model;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    [Gtk.Binding(Gdk.Key.Delete, "HandleDeleteSelectedItems")]
    [Gtk.Binding(Gdk.Key.BackSpace, "HandleDeleteSelectedItems")]
    public partial class MenuLayoutView : Gtk.Bin, IFakeDependencyObject
    {
        private static readonly OSImage _powerIconImage = typeof(INTV.Shared.Utility.ResourceHelpers).LoadImageResource("ViewModel/Resources/Images/console_16xLG.png");
        private static readonly OSImage _dirtyIconImage = typeof(MenuLayoutView).LoadImageResource("Resources/Images/lto_flash_contents_not_in_sync_16xLG.png");
        private static readonly Dictionary<Color, Gdk.Pixbuf> _colorPixbufs = new Dictionary<Color, Gdk.Pixbuf>();
        ////private static readonly Dictionary<string, Gdk.Pixbuf> _deleteButtonIcons = new Dictionary<string, Gdk.Pixbuf>();

        private DeviceViewModel _activeDevice;
        private TextCellInPlaceEditor _deviceNameEditor;
        private TextCellInPlaceEditor _longNameEditor;
        private TextCellInPlaceEditor _shortNameEditor;

        internal MenuLayoutView(LtoFlashViewModel viewModel)
        {
            // TODO: Show/Hide power icon based on state
            // TODO: Show/Hide dirty icon based on state
            DataContext = viewModel;
            this.Build();

            _menuLayoutTitle.Text = MenuLayoutViewModel.Title;

            _deviceName.CanDefault = false;
            _deviceNameEditor = new TextCellInPlaceEditor(_deviceName, FileSystemConstants.MaxShortNameLength) { IsValidCharacter = INTV.Core.Model.Grom.Characters.Contains };

            _dirtyIcon.NoShowAll = true;
            _dirtyIcon.Visible = viewModel.ShowFileSystemsDifferIcon;
            _dirtyIcon.Pixbuf = _dirtyIconImage;
            _dirtyIcon.TooltipText = LtoFlashViewModel.ContentsNotInSyncToolTip;
            _powerIcon.Pixbuf = _powerIconImage.CreateNewWithOpacity(0.5); // initialize to the "power off" image
            _powerIcon.TooltipText = Resources.Strings.ConsolePowerState_Unknown;

            ((Gtk.Image)_newFolder.Image).Pixbuf = MenuLayoutCommandGroup.NewDirectoryCommand.SmallIcon;
            _newFolder.TooltipText = MenuLayoutViewModel.NewFolderTip;

            ((Gtk.Image)_deleteSelectedItems.Image).Pixbuf = MenuLayoutCommandGroup.DeleteItemsCommand.SmallIcon;

            InitializeColorComboBox(_colorChooser, viewModel.HostPCMenuLayout.AvailableColors, viewModel.HostPCMenuLayout);
            InitializeMenuLayout(_menuLayout, viewModel.HostPCMenuLayout);

            // Cheesy way to initialize some properties
            var initializePropertyNames = new[]
                {
                    MenuLayoutViewModel.StatusPropertyName,
                    MenuLayoutViewModel.CurrentSelectionPropertyName,
                    "DeleteSelectedItemTip",
                    MenuLayoutViewModel.OverallUsageDetailsPropertyName,
                };
            foreach (var propertyName in initializePropertyNames)
            {
                HandleMenuLayoutPropertyChanged(viewModel.HostPCMenuLayout, new PropertyChangedEventArgs(propertyName));
            }

            MenuLayoutCommandGroup.NewDirectoryCommand.CanExecuteChanged += HandleCanExecuteCreateNewDirectoryChanged;
            MenuLayoutCommandGroup.DeleteItemsCommand.CanExecuteChanged += HandleCanExecuteDeleteItemsChanged;

            _activeDevice = viewModel.ActiveLtoFlashDevice;

            initializePropertyNames = new[]
                {
                    LtoFlashViewModel.ActiveLtoFlashDevicePropertyName,
                    LtoFlashViewModel.ShowFileSystemsDifferIconPropertyName,
                };
            foreach (var propertyName in initializePropertyNames)
            {
                HandleLtoFlashPropertyChanged(viewModel, new PropertyChangedEventArgs(propertyName));
            }

            viewModel.PropertyChanged += HandleLtoFlashPropertyChanged;
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
        /// Begin editing the long name of the currently selected item.
        /// </summary>
        /// <remarks>When multiple items are selected, the first item in the selection list will be edited.</remarks>
        internal void EditLongName()
        {
            Gtk.TreeIter iter;
            if (_menuLayout.Selection.GetSelected(out iter))
            {
                var path = _menuLayout.Model.GetPath(iter);
                var column = _menuLayout.Columns[(int)MenuLayoutColumn.LongName];
                _longNameEditor.EditingObject = new TextCellInPlaceEditorObjectData(path, column);
                _longNameEditor.BeginEdit();
            }
        }

        /// <summary>
        /// Begin editing the short name of the currently selected item.
        /// </summary>
        /// <remarks>When multiple items are selected, the first item in the selection list will be edited.</remarks>
        internal void EditShortName()
        {
            Gtk.TreeIter iter;
            if (_menuLayout.Selection.GetSelected(out iter))
            {
                var path = _menuLayout.Model.GetPath(iter);
                var column = _menuLayout.Columns[(int)MenuLayoutColumn.ShortName];
                _shortNameEditor.EditingObject = new TextCellInPlaceEditorObjectData(path, column);
                _shortNameEditor.BeginEdit();
            }
        }

        /// <summary>
        /// Handles the new folder button being clicked.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">Event data.</param>
        protected void HandleNewFolderClicked(object sender, System.EventArgs e)
        {
            var ltoFlashViewModel = DataContext as LtoFlashViewModel;
            MenuLayoutCommandGroup.NewDirectoryCommand.Execute(ltoFlashViewModel.HostPCMenuLayout);
        }

        /// <summary>
        /// Handles the delete selected items button being clicked.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">Event data.</param>
        protected void HandleDeleteSelectedItemsClicked(object sender, System.EventArgs e)
        {
            var viewModel = _menuLayout.GetValue(IFakeDependencyObjectHelpers.DataContextPropertyName) as MenuLayoutViewModel;
            viewModel.DeleteItems();
        }

        /// <summary>
        /// Handles the row expanded event for the tree.
        /// </summary>
        /// <param name="o">The tree control.</param>
        /// <param name="args">The row that was expanded.</param>
        protected void HandleRowExpanded(object o, Gtk.RowExpandedArgs args)
        {
            var menuTree = o as Gtk.TreeView;
            var folder = menuTree.Model.GetValue(args.Iter, 0) as FolderViewModel;
            folder.IsOpen = true;
        }

        /// <summary>
        /// Handles the row collapsed event for the tree.
        /// </summary>
        /// <param name="o">The tree control.</param>
        /// <param name="args">The row that was collapsed.</param>
        protected void HandleRowCollapsed(object o, Gtk.RowCollapsedArgs args)
        {
            var menuTree = o as Gtk.TreeView;
            var folder = menuTree.Model.GetValue(args.Iter, 0) as FolderViewModel;
            folder.IsOpen = false;
        }

        /// <summary>
        /// Handles the color selected event from the color combo box.
        /// </summary>
        /// <param name="sender">The combo box.</param>
        /// <param name="e">Event data.</param>
        protected void HandleColorSelected(object sender, System.EventArgs e)
        {
            var comboBox = sender as Gtk.ComboBox;
            var viewModel = comboBox.GetValue(IFakeDependencyObjectHelpers.DataContextPropertyName) as MenuLayoutViewModel;
            var currentItemColorIndex = -1;
            if (viewModel.CurrentSelection != null)
            {
                var color = viewModel.CurrentSelection.Color;
                currentItemColorIndex = viewModel.AvailableColors.IndexOf(color);
            }
            if ((comboBox.Active != currentItemColorIndex) && (comboBox.Active >= 0))
            {
                var newColor = viewModel.AvailableColors[comboBox.Active];
                // TODO: Verify behavior for multi-select
                foreach (var file in viewModel.SelectedItems)
                {
                    file.Color = newColor;
                }
                _menuLayout.QueueDraw(); // is there a better way?
            }
        }

        private void HandleCanExecuteDeleteItemsChanged(object sender, System.EventArgs e)
        {
            var ltoFlashViewModel = DataContext as LtoFlashViewModel;
            _deleteSelectedItems.Sensitive = MenuLayoutCommandGroup.DeleteItemsCommand.CanExecute(ltoFlashViewModel.HostPCMenuLayout);
        }

        private void HandleCanExecuteCreateNewDirectoryChanged(object sender, System.EventArgs e)
        {
            var ltoFlashViewModel = DataContext as LtoFlashViewModel;
            _newFolder.Sensitive = MenuLayoutCommandGroup.NewDirectoryCommand.CanExecute(ltoFlashViewModel.HostPCMenuLayout);
        }

        private void HandleDeviceNameTextInserted(object o, Gtk.TextInsertedArgs args)
        {
            var entry = o as Gtk.Entry;
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(entry, _deviceName));
            entry.TextInserted -= HandleDeviceNameTextInserted;
            var a = entry.Action;
            var position = args.Position;
            var currentText = entry.Text;
            foreach (var character in args.Text)
            {
                if (!INTV.Core.Model.Grom.Characters.Contains(character))
                {
                    var index = currentText.IndexOf(character);
                    while (index >= 0)
                    {
                        entry.DeleteText(index,index + 1);
                        args.Position = args.Position - 1;
                        currentText = entry.Text;
                        index = currentText.IndexOf(character);
                    }
                }
            }

            entry.TextInserted += HandleDeviceNameTextInserted;
        }

        private void InitializeColorComboBox(Gtk.ComboBox colorChooser, ObservableCollection<FileNodeColorViewModel> colors, MenuLayoutViewModel dataContext)
        {
            colorChooser.SetValue(IFakeDependencyObjectHelpers.DataContextPropertyName, dataContext);
            colorChooser.TooltipText = MenuLayoutViewModel.ColorTip;
            foreach (var color in colors)
            {
                var pixbufColor = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 14, 14);
                var uintColor = (uint)((color.Fill.Red & 0xFF) << 24) | (uint)((color.Fill.Green & 0xFF) << 16) | (uint)((color.Fill.Blue & 0xFF) << 8) | 0xFF;
                pixbufColor.Fill(uintColor);
                var pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 16, 16);
                pixbuf.Fill(0xFF); // black background
                pixbufColor.CopyArea(0, 0, 14, 14, pixbuf, 1, 1);
                _colorPixbufs[color.IntvColor] = pixbuf;
            }

            Gtk.CellRenderer cellRenderer = new Gtk.CellRendererPixbuf() { Xalign = 0 };
            colorChooser.PackStart(cellRenderer, false);
            colorChooser.SetCellDataFunc(cellRenderer, (l,e,m,i) => VisualHelpers.CellImageRenderer<FileNodeColorViewModel>(l,e,m,i, c => _colorPixbufs[c.IntvColor]));

            cellRenderer = new Gtk.CellRendererCombo() { Xalign = 0, Xpad = 4 };
            colorChooser.PackEnd(cellRenderer, true);
            colorChooser.SetCellDataFunc(cellRenderer, (l,e,m,i) => VisualHelpers.CellTextRenderer<FileNodeColorViewModel>(l,e,m,i, c=> c.Name));

            var colorListStore = new Gtk.ListStore(typeof(FileNodeColorViewModel));
            colorListStore.SynchronizeCollection(colors);
            colorChooser.Model = colorListStore;

            MenuLayoutCommandGroup.SetColorCommand.CanExecuteChanged += HandleCanExecuteSetColorChanged;
        }

        private void HandleCanExecuteSetColorChanged(object sender, System.EventArgs e)
        {
            var dataContext = _colorChooser.GetValue(IFakeDependencyObjectHelpers.DataContextPropertyName);
            var canExecute = MenuLayoutCommandGroup.SetColorCommand.CanExecute(dataContext);
            _colorChooser.Sensitive = canExecute;
        }

        private void InitializeMenuLayout(Gtk.TreeView menuLayout, MenuLayoutViewModel dataContext)
        {
            menuLayout.SetValue(IFakeDependencyObjectHelpers.DataContextPropertyName, dataContext);
            var column = new Gtk.TreeViewColumn();

            Gtk.CellRenderer cellRenderer = new Gtk.CellRendererPixbuf();
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l,c,m,i) => VisualHelpers.CellImageColumnRenderer<FileNodeViewModel>(l,c,m,i, p => p.Icon));
            //column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            //column.FixedWidth = 20;
            menuLayout.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = MenuLayoutViewModel.LongNameHeader };
            cellRenderer = new Gtk.CellRendererText();
            _longNameEditor = new TextCellInPlaceEditor(menuLayout, column, cellRenderer as Gtk.CellRendererText, FileSystemConstants.MaxLongNameLength) { IsValidCharacter = INTV.Core.Model.Grom.Characters.Contains };
            _longNameEditor.EditorClosed += HandleInPlaceEditorClosed;
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l,c,m,i) => VisualHelpers.CellTextColumnRenderer<FileNodeViewModel>(l,c,m,i, p => p.LongName.SafeString()));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.MenuLayoutLongNameColWidth;
            column.Resizable = true;
            menuLayout.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = MenuLayoutViewModel.ShortNameHeader };
            cellRenderer = new Gtk.CellRendererText();
            _shortNameEditor = new TextCellInPlaceEditor(menuLayout, column, cellRenderer as Gtk.CellRendererText, FileSystemConstants.MaxShortNameLength) { IsValidCharacter = INTV.Core.Model.Grom.Characters.Contains };
            _shortNameEditor.EditorClosed += HandleInPlaceEditorClosed;
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l,c,m,i) => VisualHelpers.CellTextColumnRenderer<FileNodeViewModel>(l,c,m,i, p => p.ShortName.SafeString()));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.MenuLayoutShortNameColWidth;
            column.Resizable = true;
            menuLayout.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = MenuLayoutViewModel.ManualHeader };
            cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l,c,m,i) => VisualHelpers.CellTextColumnRenderer<FileNodeViewModel>(l,c,m,i, GetManualColumnStringValue));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.MenuLayoutManualColWidth;
            column.Resizable = true;
            menuLayout.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = MenuLayoutViewModel.SaveDataHeader };
            cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, true);
            //column.SetCellDataFunc(cellRenderer, (l,c,m,i) => VisualHelpers.CellTextColumnRenderer<ProgramDescriptionViewModel>(l,c,m,i, p => p.Name));
            //column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            //column.FixedWidth = Properties.Settings.Default.MenuLayoutSaveDataColWidth;
            column.Resizable = true;
            column.Visible = Properties.Settings.Default.ShowAdvancedFeatures;
            menuLayout.AppendColumn(column);

            menuLayout.HasTooltip = true;
            menuLayout.QueryTooltip += HandleMenuLayoutQueryTooltip;

            var menuLayoutStore = new Gtk.TreeStore(typeof(FileNodeViewModel));
            dataContext.SynchronizeToTreeStore(menuLayoutStore);
            menuLayout.Model = menuLayoutStore;

            menuLayout.Selection.Changed += HandleSelectionChanged;

            dataContext.PropertyChanged += HandleMenuLayoutPropertyChanged;
        }

        private static string GetManualColumnStringValue(FileNodeViewModel node)
        {
            if (node is FolderViewModel)
            {
                return ((FolderViewModel)node).Status;
            }
            else
            {
                return node.Manual;
            }
        }

        private void HandleInPlaceEditorClosed(object sender, INTV.Shared.Behavior.InPlaceEditorClosedEventArgs e)
        {
            if (e.CommitedChanges)
            {
                var state = e.State as TextCellInPlaceEditorObjectData;
                Gtk.TreeIter iter;
                if (_menuLayout.Model.GetIter(out iter, state.Path))
                {
                    var item = _menuLayout.Model.GetValue(iter, 0) as FileNodeViewModel;
                    var column = (MenuLayoutColumn)_menuLayout.Columns.ToList().IndexOf(state.Column);
                    switch (column)
                    {
                        case MenuLayoutColumn.LongName:
                            item.LongName = state.Data as string;
                            break;
                        case MenuLayoutColumn.ShortName:
                            item.ShortName = state.Data as string;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void HandleMenuLayoutQueryTooltip(object o, Gtk.QueryTooltipArgs args)
        {
            var treeView = o as Gtk.TreeView;
            Gtk.TreePath path;
            Gtk.TreeViewColumn column;
            int x, y;
            treeView.ConvertWidgetToBinWindowCoords(args.X, args.Y, out x, out y);
            int cellX, cellY;
            if (treeView.GetPathAtPos(x, y, out path, out column, out cellX, out cellY))
            {
                Gtk.TreeIter iter;
                if (treeView.Model.GetIter(out iter, path))
                {
                    var item = treeView.Model.GetValue(iter, 0) as FileNodeViewModel;
                    var menuLayoutColumn = (MenuLayoutColumn)System.Array.IndexOf(treeView.Columns, column);
                    var tooltip = string.Empty;
                    switch (menuLayoutColumn)
                    {
                        case MenuLayoutColumn.None:
                            tooltip = item.IconTipStrip;
                            break;
                        case MenuLayoutColumn.LongName:
                            tooltip = MenuLayoutViewModel.LongNameTip;
                            break;
                        case MenuLayoutColumn.ShortName:
                            tooltip = MenuLayoutViewModel.ShortNameTip;
                            break;
                        case MenuLayoutColumn.ManualFileOrDirectoryInfo:
                            if (item is ProgramViewModel)
                            {
                                if (string.IsNullOrEmpty(item.Manual))
                                {
                                    tooltip = ProgramViewModel.ManualTip;
                                }
                                else
                                {
                                    tooltip = item.Manual;
                                }
                            }
                            break;
                        case MenuLayoutColumn.FlashDataFile:
                            if (item is ProgramViewModel)
                            {
                                if (string.IsNullOrEmpty(item.SaveData))
                                {
                                    ////tooltip = MenuLayoutCommandGroup.SetSaveDataCommand.ToolTip;
                                }
                                else
                                {
                                    tooltip = item.SaveData;
                                }
                            }
                            break;
                        default:
                            throw new System.InvalidOperationException("Unknown MenuLayoutColumn: " + menuLayoutColumn);
                    }

                    if (!string.IsNullOrEmpty(tooltip))
                    {
                        args.Tooltip.Text = tooltip;
                        args.RetVal = true;
                    }
                }
            }
        }

        private void HandleMenuLayoutPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
                    var menuLayout = sender as MenuLayoutViewModel;
                    switch (e.PropertyName)
                    {
                        case MenuLayoutViewModel.StatusPropertyName:
                            _rootDirectoryUsage.Text = menuLayout.Status;
                            break;
                        case MenuLayoutViewModel.CurrentSelectionPropertyName:
                    // Push from ViewModel's notion of selected item to the tree's selection.
                    // TODO: what happens when multiselect is enabled?
                            Gtk.TreeIter selectionIter;
                            if (_menuLayout.Selection.GetSelected(out selectionIter))
                            {
                                var currentSelectedItem = menuLayout.CurrentSelection;
                                var modelSelection = _menuLayout.Model.GetValue(selectionIter, 0);
                                if (modelSelection != currentSelectedItem)
                                {
                                    Gtk.TreeIter iter;
                                    if (currentSelectedItem.GetIterForItem(out iter, _menuLayout.Model as Gtk.TreeStore))
                                    {
                                        _menuLayout.Selection.SelectIter(iter);
                                    }
                                }
                            }
                            else if (menuLayout.CurrentSelection != null)
                            {
                                // Don't have a selection, so find and select item from ViewModel.
                                var currentSelectedItem = menuLayout.CurrentSelection;
                                Gtk.TreeIter iter;
                                if (currentSelectedItem.GetIterForItem(out iter, _menuLayout.Model as Gtk.TreeStore))
                                {
                                    _menuLayout.Selection.SelectIter(iter);
                                }
                            }
                            INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
                            break;
                        case "DeleteSelectedItemTip":
                            _deleteSelectedItems.TooltipText = menuLayout.DeleteSelectedItemTip;
                            break;
                        case MenuLayoutViewModel.OverallUsageDetailsPropertyName:
                            _storageUsed.Fraction = menuLayout.OverallInUseRatio;
                            _storageUsed.Text = string.Format("{0:P2}", _storageUsed.Fraction);
                            _storageUsed.TooltipText = menuLayout.OverallUsageDetails;
                            break;
                        default:
                            break;
                    }
                });
        }

        private void HandleLtoFlashPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
                    var viewModel = sender as LtoFlashViewModel;
                    switch (e.PropertyName)
                    {
                        case LtoFlashViewModel.ActiveLtoFlashDevicePropertyName:
                            _activeDevice.PropertyChanged -= HandleActiveDevicePropertyChanged;
                            _activeDevice = viewModel.ActiveLtoFlashDevice;
                            _activeDevice.PropertyChanged += HandleActiveDevicePropertyChanged;
                            UpdateActiveDeviceViewModelInfo(viewModel.ActiveLtoFlashDevice);
                            break;
                        case LtoFlashViewModel.ShowFileSystemsDifferIconPropertyName:
                            _dirtyIcon.Visible = viewModel.ShowFileSystemsDifferIcon;
                            break;
                        default:
                            break;
                    }
                });
        }

        private void HandleActiveDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
                    var device = sender as DeviceViewModel;
                    switch (e.PropertyName)
                    {
                        case DeviceViewModel.IsConfigurablePropertyName:
                        case Device.IsValidPropertyName:
                            UpdateActiveDeviceViewModelInfo(device);
                            break;
                        default:
                            break;
                    }
                });
        }

        private void UpdateActiveDeviceViewModelInfo(DeviceViewModel device)
        {
            // TODO: Hook this stuff up correctly for device, etc.
            // Make not editable unless device connected
            var editable = false;
            var deviceNameText = device.Name;
            if (!device.IsValid)
            {
                var ltoFlashViewModel = DataContext as LtoFlashViewModel;
                deviceNameText = ltoFlashViewModel.HostPCMenuLayout.ShortName;
                if (string.IsNullOrEmpty(deviceNameText))
                {
                    deviceNameText = ltoFlashViewModel.HostPCMenuLayout.LongName;
                }
            }
            else
            {
                editable = device.IsConfigurable;
            }
            _deviceName.Text = deviceNameText;
            _deviceName.IsEditable = editable;
            _deviceName.TooltipText = deviceNameText;
        }

        private void HandleDeleteSelectedItems()
        {
            var dataContext = _menuLayout.GetValue(IFakeDependencyObjectHelpers.DataContextPropertyName);
            if (MenuLayoutCommandGroup.DeleteItemsCommand.CanExecute(dataContext))
            {
                MenuLayoutCommandGroup.DeleteItemsCommand.Execute(dataContext);
            }
        }

        private void HandleSelectionChanged(object sender, System.EventArgs e)
        {
            var selection = sender as Gtk.TreeSelection;
            var menuLayoutViewModel = selection.TreeView.GetValue(IFakeDependencyObjectHelpers.DataContextPropertyName) as MenuLayoutViewModel;
            FileNodeViewModel currentSelectedItem = null;
            var viewModelSelection = menuLayoutViewModel.SelectedItems;
            var selectedItems = new List<FileNodeViewModel>();
            var selectedItemPaths = selection.GetSelectedRows();
            foreach (var path in selectedItemPaths)
            {
                Gtk.TreeIter iter;
                if (selection.TreeView.Model.GetIter(out iter, path))
                {
                    var item = selection.TreeView.Model.GetValue(iter, 0) as FileNodeViewModel;
                    currentSelectedItem = currentSelectedItem ?? item;
                    selectedItems.Add(item);
                }
            }
            var itemsToRemove = viewModelSelection.Except(selectedItems).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                viewModelSelection.Remove(itemToRemove);
            }
            foreach (var selectedItem in selectedItems.Except(viewModelSelection).ToList())
            {
                viewModelSelection.Add(selectedItem);
            }
            menuLayoutViewModel.CurrentSelection = currentSelectedItem;
            var activeColor = -1;
            if (currentSelectedItem != null)
            {
                activeColor = menuLayoutViewModel.AvailableColors.IndexOf(currentSelectedItem.Color);
            }
            _colorChooser.Active = activeColor;
        }
    }
}
