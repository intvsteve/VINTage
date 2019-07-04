// <copyright file="RomListView.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2019 All Rights Reserved
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
using System.Linq;
using INTV.Shared.Commands;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    /// <remarks>Much of the model work here could / should be relocated to a RomListViewModel.Gtk.cs file.</remarks>
    [System.ComponentModel.ToolboxItem(true)]
    [Gtk.Binding(Gdk.Key.Delete, "HandleDeleteSelectedItems")]
    [Gtk.Binding(Gdk.Key.BackSpace, "HandleDeleteSelectedItems")]
    public partial class RomListView : Gtk.Bin, IFakeDependencyObject
    {
        private static readonly Gdk.Pixbuf DragOneRomImage = typeof(RomListView).LoadImageResource("Resources/Images/rom_16xSM.png");
        private static readonly Gdk.Pixbuf DragMultipleRomsImage = typeof(RomListView).LoadImageResource("Resources/Images/roms_16xSM.png");

        private bool _updatingSelection;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.RomListView"/> class.
        /// </summary>
        /// <param name="viewModel">The data context for this visual.</param>
        public RomListView(RomListViewModel viewModel)
        {
            // TODO: Set up sorting!
            // TODO: DragDrop!
            DataContext = viewModel;
            this.Build();
            var treeView = _romListView;
            treeView.Selection.Mode = Gtk.SelectionMode.Multiple;
            treeView.HasTooltip = true;
            treeView.EnableModelDragDest(RomListViewModel.DragDropTargetEntries, Gdk.DragAction.Private);
            treeView.EnableModelDragSource(Gdk.ModifierType.Button1Mask, RomListViewModel.DragDropSourceEntries, Gdk.DragAction.Copy);

            var column = new Gtk.TreeViewColumn();
            Gtk.CellRenderer cellRenderer = new Gtk.CellRendererPixbuf();
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellImageColumnRenderer<ProgramDescriptionViewModel>(l, c, m, i, p => p.RomFileStatusIcon));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = 20;
            treeView.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = RomListViewModel.TitleHeader };
            cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellTextColumnRenderer<ProgramDescriptionViewModel>(l, c, m, i, p => p.Name));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.RomListNameColWidth;
            column.Resizable = true;
            treeView.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = RomListViewModel.CompanyHeader };
            cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellTextColumnRenderer<ProgramDescriptionViewModel>(l, c, m, i, p => p.Vendor));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.RomListVendorColWidth;
            column.Resizable = true;
            treeView.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = RomListViewModel.YearHeader };
            cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellTextColumnRenderer<ProgramDescriptionViewModel>(l, c, m, i, p => p.Year));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.RomListYearColWidth;
            column.Resizable = true;
            treeView.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = RomListViewModel.FeaturesHeader };
            cellRenderer = new Gtk.CellRendererPixbuf();
            cellRenderer.Xalign = 0;
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellImageColumnRenderer<ProgramDescriptionViewModel>(l, c, m, i, p => INTV.Shared.Converter.ProgramFeaturesToPixbufConverter.ConvertToPixbuf(p)));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = Properties.Settings.Default.RomListFeaturesColWidth;
            column.Resizable = true;
            treeView.AppendColumn(column);

            column = new Gtk.TreeViewColumn() { Title = RomListViewModel.RomFileHeader };
            cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellTextColumnRenderer<ProgramDescriptionViewModel>(l, c, m, i, p => p.Rom.RomPath.Path));
            ////column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            ////column.FixedWidth = Properties.Settings.Default.RomListPathColWidth;
            column.Resizable = true;
            treeView.AppendColumn(column);

            treeView.QueryTooltip += HandleQueryTooltip;

            ////column = new Gtk.TreeViewColumn() { Title = "Manual" };
            ////cellRenderer = new Gtk.CellRendererText();
            ////column.PackStart(cellRenderer, true);
            ////column.SetCellDataFunc(cellRenderer, (l,c,m,i) =>VisualHelpers.CellTextColumnRenderer<ProgramDescriptionViewModel>(l,c,m,i, p => p.ManualPath));
            ////column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            ////column.FixedWidth = Properties.Settings.Default.RomListManualPathColWidth;;
            ////column.Resizable = true;
            ////treeView.AppendColumn(column);

            var romListStore = new Gtk.ListStore(typeof(ProgramDescriptionViewModel));
            romListStore.SynchronizeCollection(viewModel.Programs);
            treeView.Model = romListStore;

            // Hackish way to get the visibility right.
            HandleSettingsChanged(Properties.Settings.Default, new System.ComponentModel.PropertyChangedEventArgs(Properties.Settings.ShowRomDetailsSettingName));
            Properties.Settings.Default.PropertyChanged += HandleSettingsChanged;
            viewModel.Programs.CollectionChanged += HandleProgramsChanged;
            viewModel.Model.ProgramStatusChanged += HandleProgramStatusChanged;

            treeView.Selection.Changed += HandleSelectionChanged;
            ViewModel.CurrentSelection.CollectionChanged += HandleViewModelSelectionChanged;
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

        private RomListViewModel ViewModel
        {
            get { return (RomListViewModel)DataContext; }
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            var columnNumbers = new[] { RomListColumn.Title, RomListColumn.Vendor, RomListColumn.Year, RomListColumn.Features, RomListColumn.RomFile /*, RomListColumn.ManualFile*/ };
            foreach (var columnNumber in columnNumbers)
            {
                var column = _romListView.Columns[(int)columnNumber];
                var width = column.Width;
                if (width == 0)
                {
                    continue;
                }
                switch (columnNumber)
                {
                    case RomListColumn.Title:
                        Properties.Settings.Default.RomListNameColWidth = width;
                        break;
                    case RomListColumn.Vendor:
                        Properties.Settings.Default.RomListVendorColWidth = width;
                        break;
                    case RomListColumn.Year:
                        Properties.Settings.Default.RomListYearColWidth = width;
                        break;
                    case RomListColumn.Features:
                        Properties.Settings.Default.RomListFeaturesColWidth = width;
                        break;
                    case RomListColumn.RomFile:
                        ////Properties.Settings.Default.RomListPathColWidth = width;
                        break;
                    case RomListColumn.ManualFile:
                        throw new System.NotImplementedException("RomList Manual File column width");
                    default:
                        break;
                }
            }
            base.OnDestroyed();
        }

        /// <summary>
        /// Handles the rom list FocusIn event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleRomListFocusIn(object o, Gtk.FocusInEventArgs args)
        {
            ViewModel.ListHasFocus = true;
        }

        /// <summary>
        /// Handles the rom list FocusOut event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleRomListFocusOut(object o, Gtk.FocusOutEventArgs args)
        {
            ViewModel.ListHasFocus = false;
        }

        /// <summary>
        /// Handles the drag begin event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragBegin(object o, Gtk.DragBeginArgs args)
        {
            var numItemsSelected = ViewModel.CurrentSelection.Count;
            DebugDragDrop("HandleDragBegin: numItemsSelected: " + numItemsSelected);
            var dragImage = numItemsSelected > 1 ? DragMultipleRomsImage : DragOneRomImage;
            var dragText = numItemsSelected > 1 ? "<Multiple Items>" : ViewModel.CurrentSelection.First().Name;
            var dragWidget = new DragDropImage(dragImage, dragText);
            Gtk.Drag.SetIconWidget(args.Context, dragWidget, 0, 0);
        }

        /// <summary>
        /// Handles the drag data delete event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragDataDelete(object o, Gtk.DragDataDeleteArgs args)
        {
            DebugDragDrop("HandleDragDataDelete");
        }

        /// <summary>
        /// Handles the drag data get event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragDataGet(object o, Gtk.DragDataGetArgs args)
        {
            DebugDragDrop("HandleDragDataGet");
            if (args.Info == RomListViewModel.DragDropSourceDataIdentifier)
            {
                var romList = o as Gtk.TreeView;
                var paths = romList.Selection.GetSelectedRows();
                var data = string.Join("\n", paths.Select(p => p.ToString()));
                args.SelectionData.Set(args.SelectionData.Target, 8, System.Text.Encoding.UTF8.GetBytes(data));
            }
        }

        /// <summary>
        /// Handles the drag data received event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
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
            var dragDataArgs = new System.Tuple<Gtk.DragDataReceivedArgs, Gtk.TreePath, Gtk.TreeViewDropPosition>(args, path, pos);
            ViewModel.DropFilesCommand.Execute(dragDataArgs);
        }

        /// <summary>
        /// Handles the drag drop event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragDrop(object o, Gtk.DragDropArgs args)
        {
            DebugDragDrop("HandleDragDrop");
        }

        /// <summary>
        /// Handles the drag end event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragEnd(object o, Gtk.DragEndArgs args)
        {
            DebugDragDrop("HandleDragEnd");
        }

        /// <summary>
        /// Handles the drag leave event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragLeave(object o, Gtk.DragLeaveArgs args)
        {
            DebugDragDrop("HandleDragLeave");
        }

        /// <summary>
        /// Handles the DragMotion event.
        /// </summary>
        /// <param name="o">The ROM list Gtk.TreeView.</param>
        /// <param name="args">The event data.</param>
        protected void HandleDragMotion(object o, Gtk.DragMotionArgs args)
        {
            DebugDragDrop("HandleDragMotion");
        }

        [System.Diagnostics.Conditional("ENABLE_DRAGDROP_TRACE")]
        private static void DebugDragDrop(object message)
        {
            System.Diagnostics.Debug.WriteLine("DRAG_DROP: RomsList: " + message);
        }

        private void HandleDeleteSelectedItems()
        {
            if (RomListCommandGroup.RemoveRomsCommand.CanExecute(DataContext))
            {
                RomListCommandGroup.RemoveRomsCommand.Execute(DataContext);
            }
        }

        private void HandleQueryTooltip(object o, Gtk.QueryTooltipArgs args)
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
                    var item = treeView.Model.GetValue(iter, 0) as ProgramDescriptionViewModel;
                    var romListColumn = (RomListColumn)System.Array.IndexOf(treeView.Columns, column);
                    var tooltip = string.Empty;
                    switch (romListColumn)
                    {
                        case RomListColumn.None:
                            tooltip = item.RomFileStatus;
                            break;
                        case RomListColumn.Title:
                            tooltip = item.Name;
                            break;
                        case RomListColumn.Vendor:
                            tooltip = item.Vendor;
                            break;
                        case RomListColumn.Year:
                            ////tooltip = item.Year;
                            break;
                        case RomListColumn.Features:
                            tooltip = INTV.Shared.Converter.ProgramFeaturesToPixbufConverter.GetFeatureTooltip(item, cellX, cellY);
                            break;
                        case RomListColumn.RomFile:
                            tooltip = item.Rom.RomPath.Path;
                            break;
                        case RomListColumn.ManualFile:
                            throw new System.NotImplementedException("RomListView.HandleQueryTooltip(): ManualFile");
                        default:
                            break;
                    }

                    if (!string.IsNullOrEmpty(tooltip))
                    {
                        args.Tooltip.Text = tooltip;
                        args.RetVal = true;
                    }
                }
            }
        }

        private void HandleSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleSettingsChangedCore);
        }

        private void HandleSettingsChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var show = Properties.Settings.Default.ShowRomDetails;
            switch (e.PropertyName)
            {
                case Properties.Settings.ShowRomDetailsSettingName:
                    _romListView.Columns[(int)RomListColumn.Vendor].Visible = show;
                    _romListView.Columns[(int)RomListColumn.Year].Visible = show;
                    _romListView.Columns[(int)RomListColumn.Features].Visible = show;
                    ////_romListView.Columns[(int)RomListColumn.ManualFile].Visible = show;
                    break;
                default:
                    break;
            }
        }

        private void HandleProgramsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleProgramsChangedCore);
        }

        private void HandleProgramsChangedCore(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _romListView.Model.SynchronizeCollection<ProgramDescriptionViewModel>(e);
        }

        private void HandleProgramStatusChanged(object sender, INTV.Shared.Model.Program.ProgramFeaturesChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleProgramStatusChangedCore);
        }

        private void HandleProgramStatusChangedCore(object sender, INTV.Shared.Model.Program.ProgramFeaturesChangedEventArgs e)
        {
            // Need to goose the TreeView to re-draw the status icons.
            _romListView.QueueDraw();
        }

        private void HandleSelectionChanged(object sender, System.EventArgs e)
        {
            if (!_updatingSelection)
            {
                _updatingSelection = true;
                try
                {
                    var viewModelSelection = ViewModel.CurrentSelection;
                    var selectedItems = new List<ProgramDescriptionViewModel>();
                    var selection = sender as Gtk.TreeSelection;
                    var selectedItemPaths = selection.GetSelectedRows();
                    foreach (var path in selectedItemPaths)
                    {
                        Gtk.TreeIter iter;
                        if (selection.TreeView.Model.GetIter(out iter, path))
                        {
                            var item = selection.TreeView.Model.GetValue(iter, 0) as ProgramDescriptionViewModel;
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

                    INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
                }
                finally
                {
                    _updatingSelection = false;
                }
            }
        }

        private void HandleViewModelSelectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!_updatingSelection)
            {
                this.HandleEventOnMainThread(sender, e, HandleViewModelSelectionChangedCore);
            }
        }

        private void HandleViewModelSelectionChangedCore(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _updatingSelection = true;
            try
            {
                var selection = _romListView.Selection;
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                        if (e.NewItems != null)
                        {
                            foreach (ProgramDescriptionViewModel item in e.NewItems)
                            {
                                var index = ViewModel.Programs.IndexOf(item);
                                var path = new Gtk.TreePath();
                                path.AppendIndex(index);
                                selection.SelectPath(path);
                            }
                        }
                        if (e.OldItems != null)
                        {
                            foreach (ProgramDescriptionViewModel item in e.OldItems)
                            {
                                var index = ViewModel.Programs.IndexOf(item);
                                var path = new Gtk.TreePath();
                                path.AppendIndex(index);
                                selection.UnselectPath(path);
                            }
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        selection.UnselectAll();
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                        break;
                }
            }
            finally
            {
                _updatingSelection = false;
            }
        }
    }
}
