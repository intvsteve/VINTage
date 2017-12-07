// <copyright file="RomListViewController.Mac.cs" company="INTV Funhouse">
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

////#define ENABLE_ITEMCHANGE_TRACE
////#define ENABLE_DRAGDROP_TRACE

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

#if __UNIFIED__
using AppKit;
using Foundation;
using ObjCRuntime;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__
using INTV.Core.ComponentModel;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.Shared.Behavior;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

#if __UNIFIED__
using nint = System.nint;
using nfloat = System.nfloat;
using INSPasteboardWriting = AppKit.INSPasteboardWriting;
using CGPoint = CoreGraphics.CGPoint;
using CGRect = CoreGraphics.CGRect;
#else
using nint = System.Int32;
using nfloat = System.Single;
using INSPasteboardWriting = MonoMac.AppKit.NSPasteboardWriting;
using CGPoint = System.Drawing.PointF;
using CGRect = System.Drawing.RectangleF;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Controller implementation for the RomListView. Does the real work of hooking up
    /// DataContext and other event / data management.
    /// </summary>
    public partial class RomListViewController : NSViewController
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public RomListViewController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public RomListViewController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public RomListViewController()
            : base("RomListView", NSBundle.MainBundle)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the view as a strongly typed value.
        /// </summary>
        public new RomListView View { get { return (RomListView)base.View; } }

        /// <summary>
        /// Gets the text to display showing the prompt to drop files in the ROM list area.
        /// </summary>
        [Export("DropFilesHereText")]
        public string DropFilesHereText { get { return RomListViewModel.DropFilesHere; } }

        /// <summary>
        /// Gets the array data controller used for the Mac-specific UI-facing data used by the ROMs NSTableView control.
        /// </summary>
        public NSArrayController DataController { get { return RomsArrayController; } }

        private RomListTableViewDelegate TheDelegate { get; set; }

        private RomListDataSource DataSource { get; set; }

        /// <inheritdoc/>
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            View.Controller = this;

            var nsArray = NSArray.FromNSObjects(View.ViewModel.Programs.ToArray());

            var origValue = RomsArrayController.SelectsInsertedObjects;
            RomsArrayController.SelectsInsertedObjects = false;
            RomsArrayController.AddObjects(nsArray);
            RomsArrayController.RearrangeObjects();
            RomsArrayController.SelectsInsertedObjects = origValue;

            var table = View.FindChild<ROMsTableView>();
            table.Controller = this;
            DataSource = new RomListDataSource(RomsArrayController);
            table.DataSource = DataSource;
            var tableDelegate = new RomListTableViewDelegate(RomsArrayController, View.ViewModel);
            table.Delegate = tableDelegate;
            TheDelegate = tableDelegate;
            var programs = RomsArrayController.ArrangedObjects();
            for (int i = 0; i < (int)nsArray.Count; ++i)
            {
                var program = programs[i] as ProgramDescriptionViewModel;
                var featuresTip = program.FeaturesTip;
            }

            View.ViewModel.Programs.CollectionChanged += HandleProgramsChanged;
            View.ViewModel.CurrentSelection.CollectionChanged += HandleRomListSelectionChanged;
            HandleRomListSelectionChanged(null, null);

            INTV.Shared.Properties.Settings.Default.PropertyChanged += HandlePreferenceChanged;
            HandlePreferenceChanged(null, new System.ComponentModel.PropertyChangedEventArgs(RomListSettingsPageViewModel.ShowRomDetailsPropertyName));

            View.RegisterForDraggedTypes(new string[] { NSPasteboard.NSFilenamesType });
            INTV.Core.Model.Device.Peripheral.PeripheralAttached += HandlePeripheralArrivalOrDeparture;
            INTV.Core.Model.Device.Peripheral.PeripheralDetached += HandlePeripheralArrivalOrDeparture;
        }

        /// <summary>
        /// Gets the ViewModel of the ROM at a specific row in the ROMs table.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal ProgramDescriptionViewModel GetObjectAtRow(int row)
        {
            var programViewModel = RomsArrayController.ArrangedObjects().ToList()[row] as ProgramDescriptionViewModel;
            return programViewModel;
        }

        /// <summary>
        /// Start editing a program description.
        /// </summary>
        /// <param name="program">The program description to be edited.</param>
        internal void EditProgramDescription(ProgramDescriptionViewModel program)
        {
            var table = View.FindChild<NSTableView>();
            var itemToEditIndex = RomsArrayController.ArrangedObjects().ToList().IndexOf(program);
            var column = table.TableColumns()[(int)RomListColumn.Title];
            if ((itemToEditIndex >= 0) && table.Delegate.ShouldEditTableColumn(table, column, itemToEditIndex))
            {
                table.EditColumn((int)RomListColumn.Title, itemToEditIndex);
            }
        }

        /// <summary>
        /// Cancels the edit session of the program description being edited (if any).
        /// </summary>
        internal void CancelEditProgramDescription()
        {
            var table = View.FindChild<NSTableView>();
            var tableDelegate = table.Delegate as RomListTableViewDelegate;
            tableDelegate.CancelEdit();
        }

        /// <summary>
        /// Sorts the ROM list.
        /// </summary>
        internal void SortRoms()
        {
            RomsArrayController.RearrangeObjects();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            // TODO: Disconnect event handlers?
            View.ViewModel.Programs.CollectionChanged -= HandleProgramsChanged;
            View.ViewModel.CurrentSelection.CollectionChanged -= HandleRomListSelectionChanged;
            INTV.Shared.Properties.Settings.Default.PropertyChanged -= HandlePreferenceChanged;
            INTV.Core.Model.Device.Peripheral.PeripheralAttached -= HandlePeripheralArrivalOrDeparture;
            INTV.Core.Model.Device.Peripheral.PeripheralDetached -= HandlePeripheralArrivalOrDeparture;
            base.Dispose(disposing);
        }

        private void HandlePeripheralArrivalOrDeparture(object sender, INTV.Core.Model.Device.PeripheralEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandlePeripheralArrivalOrDepartureCore);
        }

        private void HandlePeripheralArrivalOrDepartureCore(object sender, INTV.Core.Model.Device.PeripheralEventArgs e)
        {
            View.NeedsDisplay = true;
        }

        private void HandlePreferenceChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandlePreferenceChangedCore);
        }

        private void HandlePreferenceChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case RomListSettingsPageViewModel.ShowRomDetailsPropertyName:
                    var showColumns = INTV.Shared.Properties.Settings.Default.ShowRomDetails;
                    var columnsToHide = (new[] {
                        RomListColumn.Vendor,
                        RomListColumn.Year,
                        RomListColumn.Features,
                        RomListColumn.ManualFile
                    }).Select(c => c.ToString().ToLower());
                    var table = View.FindChild<NSTableView>();
                    foreach (var column in table.TableColumns())
                    {
                        if (columnsToHide.Contains(column.Identifier.ToLower()))
                        {
                            column.Hidden = !showColumns;
                        }
                    }
                    break;
                case RomListSettingsPageViewModel.DisplayRomFileNameForTitlePropertyName:
                    var programs = RomsArrayController.ArrangedObjects();
                    foreach (var programObject in programs)
                    {
                        var program = programObject as ProgramDescriptionViewModel;
                        program.RaiseChangeValueForKey("Name");
                    }
                    break;
                default:
                    break;
            }
        }

        private void HandleRomListCommand(object sender, System.EventArgs e)
        {
            var dataContext = View.DataContext;
            ICommand command = null;
            if (sender is NSToolbarItem)
            {
                var romListCommandProvider = SingleInstanceApplication.Instance.GetCommandProvider<RomListCommandsProvider>();
                command = romListCommandProvider.GetCommandForUniqueIdentifier(((NSToolbarItem)sender).Identifier);
            }
            else if (sender is NSMenuItem)
            {
                command = ((NSObjectWrapper<ICommand>)((NSMenuItem)sender).RepresentedObject).WrappedObject;
            }
            var relayCommand = command as VisualRelayCommand;
            if (relayCommand.UniqueId == RomListCommandGroup.ShowRomInfoCommand.UniqueId)
            {
                var newState = !Properties.Settings.Default.ShowRomDetails;
                NSUserDefaults.StandardUserDefaults[RomListSettingsPageViewModel.ShowRomDetailsPropertyName] = new NSNumber(newState);
                relayCommand.MenuItem.NativeMenuItem.State = newState ? NSCellStateValue.On : NSCellStateValue.Off;
            }
            else
            {
                command.Execute(dataContext);
            }
        }

        private void HandleCanEditProgramNameChanged(object sender, System.EventArgs e)
        {
            var canExecute = RomListCommandGroup.EditProgramNameCommand.CanExecute(View.ViewModel);
            if (!canExecute)
            {
                CancelEditProgramDescription();
            }
            var menuItem = RomListCommandGroup.EditProgramNameCommand.MenuItem;
            if (!menuItem.IsEmpty)
            {
                menuItem.NativeMenuItem.Enabled = canExecute;
            }
        }

        private void HandleProgramsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleProgramsChangedCore);
        }

        private void HandleProgramsChangedCore(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DebugItemChange("ROMS COLLECTION CHANGED");
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddObjectsToArrayController(e.NewItems.OfType<NSObject>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveObjectsFromArrayController(e.OldItems.OfType<NSObject>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    var itemsToDelete = NSIndexSet.FromNSRange(new NSRange(0, RomsArrayController.ArrangedObjects().Length));
                    RomsArrayController.Remove(itemsToDelete);
                    HandleRomListSelectionChanged(null, null); // ensure we show 'drop stuff here' text
                    break;
            }
        }

        private void AddObjectsToArrayController(IEnumerable<NSObject> objectsToAdd)
        {
            foreach (var item in objectsToAdd)
            {
                DebugItemChange("ROM list ADD: item as NSObject is " + item + ", is null? " + (item == null));
                RomsArrayController.AddObject(item);
            }
        }

        private void RemoveObjectsFromArrayController(IEnumerable<NSObject> objectsToRemove)
        {
            foreach (var item in objectsToRemove)
            {
                DebugItemChange("removed " + item + " of type " + item.GetType().FullName);
                RomsArrayController.RemoveObject(item);
            }
            var selectedItemsToRemove = objectsToRemove.Intersect(View.ViewModel.CurrentSelection.OfType<NSObject>());
            if (selectedItemsToRemove.Any())
            {
                foreach (var item in selectedItemsToRemove.ToList())
                {
                    View.ViewModel.CurrentSelection.Remove(item as ProgramDescriptionViewModel);
                }
                var selectedObjects = RomsArrayController.SelectionIndexes.Select(i => RomsArrayController.ArrangedObjects()[i]);
                var itemsToAdd = selectedObjects.Except(View.ViewModel.CurrentSelection);
                foreach (var item in itemsToAdd)
                {
                    View.ViewModel.CurrentSelection.Add((ProgramDescriptionViewModel)item);
                }
            }
        }

        private void HandleRomListSelectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleRomListSelectionChangedCore);
        }

        private void HandleRomListSelectionChangedCore(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var numberOfRoms = View.ViewModel.Programs.Count;

            if (numberOfRoms == 0)
            {
                View.AlphaValue = 0.25f;
                DropFilesHint.Hidden = false;
                var table = View.FindChild<NSTableView>();
                table.Enabled = false;
                ItemsCount.Hidden = true;
                SelectedItemsCount.Hidden = true;
            }
            else
            {
                View.AlphaValue = 1;
                DropFilesHint.Hidden = true;
                var table = View.FindChild<NSTableView>();
                table.Enabled = true;
                var formats = Resources.Strings.RomListViewModel_NumItemsFormat.Split(';');
                var format = (numberOfRoms == 1) ? formats[1] : formats[0];
                ItemsCount.StringValue = string.Format(format, numberOfRoms);

                formats = Resources.Strings.RomListViewModel_NumItemsSelectedFormat.Split(';');
                var numSelected = View.ViewModel.CurrentSelection.Count;
                format = (numSelected == 1) ? formats[1] : formats[0];
                SelectedItemsCount.StringValue = string.Format(format, numSelected);
                ItemsCount.Hidden = false;
                SelectedItemsCount.Hidden = numSelected == 0;
            }
        }

        [System.Diagnostics.Conditional("ENABLE_ITEMCHANGE_TRACE")]
        private static void DebugItemChange(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        [System.Diagnostics.Conditional("ENABLE_DRAGDROP_TRACE")]
        private static void DebugDragDrop(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// Called when an element in the ROM list is double-clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        partial void OnDoubleClick(NSObject sender)
        {
            var arrangedObjectsArray = sender as NSArray;
            if (arrangedObjectsArray != null)
            {
                // If multiple items selected, get the one that was double-clicked.
                var mouseLocation = NSEvent.CurrentMouseLocation;
                var table = View.FindChild<NSTableView>();
                var rect = table.Window.ConvertRectFromScreen(new CGRect(mouseLocation.X, mouseLocation.Y, 0, 0));
                rect = table.ConvertRectFromView(rect, null);
                var row = table.GetRow(new CGPoint(rect.X, rect.Y));
                if ((row >= 0) && (row < (int)arrangedObjectsArray.Count))
                {
                    var programs = NSArray.FromArray<ProgramDescriptionViewModel>(arrangedObjectsArray);
                    var doubleClickedProgram = programs[row];
                    DebugItemChange(doubleClickedProgram.Name);
                    View.ViewModel.Model.InvokeProgramFromDescription(doubleClickedProgram.Model);
                }
            }
        }

        /// <summary>
        /// Implements the <see cref="NSPasteboardWriting"/> protocol methods necessary for drag and drop on the ROM list.
        /// </summary>
        private class ProgramInformationPasteboardWriting : NSPasteboardWriting
        {
            /// <summary>
            /// Initializes a new instance of the ProgramInformationPasteboardWriting type.
            /// </summary>
            /// <param name="programDescription">The program being dragged.</param>
            public ProgramInformationPasteboardWriting(ProgramDescriptionViewModel programDescription)
            {
                ProgramDescription = programDescription;
            }

            /// <summary>
            /// Gets the program being dragged.
            /// </summary>
            public ProgramDescriptionViewModel ProgramDescription { get; private set; }

            /// <inheritdoc/>
            public override NSObject GetPasteboardPropertyListForType(string type)
            {
                return null;
            }

            /// <inheritdoc/>
            public override string[] GetWritableTypesForPasteboard(NSPasteboard pasteboard)
            {
                return RomListDataSource.ProgramDescriptionPasteboardDataTypeArray;
            }
        }

        /// <summary>
        /// Subclass <see cref="NSTableViewDataSource"/> to get the desired drag/drop behaviors.
        /// </summary>
        internal class RomListDataSource : NSTableViewDataSource
        {
            /// <summary>
            /// Drag and drop data types that we support.
            /// </summary>
            internal static readonly string[] ProgramDescriptionPasteboardDataTypeArray = new string[] { ProgramDescriptionViewModel.DragDataFormat };

            /// <summary>
            /// Initializes a new instance of the RomListDataSource type.
            /// </summary>
            /// <param name="romListArrayController"></param>
            public RomListDataSource(NSArrayController romListArrayController)
            {
                RomListData = romListArrayController;
            }

            private NSArrayController RomListData { get; set; }

            /// <inheritdoc/>
            public override INSPasteboardWriting GetPasteboardWriterForRow(NSTableView tableView, nint row)
            {
                DebugDragDrop("**** ROMLIST GetPBWriterForRow CALLED");
                var programDescriptionViewModel = RomListData.ArrangedObjects()[row] as ProgramDescriptionViewModel;
                var pasteBoardWriting = new ProgramInformationPasteboardWriting(programDescriptionViewModel);
                // look to see where we're already using NSData
                return pasteBoardWriting;
            }

            /// <inheritdoc/>
            public override void DraggingSessionWillBegin(NSTableView tableView, NSDraggingSession draggingSession, CGPoint willBeginAtScreenPoint, NSIndexSet rowIndexes)
            {
                DebugDragDrop("**** ROMLIST DRAG WILL BEGIN");
                //var viewModel = tableView.GetInheritedValue(IFakeDependencyObjectHelpers.DataContextPropertyName) as RomListViewModel;
                var draggedItems = new List<ProgramDescriptionViewModel>();
                var items = RomListData.ArrangedObjects();
                foreach (var index in rowIndexes)
                {
                    draggedItems.Add(items[(int)index] as ProgramDescriptionViewModel);
                }
                if (draggedItems.Any())
                {
                    var pasteboard = draggingSession.DraggingPasteboard;
                    DragDropHelpers.PreparePasteboard(pasteboard, ProgramDescriptionViewModel.DragDataFormat, new NSDataWrapper(draggedItems));
                }
            }


            /// <inheritdoc/>
            public override void DraggingSessionEnded(NSTableView tableView, NSDraggingSession draggingSession, CGPoint endedAtScreenPoint, NSDragOperation operation)
            {
                DebugDragDrop("**** ROMLIST DRAG ENDED");
                DragDropHelpers.FinishedWithPasteboard(draggingSession.DraggingPasteboard);
            }

            /// <inheritdoc/>
            public override void UpdateDraggingItems(NSTableView tableView, NSDraggingInfo draggingInfo)
            {
                DebugDragDrop("***** UPDATE DRAGGING ITEMS");
            }

            /// <inheritdoc/>
            public override NSDragOperation ValidateDrop(NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
            {
                DebugDragDrop("***** UPDATE VALIDATE DROP");
                return NSDragOperation.Link;
            }
        }

        ///<summary>
        /// This is necessary to work around a bug in the delegate implementation from Xamarin.
        /// See: https://bugzilla.xamarin.com/show_bug.cgi?id=12467
        /// </summary>
        private class RomListTableViewDelegate : NSTableViewDelegate
        {
            /// <summary>
            /// Initializes a new instance of the RomListTableViewDelegate type.
            /// </summary>
            /// <param name="programs"></param>
            /// <param name="viewModel"></param>
            internal RomListTableViewDelegate(NSArrayController programs, RomListViewModel viewModel)
            {
                Programs = programs;
                ViewModel = viewModel;
            }

            private NSArrayController Programs { get; set; }

            private RomListViewModel ViewModel { get; set; }

            private TextCellInPlaceEditor InPlaceEditor { get; set; }

            private double ReturnKeyTimestamp { get; set; }

            ///<inheritdoc/>
            public override void SelectionDidChange(NSNotification notification)
            {
                var romsTable = notification.Object as NSTableView;
                var viewModel = romsTable.GetInheritedValue(IFakeDependencyObjectHelpers.DataContextPropertyName) as RomListViewModel;
                var currentSelection = viewModel.CurrentSelection;
                var updatedSelection = Programs.SelectedObjects;
                var itemsToRemove = currentSelection.Except(updatedSelection).ToList();
                var itemsToAdd = updatedSelection.Except(currentSelection).ToList();
                foreach (var item in itemsToRemove)
                {
                    var programDescription = ((ProgramDescriptionViewModel)item);
                    currentSelection.Remove(programDescription);
                }
                foreach (var item in itemsToAdd)
                {
                    var programDescription = ((ProgramDescriptionViewModel)item);
                    currentSelection.Add(programDescription);
                }
            }

            ///<inheritdoc/>
            public override bool SelectionShouldChange(NSTableView tableView)
            {
                return !SingleInstanceApplication.Current.IsBusy;
            }

            /// <inheritdoc/>
            public override bool ShouldEditTableColumn(NSTableView tableView, NSTableColumn tableColumn, nint row)
            {
                var programDescription = Programs.ArrangedObjects()[row] as ProgramDescriptionViewModel;
                var canEdit = RomListCommandGroup.EditProgramNameCommand.CanExecute(ViewModel);
                if (canEdit)
                {
                    string initialValue = null;
                    var column = (RomListColumn)tableView.TableColumns().ToList().IndexOf(tableColumn);
                    int maxLength = -1;
                    System.Predicate<char> isValidCharacter = null;
                    switch (column)
                    {
                        case RomListColumn.Title:
                            canEdit = true;
                            initialValue = programDescription.Name;
                            maxLength = ProgramDescription.MaxProgramNameLength;
                            break;
                        case RomListColumn.Vendor:
                            canEdit = true;
                            initialValue = programDescription.Vendor;
                            maxLength = ProgramDescription.MaxVendorNameLength;
                            break;
                        case RomListColumn.Year:
                            canEdit = true;
                            initialValue = programDescription.Year;
                            maxLength = ProgramDescription.MaxYearTextLength;
                            isValidCharacter = (c) => char.IsDigit(c);
                            break;
                        default:
                            break;
                    }
                    if (canEdit)
                    {
                        if (InPlaceEditor == null)
                        {
                            InPlaceEditor = new TextCellInPlaceEditor(tableView);
                        }
                        InPlaceEditor.EditingObject = programDescription;
                        InPlaceEditor.InitialValue = initialValue;
                        InPlaceEditor.MaxLength = maxLength;
                        InPlaceEditor.IsValidCharacter = isValidCharacter;
                        InPlaceEditor.EditorClosed += InPlaceEditor_EditorClosed;
                        InPlaceEditor.BeginEdit();
                    }
                }
                else if ((SingleInstanceApplication.Current.LastKeyPressed == 0x24) && (SingleInstanceApplication.Current.LastKeyPressedTimestamp != ReturnKeyTimestamp))
                {
                    // return was pressed
                    ReturnKeyTimestamp = SingleInstanceApplication.Current.LastKeyPressedTimestamp;
                    DebugItemChange(programDescription.Name);
                    ViewModel.Model.InvokeProgramFromDescription(programDescription.Model);
                }
                return canEdit;
            }

            /// <inheritdoc/>
            public override void WillDisplayCell(NSTableView tableView, NSObject cell, NSTableColumn tableColumn, nint row)
            {
                var tableCell = cell as NSCell;
                if ((tableCell != null) && (tableView.Menu != null))
                {
                    tableCell.Menu = tableView.Menu;
                }
            }

#if __UNIFIED__
            /// <inheritdoc/>
            public override NSString GetToolTip(NSTableView tableView, NSCell cell, ref CGRect rect, NSTableColumn tableColumn, nint row, CGPoint mouseLocation)
            {
                return GetToolTip(cell, rect, tableColumn, row, mouseLocation);
            }
#else
            /// <summary>
            /// Gets the tool tip to display in the ROMs feature list image cell.
            /// </summary>
            /// <param name="tableView">The <see cref="NSTableView"/> whose cell tool tip is desired.</param>
            /// <param name="cell">The cell in the table whose tool tip is desired.</param>
            /// <param name="rect">The proposed active area of the tool tip. This can be modified.</param>
            /// <param name="tableColumn">The column in the table where <paramref name="cell"/> is located.</param>
            /// <param name="row">The index of the row in the table.</param>
            /// <param name="mouse">The mouse position.</param>
            /// <returns>The tool tip. Return <c>null</c> for no tool tip.</returns>
            /// <remarks>This binding isn't present, so provide it. Note that, to be on the safe side, we'll return an empty string instead of <c>null</c>.
            /// Often using a C# <c>null</c> results in Bad Things when crossing back to the unmanaged realm.</remarks>
            [OSExport("tableView:toolTipForCell:rect:tableColumn:row:mouseLocation:")]
            public NSString ToolTipForCell(NSTableView tableView, NSCell cell, ref System.Drawing.RectangleF rect, NSTableColumn tableColumn, nint row, System.Drawing.PointF mouse)
            {
                return GetToolTip(cell, rect, tableColumn, row, mouse);
            }
#endif // __UNIFIED__

            /// <summary>
            /// Cancels editing the cell being edited, if any.
            /// </summary>
            internal void CancelEdit()
            {
                if (InPlaceEditor != null)
                {
                    InPlaceEditor.CancelEdit();
                }
            }

            private NSString GetToolTip(NSCell cell, CGRect rect, NSTableColumn tableColumn, nint row, CGPoint mouse)
            {
                var programDescription = Programs.ArrangedObjects()[row] as ProgramDescriptionViewModel;
                var toolTip = string.Empty;
                if (cell is NSImageCell)
                {
                    if (tableColumn.Identifier == "icon")
                    {
                        toolTip = programDescription.RomFileStatus;
                    }
                    else if (tableColumn.Identifier == "features")
                    {
                        var space = INTV.Shared.Converter.ProgramFeaturesToImageTransformer.Padding;
                        var offsetIntoImage = mouse.X - rect.X;
                        nfloat leftEdgeOfImage = 0;
                        for (int i = 0; string.IsNullOrEmpty(toolTip) && (i < programDescription.Features.Count); ++i)
                        {
                            var feature = programDescription.Features[i];
                            var rightEdgeOfImage = leftEdgeOfImage + feature.Image.Size.Width + (space / 2);
                            if ((leftEdgeOfImage <= offsetIntoImage) && (offsetIntoImage <= rightEdgeOfImage))
                            {
                                toolTip = feature.ToolTip;
                            }
                            else
                            {
                                leftEdgeOfImage += feature.Image.Size.Width + space;
                            }
                        }
                    }
                }
                // By leaving the string as empty, we get default behavior for tool tip, which is preferred.
                // It will display the full text only when it's too long to show in the cell.
#if false
                else if (tableColumn.Identifier == "name")
                {
                    toolTip = programDescription.Name;
                }
                else if (tableColumn.Identifier == "vendor")
                {
                    toolTip = programDescription.Vendor;
                }
#endif // false
                else if (tableColumn.Identifier == "romFile")
                {
                    toolTip = programDescription.RomFile;
                }
                return new NSString(toolTip.SafeString());
            }

            private void InPlaceEditor_EditorClosed(object sender, InPlaceEditorClosedEventArgs e)
            {
                InPlaceEditor.EditorClosed -= InPlaceEditor_EditorClosed;
                // InPlaceEditor = null;
            }
        }
    }

    /// <summary>
    /// Subclass NSTableView to get context menus to work the way we want.
    /// </summary>
    [Register("ROMsTableView")]
    public class ROMsTableView : NSTableView
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public ROMsTableView(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public ROMsTableView(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the controller of the table.
        /// </summary>
        internal RomListViewController Controller { get; set; }

        #endregion // Properties

        /// <inheritdoc/>
        public override NSMenu MenuForEvent(NSEvent theEvent)
        {
            // Handy advice found here:
            // http://forums.macrumors.com/threads/right-clicks-control-clicks-and-contextual-menus.166469/
#if false
            //Find which row is under the cursor
            [[self window] makeFirstResponder:self];
            NSPoint menuPoint = [self convertPoint:[event locationInWindow] fromView:nil];
            int row = [self rowAtPoint:menuPoint];

            /* Update the table selection before showing menu
               Preserves the selection if the row under the mouse is selected (to allow for
               multiple items to be selected), otherwise selects the row under the mouse */
            BOOL currentRowIsSelected = [[self selectedRowIndexes] containsIndex:row];
            if (!currentRowIsSelected)
                [self selectRow:row byExtendingSelection:NO];

            if ([self numberOfSelectedRows] <=0)
            {
                //No rows are selected, so the table should be displayed with all items disabled
                NSMenu* tableViewMenu = [[self menu] copy];
                int i;
                for (i=0;i<[tableViewMenu numberOfItems];i++)
                    [[tableViewMenu itemAtIndex:i] setEnabled:NO];
                return [tableViewMenu autorelease];
            }
            else
                return [self menu];
#endif // false
            // Select the row we got context click for.
            Window.MakeFirstResponder(this);
            var menuPoint = ConvertPointFromView(theEvent.LocationInWindow, null);
            var row = GetRow(menuPoint);
            if (row >= 0)
            {
                var rowAlreadySelected = SelectedRows.Contains((uint)row);
                if (!rowAlreadySelected)
                {
                    SelectRow(row, false);
                }
            }

            // Build the context menu.
            var target = (row < 0) ? null : Controller.GetObjectAtRow((int)row);
            var context = Controller.View.ViewModel;
            Menu = target.CreateContextMenu("ROMListContextMenu", context);
            return base.MenuForEvent(theEvent);
        }

        /// <inheritdoc/>
        public override bool PerformKeyEquivalent(NSEvent theEvent)
        {
            var didIt = base.PerformKeyEquivalent(theEvent);
            if (!didIt)
            {
                var deleteCommand = RomListCommandGroup.RemoveRomsCommand;
                var context = RomListCommandGroup.Group.Context;
                didIt = this.PerformKeyEquivalentForDelete(theEvent, deleteCommand, context);
            }
            return didIt;
        }

        /// <summary>
        /// Called when ESC or Cmd+. pressed. Overridden to get rid of annoying beep.
        /// </summary>
        /// <param name="sender">Sender.</param>
        [Export("cancelOperation:")]
        public void CancelOperation(NSObject sender)
        {
        }
    }

    /// <summary>
    /// Work around limitations in the MonoMac NSTableView bindings.
    /// </summary>
    internal static class NSTableViewHelpers
    {
        private static System.IntPtr selEditColumnRowWithEventSelect_Handle = Selector.GetHandle("editColumn:row:withEvent:select:");
#if __UNIFIED__
        [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void void_objc_msgSend_nint_nint_IntPtr_bool(System.IntPtr receiver, System.IntPtr selector, nint arg1, nint arg2, System.IntPtr arg3, bool arg4);
#endif // __UNIFIED__

        /// <summary>
        /// Start editing a cell in a given row and column. Yeah, the name is a little off.
        /// </summary>
        /// <param name="table">The <see cref=">NSTable"/> in which the edit operation is to occur.</param>
        /// <param name="column">The column number of the cell to edit.</param>
        /// <param name="row">The row number of the cell to edit.</param>
        internal static void EditColumn(this NSTableView table, nint column, nint row)
        {
            NSApplication.EnsureUIThread();
#if __UNIFIED__
            void_objc_msgSend_nint_nint_IntPtr_bool(table.Handle, selEditColumnRowWithEventSelect_Handle, column, row, System.IntPtr.Zero, true);
#else
            Messaging.void_objc_msgSend_int_int_IntPtr_bool(table.Handle, selEditColumnRowWithEventSelect_Handle, column, row, System.IntPtr.Zero, true);
#endif // __UNIFIED__
        }
    }
}
