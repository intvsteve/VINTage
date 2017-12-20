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

#define ENABLE_DRAGDROP_TRACE

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using INTV.Core.Model.Program;
using INTV.Core.Model.Stic;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
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
        private static readonly Gtk.TargetEntry[] DragDropSourceEntries = new[]
        {
            new Gtk.TargetEntry(MenuLayoutViewModel.DragDataFormat, Gtk.TargetFlags.Widget, MenuLayoutViewModel.DragDataFormatIdentifier),
        };

        private static readonly Gtk.TargetEntry[] DragDropTargetEntries = new[]
        {
            new Gtk.TargetEntry(ProgramDescriptionViewModel.DragDataFormat, Gtk.TargetFlags.App, RomListViewModel.DragDropSourceDataIdentifier),
            new Gtk.TargetEntry(MenuLayoutViewModel.DragDataFormat, Gtk.TargetFlags.Widget, MenuLayoutViewModel.DragDataFormatIdentifier),
        };

        private static readonly Gdk.Pixbuf DragMultipleItemsProgramImage = typeof(MenuLayoutView).LoadImageResource("Resources/Images/document_added_White_16xLG.png");
        private static readonly Gdk.Pixbuf DragMultipleItemsFolderImage = typeof(MenuLayoutView).LoadImageResource("Resources/Images/folder_added_Closed_White_16xLG.png");
        private static readonly OSImage PowerIconImage = typeof(INTV.Shared.Utility.ResourceHelpers).LoadImageResource("ViewModel/Resources/Images/console_16xLG.png");
        private static readonly OSImage DirtyIconImage = typeof(MenuLayoutView).LoadImageResource("Resources/Images/lto_flash_contents_not_in_sync_16xLG.png");
        private static readonly Dictionary<Color, Gdk.Pixbuf> ColorPixbufs = new Dictionary<Color, Gdk.Pixbuf>();
        ////private static readonly Dictionary<string, Gdk.Pixbuf> _deleteButtonIcons = new Dictionary<string, Gdk.Pixbuf>();

        private DeviceViewModel _activeDevice;
        private TextCellInPlaceEditor _longNameEditor;
        private TextCellInPlaceEditor _shortNameEditor;

        internal MenuLayoutView(LtoFlashViewModel viewModel)
        {
            // TODO: Show/Hide power icon based on state
            // TODO: Show/Hide dirty icon based on state
            DataContext = viewModel;
            this.Build();

            _menuLayoutTitle.Text = MenuLayoutViewModel.Title;

            _dirtyIcon.NoShowAll = true;
            _dirtyIcon.Visible = viewModel.ShowFileSystemsDifferIcon;
            _dirtyIcon.Pixbuf = DirtyIconImage;
            _dirtyIcon.TooltipText = LtoFlashViewModel.ContentsNotInSyncToolTip;
            _powerIcon.Pixbuf = PowerIconImage.CreateNewWithOpacity(0.5); // initialize to the "power off" image
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

        #region IFakeDependencyObject Properties

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        private LtoFlashViewModel ViewModel
        {
            get { return DataContext as LtoFlashViewModel; }
        }

        #endregion // IFakeDependencyObject Properties

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
            var ltoFlashViewModel = ViewModel;
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

        /// <summary>
        /// Handles the drag begin event.
        /// </summary>
        /// <param name="o">The menu layout Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragBegin(object o, Gtk.DragBeginArgs args)
        {
            DebugDragDrop("HandleDragBegin");
            var menuLayoutView = o as Gtk.TreeView;
            SetDragImage(menuLayoutView, args);
        }

        /// <summary>
        /// Handles the drag data delete event.
        /// </summary>
        /// <param name="o">The menu layout Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragDataDelete(object o, Gtk.DragDataDeleteArgs args)
        {
            DebugDragDrop("HandleDragDataDelete");
        }

        /// <summary>
        /// Handles the drag data get event.
        /// </summary>
        /// <param name="o">The menu layout Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragDataGet(object o, Gtk.DragDataGetArgs args)
        {
            DebugDragDrop("HandleDragDataGet");
            if (args.Info == MenuLayoutViewModel.DragDataFormatIdentifier)
            {
                var menuLayout = o as Gtk.TreeView;
                var paths = menuLayout.Selection.GetSelectedRows();
                var data = string.Join("\n", paths.Select(p => p.ToString()));
                args.SelectionData.Set(args.SelectionData.Target, 8, System.Text.Encoding.UTF8.GetBytes(data));
            }
        }

        /// <summary>
        /// Handles the drag data received event.
        /// </summary>
        /// <param name="o">The menu layout Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragDataReceived(object o, Gtk.DragDataReceivedArgs args)
        {
            DebugDragDrop("HandleDragDataReceived");
            var treeView = o as Gtk.TreeView;
            Gtk.TreePath path;
            Gtk.TreeViewDropPosition pos;
            if (treeView.GetDestRowAtPos(args.X, args.Y, out path, out pos))
            {
                DebugDragDrop("HandleDragDataReceived: path: " + path + " pos: " + pos);
            }

            Folder dropTarget = null;
            int insertLocation = -1;
            Gtk.TreeIter droppedOnItemIter;
            if (treeView.Model.GetIter(out droppedOnItemIter, path))
            {
                var droppedOnItem = treeView.Model.GetValue(droppedOnItemIter, 0) as FileNodeViewModel;
                dropTarget = droppedOnItem.Model as Folder;
                if (dropTarget == null)
                {
                    // Dropped on an item.
                    dropTarget = droppedOnItem.Parent as Folder;
                    switch (pos)
                    {
                        case Gtk.TreeViewDropPosition.Before:
                        case Gtk.TreeViewDropPosition.IntoOrBefore:
                            insertLocation = path.Indices.Last(); // insert before
                            break;
                        case Gtk.TreeViewDropPosition.After:
                        case Gtk.TreeViewDropPosition.IntoOrAfter:
                            insertLocation = path.Indices.Last() + 1; // insert after
                            if (insertLocation >= dropTarget.Items.Count())
                            {
                                insertLocation = -1;
                            }
                            break;
                    }
                }
                else
                {
                    // Dropped on a folder.
                    switch (pos)
                    {
                        case Gtk.TreeViewDropPosition.Before:
                            dropTarget = dropTarget.Parent as Folder;
                            insertLocation = path.Indices.Last(); // insert before folder
                            break;
                        case Gtk.TreeViewDropPosition.After:
                            dropTarget = dropTarget.Parent as Folder;
                            insertLocation = path.Indices.Last() + 1; // insert after folder
                            if (insertLocation >= dropTarget.Items.Count())
                            {
                                insertLocation = -1;
                            }
                            break;
                        case Gtk.TreeViewDropPosition.IntoOrBefore:
                            insertLocation = 0; // insert at beginning of folder
                            break;
                        case Gtk.TreeViewDropPosition.IntoOrAfter:
                            insertLocation = -1; // append at end of folder
                            break;
                    }
                }
            }
            else
            {
                // Append to end of root.
                pos = Gtk.TreeViewDropPosition.After;
                dropTarget = ViewModel.HostPCMenuLayout.Model as Folder;
            }
            if (dropTarget != null)
            {
                var menuLayout = ViewModel.HostPCMenuLayout;
                if (args.Info == RomListViewModel.DragDropSourceDataIdentifier)
                {
                    DebugDragDrop("HandleDragDataReceived: Data from ROM list");
                    var itemsToAdd = new List<ProgramDescription>();
                    var stringIndices = System.Text.Encoding.UTF8.GetString(args.SelectionData.Data).Split('\n');
                    foreach (var stringIndex in stringIndices)
                    {
                        int index;
                        if (int.TryParse(stringIndex, out index))
                        {
                            var program = INTV.Shared.Model.Program.ProgramCollection.Roms[index];
                            if (program != null)
                            {
                                itemsToAdd.Add(program);
                            }
                        }
                    }
                    MenuLayoutViewModel.AddItems(menuLayout, dropTarget, itemsToAdd, insertLocation);
                }
                else if (args.Info == MenuLayoutViewModel.DragDataFormatIdentifier)
                {
                    DebugDragDrop("HandleDragDataReceived: Rearrange drop");
                    var itemsToMove = new List<FileNodeViewModel>();
                    var stringPaths = System.Text.Encoding.UTF8.GetString(args.SelectionData.Data).Split('\n');
                    foreach (var stringPath in stringPaths)
                    {
                        var itemToMovePath = new Gtk.TreePath(stringPath);
                        Gtk.TreeIter itemIter;
                        if (treeView.Model.GetIter(out itemIter, itemToMovePath))
                        {
                            var fileNode = treeView.Model.GetValue(itemIter, 0) as FileNodeViewModel;
                            itemsToMove.Add(fileNode);
                            DebugDragDrop("HandleDragDataReceived: Plan to move: " + fileNode.LongName + " : " + itemToMovePath.ToString());
                        }
                    }
                    var newParent = menuLayout.FindViewModelForModel(dropTarget);
                    newParent.MoveItems(menuLayout, newParent, insertLocation, itemsToMove);
                }
            }
        }

        /// <summary>
        /// Handles the drag drop event.
        /// </summary>
        /// <param name="o">The menu layout Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragDrop(object o, Gtk.DragDropArgs args)
        {
            DebugDragDrop("HandleDragDrop");
        }

        /// <summary>
        /// Handles the drag end event.
        /// </summary>
        /// <param name="o">The menu layout Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragEnd(object o, Gtk.DragEndArgs args)
        {
            DebugDragDrop("HandleDragEnd");
        }

        /// <summary>
        /// Handles the drag leave event.
        /// </summary>
        /// <param name="o">The menu layout Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragLeave(object o, Gtk.DragLeaveArgs args)
        {
            DebugDragDrop("HandleDragLeave");
        }

        /// <summary>
        /// Handles the DragMotion event.
        /// </summary>
        /// <param name="o">The menu layout Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragMotion(object o, Gtk.DragMotionArgs args)
        {
            DebugDragDrop("HandleDragMotion");
        }

        [System.Diagnostics.Conditional("ENABLE_DRAGDROP_TRACE")]
        private static void DebugDragDrop(object message)
        {
            System.Diagnostics.Debug.WriteLine("DRAG_DROP: MenuLayout: " + message);
        }

        private static void SetDragImage(Gtk.TreeView menuLayout, Gtk.DragBeginArgs args)
        {
            var selection = menuLayout.Selection;
            var firstSelectedPath = selection.GetSelectedRows().First();
            Gtk.TreeIter iter;
            if (menuLayout.Model.GetIter(out iter, firstSelectedPath))
            {
                var firstDraggedItem = menuLayout.Model.GetValue(iter, 0) as FileNodeViewModel;
                string text;
                Gdk.Pixbuf icon;
                if (selection.CountSelectedRows() > 1)
                {
                    text = "<Multiple Items>";
                    icon = firstDraggedItem is FolderViewModel ? DragMultipleItemsFolderImage : DragMultipleItemsProgramImage;
                }
                else
                {
                    text = firstDraggedItem.LongName;
                    icon = firstDraggedItem.Icon;
                }
                var dragWidget = new DragDropImage(icon, text);
                Gtk.Drag.SetIconWidget(args.Context, dragWidget, 0, 0);
            }
        }

        private void HandleCanExecuteDeleteItemsChanged(object sender, System.EventArgs e)
        {
            var ltoFlashViewModel = ViewModel;
            _deleteSelectedItems.Sensitive = MenuLayoutCommandGroup.DeleteItemsCommand.CanExecute(ltoFlashViewModel.HostPCMenuLayout);
        }

        private void HandleCanExecuteCreateNewDirectoryChanged(object sender, System.EventArgs e)
        {
            var ltoFlashViewModel = ViewModel;
            _newFolder.Sensitive = MenuLayoutCommandGroup.NewDirectoryCommand.CanExecute(ltoFlashViewModel.HostPCMenuLayout);
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
                ColorPixbufs[color.IntvColor] = pixbuf;
            }

            Gtk.CellRenderer cellRenderer = new Gtk.CellRendererPixbuf() { Xalign = 0 };
            colorChooser.PackStart(cellRenderer, false);
            colorChooser.SetCellDataFunc(cellRenderer, (l, e, m, i) => VisualHelpers.CellImageRenderer<FileNodeColorViewModel>(l, e, m, i, c => ColorPixbufs[c.IntvColor]));

            cellRenderer = new Gtk.CellRendererCombo() { Xalign = 0, Xpad = 4 };
            colorChooser.PackEnd(cellRenderer, true);
            colorChooser.SetCellDataFunc(cellRenderer, (l, e, m, i) => VisualHelpers.CellTextRenderer<FileNodeColorViewModel>(l, e, m, i, c => c.Name));

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
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellImageColumnRenderer<FileNodeViewModel>(l, c, m, i, p => p.Icon));
            ////column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            ////column.FixedWidth = 20;
            menuLayout.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = MenuLayoutViewModel.LongNameHeader };
            cellRenderer = new Gtk.CellRendererText();
            _longNameEditor = new TextCellInPlaceEditor(menuLayout, column, cellRenderer as Gtk.CellRendererText, FileSystemConstants.MaxLongNameLength) { IsValidCharacter = INTV.Core.Model.Grom.Characters.Contains };
            _longNameEditor.EditorClosed += HandleInPlaceEditorClosed;
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellTextColumnRenderer<FileNodeViewModel>(l, c, m, i, p => p.LongName.SafeString()));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.MenuLayoutLongNameColWidth;
            column.Resizable = true;
            menuLayout.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = MenuLayoutViewModel.ShortNameHeader };
            cellRenderer = new Gtk.CellRendererText();
            _shortNameEditor = new TextCellInPlaceEditor(menuLayout, column, cellRenderer as Gtk.CellRendererText, FileSystemConstants.MaxShortNameLength) { IsValidCharacter = INTV.Core.Model.Grom.Characters.Contains };
            _shortNameEditor.EditorClosed += HandleInPlaceEditorClosed;
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellTextColumnRenderer<FileNodeViewModel>(l, c, m, i, p => p.ShortName.SafeString()));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.MenuLayoutShortNameColWidth;
            column.Resizable = true;
            menuLayout.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = MenuLayoutViewModel.ManualHeader };
            cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellTextColumnRenderer<FileNodeViewModel>(l, c, m, i, GetManualColumnStringValue));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.MenuLayoutManualColWidth;
            column.Resizable = true;
            menuLayout.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = MenuLayoutViewModel.SaveDataHeader };
            cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, true);
            ////column.SetCellDataFunc(cellRenderer, (l,c,m,i) => VisualHelpers.CellTextColumnRenderer<ProgramDescriptionViewModel>(l,c,m,i, p => p.Name));
            ////column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            ////column.FixedWidth = Properties.Settings.Default.MenuLayoutSaveDataColWidth;
            column.Resizable = true;
            column.Visible = Properties.Settings.Default.ShowAdvancedFeatures;
            menuLayout.AppendColumn(column);

            menuLayout.HasTooltip = true;
            menuLayout.QueryTooltip += HandleMenuLayoutQueryTooltip;

            var menuLayoutStore = new Gtk.TreeStore(typeof(FileNodeViewModel));
            dataContext.SynchronizeToTreeStore(menuLayoutStore);
            menuLayout.Model = menuLayoutStore;

            menuLayout.EnableModelDragSource(Gdk.ModifierType.Button1Mask, DragDropSourceEntries, Gdk.DragAction.Move);
            menuLayout.EnableModelDragDest(DragDropTargetEntries, Gdk.DragAction.Copy | Gdk.DragAction.Move);
            menuLayout.Selection.Changed += HandleSelectionChanged;

            dataContext.PropertyChanged += HandleMenuLayoutPropertyChanged;
        }

        private string GetManualColumnStringValue(FileNodeViewModel node)
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
            this.HandleEventOnMainThread(sender, e, HandleMenuLayoutPropertyChangedCore);
        }

        private void HandleMenuLayoutPropertyChangedCore(object sender, PropertyChangedEventArgs e)
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
        }

        private void HandleLtoFlashPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleLtoFlashPropertyChangedCore);
        }

        private void HandleLtoFlashPropertyChangedCore(object sender, PropertyChangedEventArgs e)
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
        }

        private void HandleActiveDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleActiveDevicePropertyChangedCore);
        }

        private void HandleActiveDevicePropertyChangedCore(object sender, PropertyChangedEventArgs e)
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
        }

        private void UpdateActiveDeviceViewModelInfo(DeviceViewModel device)
        {
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
