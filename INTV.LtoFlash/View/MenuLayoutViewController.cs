// <copyright file="MenuLayoutViewController.Mac.cs" company="INTV Funhouse">
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

////#define ENABLE_DELEGATE_TRACE
////#define DEBUG_DRAGDROP
////#define ENABLE_DRAGDROP_TRACE
#define ENABLE_MULTIPLE_SELECTION

using System.Collections.Generic;
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
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;
using INTV.Shared.Behavior;

#if __UNIFIED__
using CGPoint = CoreGraphics.CGPoint;
using CGRect = CoreGraphics.CGRect;
using nfloat = System.nfloat;
using nint = System.nint;
#else
using CGPoint = System.Drawing.PointF;
using CGRect = System.Drawing.RectangleF;
using nfloat = System.Single;
using nint = System.Int32;
#endif // __UNIFIED__

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Used to indicate which column in the view should begin editing.
    /// </summary>
    internal enum EditableOutlineViewColumn
    {
        /// <summary>
        /// Invalid column index value.
        /// </summary>
        None = -1,

        /// <summary>
        /// Index of the long name column in the outline view.
        /// </summary>
        LongName = 1,

        /// <summary>
        /// Index of the short name column in the outline view.
        /// </summary>
        ShortName,
    }

    public partial class MenuLayoutViewController : NSViewController
    {
        private const string EnableMultiSelectEnvironmentVariableName = "LUI_ENABLE_MENU_LAYOUT_MULTISELECT";

        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public MenuLayoutViewController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public MenuLayoutViewController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public MenuLayoutViewController()
            : base("MenuLayoutView", NSBundle.MainBundle)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        #region Properties

        #region NSViewSController

        /// <summary>
        /// Gets the view as a strongly typed value.
        /// </summary>
        public new MenuLayoutView View { get { return (MenuLayoutView)base.View; } }

        #endregion // NSViewController

        /// <summary>
        /// Gets the status 
        /// </summary>
        [INTV.Shared.Utility.OSExport("Status")]
        public NSString Status { get; private set; }

        /// <summary>
        /// Gets a string showing overall usage as a percentage.
        /// </summary>
        [OSExport(MenuLayoutViewModel.OverallInUseRatioPropertyName)]
        public string OverallInUseRatio { get { return string.Format("{0} {1:P2}", Resources.Strings.MenuLayout_StorageUsed, LtoFlashViewModel.HostPCMenuLayout.OverallInUseRatio); } }

        /// <summary>
        /// Gets a string indicating the overall usage in detail.
        /// </summary>
        [OSExport(MenuLayoutViewModel.OverallUsageDetailsPropertyName)]
        public string OverallUsageDetails { get { return LtoFlashViewModel.HostPCMenuLayout.OverallUsageDetails; } }

        /// <summary>
        /// Gets a value representing the overall space usage on the device.
        /// </summary>
        [OSExport("OverallInUsePercent")]
        public double OverallInUsePercent { get { return LtoFlashViewModel.HostPCMenuLayout.OverallInUseRatio * 100; } }

        /// <summary>
        /// Gets whether to show the hint text.
        /// </summary>
        [INTV.Shared.Utility.OSExport("DropProgramsHereHintText")]
        public string DropProgramsHereHintText { get { return LtoFlashViewModel.HostPCMenuLayout.OverlayText; } }

        /// <summary>
        /// Gets whether to show the update-in-progress overlay text.
        /// </summary>
        [INTV.Shared.Utility.OSExport(MenuLayoutViewModel.ShowOverlayPropertyName)]
        public bool ShowOverlay { get { return LtoFlashViewModel.HostPCMenuLayout.ShowOverlay; } }

        /// <summary>
        /// Gets or sets the LtoFlashViewModel needed by the implementation.
        /// </summary>
        public LtoFlashViewModel LtoFlashViewModel { get; set; }

        private MenuLayoutViewModel ViewModel { get; set; }

        private NSPopUpButton EcsCompatibilityButton { get; set; }

        private NSPopUpButton IntyIICompatibilityButton { get; set; }

        /// <summary>
        /// Gets or sets the delegate. (General paranoia about MonoMac's NSObject lifetime management.
        /// </summary>
        private OutlineViewDelegate TheDelegate { get; set; }

        /// <summary>
        /// Gets or sets the data source. (General paranoia about MonoMac's NSObject lifetime management.
        /// </summary>
        private OutlineViewDataSource TheDataSource { get; set; }

        #endregion // Properties

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            DropFilesHereText.AlphaValue = 0.25f;
            View.Controller = this;
            View.PropertyChanged += HandleDataContextChanged;
            this.SetValue(MenuLayoutCommandGroup.NewDirectoryCommand.UniqueId, NewFolderButton);
            this.SetValue(MenuLayoutCommandGroup.DeleteItemsCommand.UniqueId, RemoveItemButton);
            this.SetValue(MenuLayoutCommandGroup.SetColorCommand.UniqueId, ItemColorWell);
            this.InitializeCommandsInAwakeFromNib();
            var outlineView = View.FindChild<MenuOutlineView>();
            TheDelegate = new OutlineViewDelegate(MenuLayoutController, LtoFlashViewModel);
            TheDataSource = new OutlineViewDataSource(MenuLayoutController);
            outlineView.Controller = this;
            outlineView.Delegate = TheDelegate;
            outlineView.DataSource = TheDataSource;
            outlineView.RegisterForDraggedTypes(new string[] { MenuLayoutViewModel.DragDataFormat, ProgramDescriptionViewModel.DragDataFormat });
            outlineView.SetDraggingSourceOperationMask(NSDragOperation.Move, true);

            DeviceCommandGroup.Group.InitializeConnectionMenu(LtoFlashViewModel);

            PowerStateIcon.Image = typeof(CommandGroup).LoadImageResource("ViewModel/Resources/Images/console_16xLG.png");
            PowerStateIcon.ToolTip = LtoFlashViewModel.ActiveLtoFlashDevice.PowerState;
            PowerStateIcon.Enabled = LtoFlashViewModel.ActiveLtoFlashDevice.IsValid && LtoFlashViewModel.ActiveLtoFlashDevice.Device.HardwareStatus.HasFlag(HardwareStatusFlags.ConsolePowerOn);
            ConnectionIcon.Image = this.LoadImageResource("Resources/Images/connected_16x16.png");
            ConnectionIcon.ToolTip = LtoFlashViewModel.ActiveLtoFlashDevice.DisplayName;
            ConnectionIcon.Enabled = LtoFlashViewModel.ActiveLtoFlashDevice.IsValid;
            MenuDifferencesIcon.Hidden = !LtoFlashViewModel.ShowFileSystemsDifferIcon;
            MenuDifferencesIcon.Image = this.LoadImageResource("Resources/Images/lto_flash_contents_not_in_sync_16xLG.png");
        }

        /// <summary>
        /// Override the default behavior of ValidateToolbarItem.
        /// </summary>
        /// <param name="toolbarItem">The toolbar item to validate.</param>
        /// <returns><c>true</c> if the item is valid, and should be enabled.</returns>
        /// <remarks>TODO: Unsure why this is here... is it needed, or leftover from some experiment?</remarks>
        [Export ("validateToolbarItem:")]
        public bool ValidateToolbarItem(NSToolbarItem toolbarItem)
        {
            return false;
        }

        /// <summary>
        /// Invokes the in-place editor for a specific column in the menu view.
        /// </summary>
        /// <param name="column">Which column to edit.</param>
        internal void EditMenuItemName(EditableOutlineViewColumn column)
        {
            var outlineView = View.FindChild<NSOutlineView>();
            var row = outlineView.SelectedRow;
            var col = outlineView.TableColumns()[(int)column];
            var d = outlineView.Delegate;
            var n = MenuLayoutController.SelectedNodes[0];
            if (d.ShouldEditTableColumn(outlineView, col, n))
            {
                outlineView.EditColumn((nint)(int)column, row);
            }
        }

        private void HandleDataContextChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == INTV.Shared.View.IFakeDependencyObjectHelpers.DataContextPropertyName)
            {
                var viewModel = View.DataContext as INTV.LtoFlash.ViewModel.MenuLayoutViewModel;
                ViewModel = viewModel;
                viewModel.PropertyChanged += HandleMenuLayoutPropertyChanged;
                LtoFlashViewModel.PropertyChanged += HandleLtoFlashPropertyChanged;
                HandleMenuLayoutPropertyChanged(viewModel, new System.ComponentModel.PropertyChangedEventArgs(FolderViewModel.StatusPropertyName));
                var selectsInsertedObjects = MenuLayoutController.SelectsInsertedObjects;
                MenuLayoutController.SelectsInsertedObjects = false;
                MenuLayoutController.SetContent(viewModel.Root);
                MenuLayoutController.SelectsInsertedObjects = selectsInsertedObjects;
                var outlineView = View.FindChild<NSOutlineView>();
                outlineView.ExpandItem(outlineView.ItemAtRow(0));
                HandleMenuLayoutPropertyChanged(viewModel, new System.ComponentModel.PropertyChangedEventArgs(MenuLayoutViewModel.FolderCountPropertyName));
            }
        }

        private DeviceViewModel _activeDeviceViewModel;

        private void HandleLtoFlashPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case LtoFlashViewModel.ActiveLtoFlashDevicePropertyName:
                    if (_activeDeviceViewModel != null)
                    {
                        _activeDeviceViewModel.PropertyChanged -= HandleActiveDevicePropertyChanged;
                    }
                    _activeDeviceViewModel = LtoFlashViewModel.ActiveLtoFlashDevice;
                    if (_activeDeviceViewModel != null)
                    {
                        _activeDeviceViewModel.PropertyChanged += HandleActiveDevicePropertyChanged;
                    }
                    break;
                case LtoFlashViewModel.ShowFileSystemsDifferIconPropertyName:
                    MenuDifferencesIcon.ToolTip = LtoFlashViewModel.ContentsNotInSyncToolTip;
                    MenuDifferencesIcon.Hidden = !LtoFlashViewModel.ShowFileSystemsDifferIcon;
                    break;
            }
        }

        private void HandleActiveDevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!OSDispatcher.IsMainThread)
            {
                this.InvokeOnMainThread(() => HandleActiveDevicePropertyChanged(sender, e));
                return;
            }
            switch (e.PropertyName)
            {
                case "PowerState":
                    PowerStateIcon.ToolTip = LtoFlashViewModel.ActiveLtoFlashDevice.PowerState;
                    PowerStateIcon.Enabled = LtoFlashViewModel.ActiveLtoFlashDevice.IsValid && LtoFlashViewModel.ActiveLtoFlashDevice.Device.HardwareStatus.HasFlag(HardwareStatusFlags.ConsolePowerOn);
                    break;
                case Device.IsValidPropertyName:
                    ConnectionIcon.Enabled = LtoFlashViewModel.ActiveLtoFlashDevice.IsValid;
                    break;
                case DeviceViewModel.DisplayNamePropertyName:
                    ConnectionIcon.ToolTip = LtoFlashViewModel.ActiveLtoFlashDevice.DisplayName;
                    break;
                case Device.EcsCompatibilityPropertyName:
                    if (EcsCompatibilityButton != null)
                    {
                        EcsCompatibilityButton.SelectItemWithTag((byte)LtoFlashViewModel.ActiveLtoFlashDevice.EcsCompatibility);
                    }
                    break;
                case Device.IntvIICompatibilityPropertyName:
                    if (IntyIICompatibilityButton != null)
                    {
                        IntyIICompatibilityButton.SelectItemWithTag((byte)LtoFlashViewModel.ActiveLtoFlashDevice.IntvIICompatibility);
                    }
                    break;
            }
        }

        private void HandleMenuLayoutPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case FolderViewModel.StatusPropertyName:
                    this.WillChangeValue(e.PropertyName);
                    Status = new NSString(ViewModel.Status);
                    this.DidChangeValue(e.PropertyName);
                    UpdateDropProgramsHint();
                    break;
                case MenuLayoutViewModel.OverlayTextPropertyName:
                case MenuLayoutViewModel.ShowOverlayPropertyName:
                    this.WillChangeValue(e.PropertyName);
                    this.DidChangeValue(e.PropertyName);
                    this.WillChangeValue("DropProgramsHereHintText");
                    this.DidChangeValue("DropProgramsHereHintText");
                    break;
                case MenuLayoutViewModel.CurrentSelectionPropertyName:
                    if (ViewModel.CurrentSelection == null)
                    {
                        MenuLayoutController.SelectionIndexPath = new NSIndexPath();
                        CommandManager.InvalidateRequerySuggested();
                    }
                    else
                    {
                        var indexPath = new List<int>();
                        var currentSelection = ViewModel.CurrentSelection.Model;
                        var parent = currentSelection.Parent;
                        while (parent != null)
                        {
                            var index = parent.IndexOfChild(currentSelection);
                            indexPath.Add(index);
                            currentSelection = parent;
                            parent = parent.Parent;
                        }
                        // add the root
                        indexPath.Add(0);
                        indexPath.Reverse();
                        var newSelectionIndexPath = NSIndexPath.Create(indexPath.ToArray());
                        var currentSelectionIndexPath = MenuLayoutController.SelectionIndexPath;
                        if ((currentSelectionIndexPath == null) || (newSelectionIndexPath.Compare(currentSelectionIndexPath) != 0))
                        {
                            if (NSApplication.SharedApplication.MainWindow.MakeFirstResponder(View.FindChild<NSOutlineView>()))
                            {
                                MenuLayoutController.SelectionIndexPath = newSelectionIndexPath;
                            }
                        }
                    }
                    break;
                case MenuLayoutViewModel.FolderCountPropertyName:
                case MenuLayoutViewModel.FileCountPropertyName:
                case MenuLayoutViewModel.ForkCountPropertyName:
                    break;
                case MenuLayoutViewModel.OverallInUseRatioPropertyName:
                case MenuLayoutViewModel.OverallUsageDetailsPropertyName:
                    this.RaiseChangeValueForKey(e.PropertyName);
                    this.RaiseChangeValueForKey("OverallInUsePercent");
                    break;
            }
            if (OSDispatcher.IsMainThread)
            {
                View.NeedsDisplay = true;
                RemoveItemButton.ToolTip = ViewModel.DeleteSelectedItemTip.SafeString();
            }
        }

        private void UpdateDropProgramsHint()
        {
            var anyItems = ViewModel.ItemCount > 0;
            var outlineView = View.FindChild<NSOutlineView>();
            if (anyItems)
            {
                MenuTreeScrollView.AlphaValue = 1;
                outlineView.Enabled = true;
                DropFilesHereText.Hidden = true;
            }
            else
            {
                MenuTreeScrollView.AlphaValue = 0.25f;
                outlineView.Enabled = false;
                DropFilesHereText.Hidden = false;
            }
        }

        partial void OnDoubleClick(NSObject sender)
        {
            var selectedItemPaths = sender as NSArray;
            if (selectedItemPaths != null)
            {
                var mouseLocation = NSEvent.CurrentMouseLocation; // if multiple items selected, get the one that was double-clicked
                var outline = View.FindChild<MenuOutlineView>();
                var rect = outline.Window.ConvertRectFromScreen(new CGRect(mouseLocation.X, mouseLocation.Y, 0, 0));
                rect = outline.ConvertRectFromView(rect, null);
                var row = outline.GetRow(new CGPoint(rect.X, rect.Y));
                if (row >= 0)
                {
                    var item = outline.ItemAtRow(row) as NSTreeNode;
                    if (item.IsLeaf && DownloadCommandGroup.DownloadAndPlayCommand.CanExecute(LtoFlashViewModel))
                    {
                        // Should this be changed to the commented out code below (if it even compiles)?
                        var program = item.RepresentedObject as ProgramViewModel;
                        if (program != null)
                        {
                            DownloadCommandGroup.DownloadAndPlay(LtoFlashViewModel.ActiveLtoFlashDevice.Device, program.ProgramDescription);
                        }
                        // (steveno) IIRC the intent here may be to go to the more generic code from INTV.Shared that tries to cope with multiple
                        // possible actions being available on a double-click. For example, what if you have Intellicart, CC3 and LTO Flash! all
                        // available at the same time?
                        ////var programs = NSArray.FromArray<ProgramDescriptionViewModel>(arrangedObjectsArray);
                        ////var doubleClickedProgram = programs[row];
                        ////DebugItemChange(doubleClickedProgram.Name);
                        ////View.ViewModel.Model.InvokeProgramFromDescription(doubleClickedProgram.Model);
                    }
                    else
                    {
                        if (outline.IsItemExpanded(item))
                        {
                            outline.CollapseItem(item);
                        }
                        else if (outline.IsExpandable(item))
                        {
                            outline.ExpandItem(item);
                        }
                    }
                }
            }
        }

        [System.Diagnostics.Conditional("ENABLE_DRAGDROP_TRACE")]
        private static void DebugDragDropPrint(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// Sublcass NSOutlineView to get context menus and other desired features.
        /// </summary>
        [Register ("MenuOutlineView")]
        public class MenuOutlineView : NSOutlineView
        {
            #region Constructors

            /// <summary>
            /// Called when created from unmanaged code.
            /// </summary>
            /// <param name="handle">Native pointer to NSView.</param>
            public MenuOutlineView(System.IntPtr handle)
                : base(handle)
            {
                Initialize();
            }

            /// <summary>
            /// Called when created directly from a XIB file.
            /// </summary>
            /// <param name="coder">Used to deserialize from a XIB.</param>
            [Export("initWithCoder:")]
            public MenuOutlineView(NSCoder coder)
                : base(coder)
            {
                Initialize();
            }

            /// <summary>Shared initialization code.</summary>
            private void Initialize()
            {
#if ENABLE_MULTIPLE_SELECTION
                var allowsMultipleSelection = true;
                try
                {
                    var allowsMultipleSelectionString = NSBundle.MainBundle.GetEnvironmentValue<string>(EnableMultiSelectEnvironmentVariableName);
                    if (!string.IsNullOrEmpty(allowsMultipleSelectionString))
                    {
                        bool environmentValue;
                        if (!bool.TryParse(allowsMultipleSelectionString, out environmentValue))
                        {
                            allowsMultipleSelection = string.Compare(allowsMultipleSelectionString, "yes", true) == 0;
                        }
                        else
                        {
                            allowsMultipleSelection = environmentValue;
                        }
                    }
                }
                catch (System.Exception)
                {
                    // If anything fails, just leave things to behave as default.
                }
                AllowsMultipleSelection = allowsMultipleSelection;
#endif // ENABLE_MULTIPLE_SELECTION
            }

            #endregion // Constructors

            /// <summary>
            /// Access to our controller. Too bad this isn't more like the iOS APIs where the responder chain is more clearly defined. This wouldn't be necessary then.
            /// </summary>
            internal MenuLayoutViewController Controller { get; set; }

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
                Window.MakeFirstResponder(this);
                // Select the clicked item when we get context menu click.
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
                var treeNode = (row < 0) ? null : ItemAtRow(row) as NSTreeNode;
                var target = (row < 0) ? null : treeNode.RepresentedObject as FileNodeViewModel;
                var context = Controller.LtoFlashViewModel.HostPCMenuLayout;
                Menu = target.CreateContextMenu("MenuEditorContextMenu", context);
                return base.MenuForEvent(theEvent);
            }

#if DEBUG_DRAGDROP
            /// <inheritdoc/>
            public override bool PerformDragOperation(NSDraggingInfo sender)
            {
                DebugDragDropPrint("**** MENUOUTLINEVIEW PERFORM DRAG OP");
                return base.PerformDragOperation(sender);
            }

            /// <inheritdoc/>
            public override bool PrepareForDragOperation(NSDraggingInfo sender)
            {
                DebugDragDropPrint("**** MENUOUTLINEVIEW PREPARE FOR DRAG OP");
                return base.PrepareForDragOperation(sender);
            }

            /// <inheritdoc/>
            public override NSDraggingSession BeginDraggingSession(NSDraggingItem[] itmes, NSEvent evnt, NSDraggingSource source)
            {
                DebugDragDropPrint("**** MENUOUTLINEVIEW DRAGGING BEGAN");
                return base.BeginDraggingSession(itmes, evnt, source);
            }

            /// <inheritdoc/>
            public override void ConcludeDragOperation(NSDraggingInfo sender)
            {
                DebugDragDropPrint("**** MENUOUTLINEVIEW DRAGGING CONCLUCDE");
                base.ConcludeDragOperation(sender);
            }

            /// <inheritdoc/>
            public override NSDragOperation DraggingEntered(NSDraggingInfo sender)
            {
                DebugDragDropPrint("**** MENUOUTLINEVIEW DRAGGING ENTERED");
                return base.DraggingEntered(sender);
            }

            /// <inheritdoc/>
            public override NSDragOperation DraggingUpdated(NSDraggingInfo sender)
            {
                DebugDragDropPrint("**** MENUOUTLINEVIEW DRAGGING UPDATED");
                return base.DraggingUpdated(sender);
            }

            /// <inheritdoc/>
            public override void DraggingExited(NSDraggingInfo sender)
            {
                DebugDragDropPrint("**** MENUOUTLINEVIEW DRAGGING EXITED");
                base.DraggingExited(sender);
            }
#endif // DEBUG_DRAGDROP

            /// <inheritdoc/>
            public override void DraggingEnded(NSDraggingInfo sender)
            {
                DebugDragDropPrint("**** MYENUOUTLINEVIEW DRAGGING ENDED");
                ////base.DraggingEnded(sender);
                DragDropHelpers.FinishedWithPasteboard(sender.DraggingPasteboard);
            }

            /// <inheritdoc/>
            public override bool PerformKeyEquivalent(NSEvent theEvent)
            {
                var didIt = base.PerformKeyEquivalent(theEvent);
                if (!didIt)
                {
                    var deleteCommand = MenuLayoutCommandGroup.DeleteItemsCommand;
                    var context = MenuLayoutCommandGroup.Group.Context;
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
        /// Specialized delegate to get the desired behaviors for the menu layout editor.
        /// </summary>
        private class OutlineViewDelegate : NSOutlineViewDelegate
        {
            private NSTreeController TreeData { get; set; }
            private LtoFlashViewModel ViewModel { get; set; }
            private EditableOutlineViewColumn EditingColumn { get; set; }

            private TextCellInPlaceEditor InPlaceEditor { get; set; }

            private double ReturnKeyTimestamp { get; set; }

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the type OutlineViewDelegate.
            /// </summary>
            /// <param name="treeData"></param>
            /// <param name="viewModel"></param>
            internal OutlineViewDelegate(NSTreeController treeData, LtoFlashViewModel viewModel)
            {
                TreeData = treeData;
                ViewModel = viewModel;
            }

            #endregion // Constructors

            /// <inheritdoc />
            public override nfloat GetRowHeight(NSOutlineView outlineView, NSObject item)
            {
                float height = 20;
                var rowForItem = outlineView.RowForItem(item);
                if (rowForItem == 0)
                {
                    ////outlineView.IndentationPerLevel = 0;
                    height = float.Epsilon; // can't be zero, but we don't want to see it.
                }
                else
                {
                    ////outlineView.IndentationPerLevel = 16;
                }
                return height;
            }

            /// <inheritdoc />
            public override bool SelectionShouldChange (NSOutlineView outlineView)
            {
                var shouldChange = !SingleInstanceApplication.Instance.IsBusy;
                DebugDelegatePrint("OutlineViewDelegate: selectionShouldChange: " + shouldChange);
                return shouldChange;
            }

            /// <inheritdoc />
            /// <remarks>WHY DOES THIS NOT GET CALLED WHEN LAST ITEM IN ROOT IS DELETED???</remarks>
            public override void SelectionDidChange(NSNotification notification)
            {
                DebugDelegatePrint("OutlineViewDelegate: selectionChanged in Delegate");
                var outlineView = notification.Object as NSOutlineView;
                var selectedObjects = TreeData.SelectedObjects;
                var selectedItem = selectedObjects.FirstOrDefault() as FileNodeViewModel;
                var selectedItems = selectedObjects.OfType<FileNodeViewModel>().Where(i => !(i is MenuLayoutViewModel));
                var viewModel = outlineView.GetInheritedValue<MenuLayoutViewModel>(IFakeDependencyObjectHelpers.DataContextPropertyName);
                if (selectedItem == viewModel.Root || ((selectedItem != null) && (selectedItem.Parent == null)))
                {
                    selectedItem = null;
                }
                viewModel.CurrentSelection = selectedItem;
                var itemsToRemove = viewModel.SelectedItems.Except(selectedItems).ToArray();
                foreach (var itemToRemove in itemsToRemove)
                {
                    viewModel.SelectedItems.Remove(itemToRemove);
                }
                var itemsToAdd = selectedItems.Except(viewModel.SelectedItems);
                foreach (var itemToAdd in itemsToAdd)
                {
                    viewModel.SelectedItems.Add(itemToAdd);
                }
                CommandManager.InvalidateRequerySuggested(); // ensure command availability is updated
            }

            /// <inheritdoc />
            public override bool ShouldExpandItem(NSOutlineView outlineView, NSObject item)
            {
                DebugDelegatePrint("********** OUTLINE DELEGATE SHOULD EXPAND ITEM");
                return true;
            }

            /// <inheritdoc />
            public override bool ShouldEditTableColumn(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
            {
                DebugDelegatePrint("***************** SHOULD EDIT CALLED");
                var canEdit = MenuLayoutCommandGroup.EditLongNameCommand.CanExecute(ViewModel.HostPCMenuLayout);
                if (canEdit)
                {
                    var treeNode = item as NSTreeNode;
                    EditingColumn = (EditableOutlineViewColumn)outlineView.TableColumns().ToList().IndexOf(tableColumn);
                    // EditingObject = treeNode.RepresentedObject as FileNodeViewModel;
                    var editingObject = treeNode.RepresentedObject as FileNodeViewModel;
                    string initialValue = null;
                    int maxLength = -1;
                    switch (EditingColumn)
                    {
                        case EditableOutlineViewColumn.LongName:
                            canEdit = true;
                            initialValue = editingObject.LongName;
                            maxLength = INTV.LtoFlash.Model.FileSystemConstants.MaxLongNameLength;
                            break;
                        case EditableOutlineViewColumn.ShortName:
                            canEdit = true;
                            initialValue = editingObject.ShortName;
                            maxLength = INTV.LtoFlash.Model.FileSystemConstants.MaxShortNameLength;
                            break;
                        default:
                            ErrorReporting.ReportError("Unsupported edit column");
                            break;
                    }
                    if (InPlaceEditor == null)
                    {
                        InPlaceEditor = new TextCellInPlaceEditor(outlineView);
                    }
                    InPlaceEditor.EditingObject = editingObject;
                    InPlaceEditor.InitialValue = initialValue;
                    InPlaceEditor.MaxLength = maxLength;
                    InPlaceEditor.IsValidCharacter = (c) => INTV.Core.Model.Grom.Characters.Contains(c); //.RestrictToGromCharacters = true;
                    InPlaceEditor.EditorClosed += InPlaceEditor_EditorClosed;
                    InPlaceEditor.BeginEdit();
                }
                else if ((SingleInstanceApplication.Current.LastKeyPressed == 0x24) && (SingleInstanceApplication.Current.LastKeyPressedTimestamp != ReturnKeyTimestamp))
                {
                    // return was pressed
                    ReturnKeyTimestamp = SingleInstanceApplication.Current.LastKeyPressedTimestamp;
                    if (!outlineView.IsExpandable(item) && DownloadCommandGroup.DownloadAndPlayCommand.CanExecute(ViewModel))
                    {
                        var program = ViewModel.HostPCMenuLayout.CurrentSelection as ProgramViewModel;
                        if ((program != null) && DownloadCommandGroup.DownloadAndPlayCommand.CanExecute(ViewModel))
                        {
                            DownloadCommandGroup.DownloadAndPlay(ViewModel.ActiveLtoFlashDevice.Device, program.ProgramDescription);
                        }
                    }
                    else
                    {
                        // On a directory. If so, toggle state.
                        if (outlineView.IsItemExpanded(item))
                        {
                            outlineView.CollapseItem(item);
                        }
                        else
                        {
                            outlineView.ExpandItem(item);
                        }
                    }
                }
                return canEdit;
            }

            /// <inheritdoc />
            public override bool ShouldSelectItem(NSOutlineView outlineView, NSObject item)
            {
                var rowForItem = outlineView.RowForItem(item);
                return rowForItem != 0;
            }

            /// <inheritdoc />
            public override bool ShouldShowOutlineCell(NSOutlineView outlineView, NSObject item)
            {
                var rowForItem = outlineView.RowForItem(item);
                return rowForItem > 0;
            }

            /// <inheritdoc />
            public override string ToolTipForCell(NSOutlineView outlineView, NSCell cell, ref CGRect rect, NSTableColumn tableColumn, NSObject item, CGPoint mouseLocation)
            {
                var toolTip = string.Empty;
                var treeNode = item as NSTreeNode;
                var mouseOverObject = treeNode.RepresentedObject as FileNodeViewModel;
                var whichColumn = outlineView.TableColumns().ToList().IndexOf(tableColumn);
                switch (whichColumn)
                {
                    case 0:
                        if ((mouseOverObject != null) && !string.IsNullOrEmpty(mouseOverObject.IconTipStrip))
                        {
                            toolTip = mouseOverObject.IconTipStrip;
                        }
                        break;
                }
                return toolTip;
            }

            private void InPlaceEditor_EditorClosed (object sender, InPlaceEditorClosedEventArgs e)
            {
                InPlaceEditor.EditorClosed -= InPlaceEditor_EditorClosed;
                // InPlaceEditor = null;
            }

            [System.Diagnostics.Conditional("ENABLE_DELEGATE_TRACE")]
            private static void DebugDelegatePrint(object message)
            {
                System.Diagnostics.Debug.WriteLine(message);
            }
        }

        /// <summary>
        /// Custom data source implementation for the menu editor. Specifically, this is used for drag/drop operations.
        /// </summary>
        private class OutlineViewDataSource : NSOutlineViewDataSource
        {
            private static readonly string[] MenuLayoutPasteboardDataTypeArray = new string[] { MenuLayoutViewModel.DragDataFormat };
            private static readonly string[] ProgramDescriptionPasteboardDataTypeArray = new string[] { ProgramDescriptionViewModel.DragDataFormat };
            public OutlineViewDataSource(NSTreeController treeData)
            {
                TreeData = treeData;
            }

#if false // The following implementations are not yet needed, or are addressed by OutlineViewDelegate.
            /// <inheritdoc />
            public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
            {
                throw new System.NotImplementedException ();
            }

            /// <inheritdoc />
            public override int GetChildrenCount(NSOutlineView outlineView, NSObject item)
            {
                throw new System.NotImplementedException ();
            }

            /// <inheritdoc />
            public override NSObject GetChild(NSOutlineView outlineView, int childIndex, NSObject item)
            {
                throw new System.NotImplementedException ();
            }

            /// <inheritdoc />
            public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
            {
                throw new System.NotImplementedException ();
            }
#endif // false

            private NSTreeController TreeData { get; set; }

            /// <inheritdoc />
            public override NSObject PersistentObjectForItem(NSOutlineView outlineView, NSObject item)
            {
                return null;
            }

            /// <inheritdoc />
            public override bool OutlineViewwriteItemstoPasteboard(NSOutlineView outlineView, NSArray items, NSPasteboard pboard)
            {
                DebugDragDropPrint("***** OutlineView.OutlineViewwriteItemstoPasteboard");
#if DEBUG_DRAGDROP
                var nodes = NSArray.FromArray<NSTreeNode>(items);
#endif // DEBUG_DRAGDROP
                var draggedItems = NSArray.FromArray<NSTreeNode>(items).Select(node => node.RepresentedObject as FileNodeViewModel);
                var startDrag = draggedItems.Any();
                if (startDrag)
                {
                    DragDropHelpers.PreparePasteboard(pboard, MenuLayoutViewModel.DragDataFormat, new NSDataWrapper(draggedItems));
                }
                return startDrag;
            }

            /// <inheritdoc />
            public override bool AcceptDrop(NSOutlineView outlineView, NSDraggingInfo info, NSObject item, nint index)
            {
                DebugDragDropPrint("***** OutlineView.AcceptDrop, index: " + index);
                var dropLocationTreeNode = item as NSTreeNode;
                bool acceptedDrop = dropLocationTreeNode != null;
                var viewModel = outlineView.GetInheritedValue<MenuLayoutViewModel>(IFakeDependencyObjectHelpers.DataContextPropertyName);
                IEnumerable<FileNodeViewModel> draggedItems = null;
                if (acceptedDrop)
                {
                    var newParent = dropLocationTreeNode.GetRepresentedObject() as FolderViewModel;
                    var pasteboard = info.DraggingPasteboard;
                    acceptedDrop = newParent != null;
                    if (acceptedDrop && pasteboard.CanReadItemWithDataConformingToTypes(MenuLayoutPasteboardDataTypeArray))
                    {
                        draggedItems = DragDropHelpers.GetDataForType<IEnumerable<FileNodeViewModel>>(pasteboard, MenuLayoutPasteboardDataTypeArray);
                        if (acceptedDrop)
                        {
                            acceptedDrop = newParent.ShouldAcceptDraggedItems(draggedItems.Select(draggedItem => draggedItem.Model));
                        }
                        if (acceptedDrop)
                        {
                            var firstDraggedItem = draggedItems.First();
                            var parent = firstDraggedItem.Parent;
                            var parentViewModel = viewModel.FindViewModelForModel(parent);
                            if (parentViewModel == newParent)
                            {
                                var currentIndex = parent.IndexOfChild(firstDraggedItem.Model);
                                if (currentIndex < index)
                                {
                                    --index;
                                }
                            }
                            acceptedDrop = newParent.MoveItems(viewModel, newParent, (int)index, draggedItems);
                        }
                    }
                    else if (acceptedDrop && pasteboard.CanReadItemWithDataConformingToTypes(ProgramDescriptionPasteboardDataTypeArray))
                    {
                        var droppedItems = DragDropHelpers.GetDataForType<IEnumerable<ProgramDescriptionViewModel>>(pasteboard, ProgramDescriptionPasteboardDataTypeArray).Select(draggedItem => draggedItem.Model);
                        newParent.AddItems(viewModel, (int)index, droppedItems);
                    }
                    else
                    {
                        acceptedDrop = false;
                    }
                    DebugDragDropPrint("*** OutlineView drop into: " + newParent);
                }
                if (acceptedDrop)
                {
                    TreeData.RearrangeObjects();
                    if ((draggedItems != null) && draggedItems.Any())
                    {
                        viewModel.CurrentSelection = draggedItems.FirstOrDefault(i => i.Model != null);
                    }

                }
                return acceptedDrop;
            }

            /// <inheritdoc />
            public override NSDragOperation ValidateDrop(NSOutlineView outlineView, NSDraggingInfo info, NSObject proposedParentItem, nint index)
            {
                var operation = NSDragOperation.None;
                string targetName = "<NULL>";
                if ((proposedParentItem != null) && (index >= 0))
                {
                    var node = proposedParentItem as NSTreeNode;
                    var proposedParent = node == null ? null : node.GetRepresentedObject() as FileNodeViewModel;
                    if ((node != null) && (proposedParent != null))
                    {
                        var pasteboard = info.DraggingPasteboard;
                        targetName = proposedParent.LongName;
                        if (pasteboard.CanReadItemWithDataConformingToTypes(MenuLayoutPasteboardDataTypeArray))
                        {
                            // Dragging within the menu layout editor.
                            var draggedItems = DragDropHelpers.GetDataForType<IEnumerable<FileNodeViewModel>>(pasteboard, MenuLayoutPasteboardDataTypeArray);
                            if (ShouldAcceptProposedIndex(draggedItems, proposedParent, (int)index) && proposedParent.ShouldAcceptDraggedItems(draggedItems.Select(draggedItem => draggedItem.Model)))
                            {
                                operation = NSDragOperation.Move;
                            }
                        }
                        else if (pasteboard.CanReadItemWithDataConformingToTypes(ProgramDescriptionPasteboardDataTypeArray))
                        {
                            DebugDragDropPrint("OutlineView received drop of ProgramDescriptions!");
                            var draggedItems = DragDropHelpers.GetDataForType<IEnumerable<ProgramDescriptionViewModel>>(pasteboard, ProgramDescriptionPasteboardDataTypeArray);
                            IEnumerable<ProgramDescription> droppedPrograms = ((draggedItems != null) && draggedItems.Any()) ? draggedItems.Select(draggedItem => draggedItem.Model) : null;
                            if (proposedParent.ShouldAcceptDraggedItems(droppedPrograms))
                            {
                                operation = NSDragOperation.Link;
                            }
                        }
                    }
                    else
                    {
                        DebugDragDropPrint("**** OutlineView.ValidateDrop parent not NSTreeNode! Type is: " + proposedParentItem.GetType().FullName);
                    }
                }
                DebugDragDropPrint("**** OutlineView.ValidateDrop index: " + index + ", returning: " + operation);
                return operation;
            }

            private bool ShouldAcceptProposedIndex(IEnumerable<FileNodeViewModel> draggedItems, FileNodeViewModel proposedParent, int proposedIndex)
            {
                bool shouldAcceptProposedIndex = (draggedItems != null) && draggedItems.Any();
                if (shouldAcceptProposedIndex)
                {
                    var firstDraggedItem = draggedItems.First();
                    if (draggedItems.Count() == 1)
                    {
                        // prevent dropping an item on itself
                        FileNodeViewModel dropTargetItem = null;
                        if (proposedParent.Items.Count > 0)
                        {
                            dropTargetItem = (proposedIndex < proposedParent.Items.Count) ? proposedParent.Items[proposedIndex] : proposedParent.Items.LastOrDefault();
                            shouldAcceptProposedIndex = dropTargetItem != firstDraggedItem;
                        }
                    }
                }
                return shouldAcceptProposedIndex;
            }
        }
    }

    /// <summary>
    /// Bindings for methods that are either missing, or done incorrectly, for NSTableView.
    /// </summary>
    /// <remarks>TODO: Put into INTV.Shared to be generally available.</remarks>
    internal static class NSTableViewHelpers
    {
#if __UNIFIED__
        [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void void_objc_msgSend_nint_nint_IntPtr_bool(System.IntPtr receiver, System.IntPtr selector, nint arg1, nint arg2, System.IntPtr arg3, bool arg4);
#endif // __UNIFIED__

        private static System.IntPtr selEditColumnRowWithEventSelect_Handle = Selector.GetHandle("editColumn:row:withEvent:select:");

        /// <summary>
        /// Edits the cell at the specified column and row using the specified event and selection behavior.
        /// </summary>
        /// <param name="table">The <see cref="NSTableView"/> in which the cell exists.</param>
        /// <param name="column">The index of the column of the cell to edit.</param>
        /// <param name="row">The index of the row of the cell to edit.</param>
        /// <remarks>This doesn't seem to have a proper binding in MonoMac at this time. Perhaps newer versions of Xamarin.Mac have this binding. Either that, or the
        /// method fails because it won't allow passing <c>null</c> for the native method's <see cref="NSEvent"/> argument. The bindings seem to choke on that often.</remarks>
        internal static void EditColumn(this NSTableView table, nint column, nint row)
        {
            NSApplication.EnsureUIThread();
#if __UNIFIED__
            void_objc_msgSend_nint_nint_IntPtr_bool(table.Handle, selEditColumnRowWithEventSelect_Handle, column, row, System.IntPtr.Zero, true);
#else
            Messaging.void_objc_msgSend_int_int_IntPtr_bool (table.Handle, selEditColumnRowWithEventSelect_Handle, column, row, System.IntPtr.Zero, true);
#endif // __UNIFIED__
        }
    }

    /// <summary>
    /// Bindings for methods that are either missing, or done incorrectly, for NSTreeNode and NSTreeController.
    /// </summary>
    /// <remarks>TODO: Put into INTV.Shared to be generally available.</remarks>
    internal static class NSTreeNodeHelpers
    {
#if __UNIFIED__
        [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern System.IntPtr IntPtr_objc_msgSend(System.IntPtr receiver, System.IntPtr selector);
        [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void void_objc_msgSend_IntPtr(System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1);
#endif // __UNIFIED__

        private static System.IntPtr selRepresentedObjectHandle = Selector.GetHandle("representedObject");

        /// <summary>
        /// Gets the represented object in the <see cref="NSTreeNode"/>.
        /// </summary>
        /// <param name="node">The node whose represented object is desired.</param>
        /// <returns>The represented object, which must be a <see cref="NSObject"/>.</returns>
        /// <remarks>The MonoMac binding for this uses the incorrect return type.</remarks>
        internal static NSObject GetRepresentedObject(this NSTreeNode node)
        {
#if __UNIFIED__
            var representedObject = node.RepresentedObject;
#else
            var representedObject = Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend(node.Handle, selRepresentedObjectHandle));
#endif // __UNIFIED__
            return representedObject;
        }

        static System.IntPtr selSetContentObjectHandle = Selector.GetHandle("setContent:");

        /// <summary>
        /// Sets the content of the tree controller.
        /// </summary>
        /// <param name="treeController">The tree controller whose content is to be set.</param>
        /// <param name="content">The content of the tree controller.</param>
        /// <remarks>The MonoMac binding uses the incorrect type for the <paramref name="content"/> argument.</remarks>
        internal static void SetContent(this NSTreeController treeController, NSObject content)
        {
#if __UNIFIED__
            treeController.Content = content;
#else
            Messaging.void_objc_msgSend_IntPtr(treeController.Handle, selSetContentObjectHandle, content.Handle);
#endif // __UNIFIED__
        }
    }
}
