// <copyright file="FileNodeViewModel.WPF.cs" company="INTV Funhouse">
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

// #define MEASURE_MOVE_TO_PARENT

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Model;
using INTV.Shared.Behavior;
using INTV.Shared.ComponentModel;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class FileNodeViewModel
    {
        private static readonly RelayCommand DragManualEnterCommandInstance = new RelayCommand(DragManualEnter);
        private static readonly RelayCommand DragManualOverCommandInstance = new RelayCommand(DragManualOver);
        private static readonly RelayCommand DragManualDropCommandInstance = new RelayCommand(DragManualDrop);

        private static readonly RelayCommand DragSaveDataEnterCommandInstance = new RelayCommand(DragSaveDataEnter);
        private static readonly RelayCommand DragSaveDataOverCommandInstance = new RelayCommand(DragSaveDataOver);
        private static readonly RelayCommand DragSaveDataDropCommandInstance = new RelayCommand(DragSaveDataDrop);

        #region Commands

        /// <summary>
        /// Gets the command to execute when a drag operation enters the object.
        /// </summary>
        public static RelayCommand DragManualEnterCommand
        {
            get { return DragManualEnterCommandInstance; }
        }

        /// <summary>
        /// Gets the command to execute when a drag operation occurs over the object.
        /// </summary>
        public static RelayCommand DragManualOverCommand
        {
            get { return DragManualOverCommandInstance; }
        }

        /// <summary>
        /// Gets the command to execute when a drop operation occurs on the object.
        /// </summary>
        public static RelayCommand DragManualDropCommand
        {
            get { return DragManualDropCommandInstance; }
        }

        /// <summary>
        /// Gets the command to execute when a drag operation enters the object.
        /// </summary>
        public static RelayCommand DragSaveDataEnterCommand
        {
            get { return DragSaveDataEnterCommandInstance; }
        }

        /// <summary>
        /// Gets the command to execute when a drag operation occurs over the object.
        /// </summary>
        public static RelayCommand DragSaveDataOverCommand
        {
            get { return DragSaveDataOverCommandInstance; }
        }

        /// <summary>
        /// Gets the command to execute when a drag operation enters the object.
        /// </summary>
        public static RelayCommand DragSaveDataDropCommand
        {
            get { return DragSaveDataDropCommandInstance; }
        }

        /// <summary>
        /// Gets the command to execute when a drag and drop operation starts.
        /// </summary>
        public INTV.Shared.Behavior.IDragStartCommand DragStartCommand
        {
            get { return new StartDragFileNodeCommand(this, CanStartDrag); }
        }

        /// <summary>
        /// Gets the command to execute when an item is being dragged during a drag and drop operation.
        /// </summary>
        public ICommand DragItemsCommand
        {
            get { return new RelayCommand(DragItems); }
        }

        /// <summary>
        /// Gets the command to execute when an item is dropped as part of a drag and drop operation.
        /// </summary>
        public ICommand DropItemsCommand
        {
            get { return new RelayCommand(DropItems); }
        }

        #endregion // Commands

        /// <summary>
        /// Determines whether the dragged data should be accepted as a valid drag drop operation.
        /// </summary>
        /// <param name="dragEventArgs">The data from a drag event.</param>
        /// <returns><c>true</c> if the dragged data is acceptable for a drag drop operation.</returns>
        protected bool AcceptDragData(DragEventArgs dragEventArgs)
        {
            bool accept = dragEventArgs.Data.GetDataPresent(MenuLayoutViewModel.DragDataFormat) ||
                          dragEventArgs.Data.GetDataPresent(ProgramDescriptionViewModel.DragDataFormat) ||
                          (false && dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop));
            if (accept && dragEventArgs.Data.GetDataPresent(DragDropHelpers.DragDropFeedbackDataFormat))
            {
                var dragDropFeedback = dragEventArgs.Data.GetData(DragDropHelpers.DragDropFeedbackDataFormat) as IDragDropFeedback;
                dragDropFeedback.AllowsChildren = this is FolderViewModel;
            }
            if (accept)
            {
                accept = AcceptDragData(dragEventArgs.Data);
            }
            if (!accept)
            {
                dragEventArgs.Effects = DragDropEffects.None;
            }
            return accept;
        }

        private static bool AcceptManual(DragEventArgs dragEventArgs)
        {
            bool accept = dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop);
            if (accept)
            {
                var data = dragEventArgs.Data as IDataObject;
                var files = data.GetData(DataFormats.FileDrop) as IEnumerable<string>;
                accept = (files != null) && files.Any();
                if (accept)
                {
                    var textFiles = files.Where(f => f.EndsWith(".txt", true, System.Globalization.CultureInfo.InvariantCulture));
                    accept = (textFiles.Count() == 1) && AcceptManual(files.First());
                }
            }
            if (!accept)
            {
                dragEventArgs.Effects = DragDropEffects.None;
            }
            return accept;
        }

        private static void DragManualEnter(object dragEventArgs)
        {
            var dragArgs = (DragEventArgs)dragEventArgs;
            if (!dragArgs.Handled)
            {
                AcceptManual(dragArgs);
            }
        }

        private static void DragManualOver(object dragEventArgs)
        {
            var dragArgs = (DragEventArgs)dragEventArgs;
            if (!dragArgs.Handled)
            {
                AcceptManual(dragArgs);
            }
        }

        private static void DragManualDrop(object dragEventArgs)
        {
            var dragArgs = (DragEventArgs)dragEventArgs;
            if (!dragArgs.Handled && AcceptManual(dragArgs))
            {
                var data = dragArgs.Data as IDataObject;
                var file = (data.GetData(DataFormats.FileDrop) as IEnumerable<string>).First();
                var dropTarget = ((FrameworkElement)dragArgs.Source).DataContext as FileNodeViewModel;
                dropTarget.Manual = file;
            }
        }

        private static void DragSaveDataEnter(object obj)
        {
            ////throw new System.NotImplementedException();
        }

        private static void DragSaveDataOver(object obj)
        {
            ////throw new System.NotImplementedException();
        }

        private static void DragSaveDataDrop(object obj)
        {
            ////throw new System.NotImplementedException();
        }

        private bool AcceptDragData(IDataObject data)
        {
            IFileContainer dropTarget = Model as IFileContainer;
            if (dropTarget == null)
            {
                dropTarget = Parent;
            }
            var draggedItem = data.GetData(MenuLayoutViewModel.DragDataFormat) as IFile;
            bool accept = draggedItem != null;
            if (accept)
            {
                var draggedItemParent = draggedItem.Parent;
                accept = draggedItem != this.Model; // don't drop on self
                if (accept && (dropTarget != null))
                {
                    // check if drop target and drop position would make us insert right before or after self -- and disallow
                    if ((Parent != null) && (draggedItemParent == Parent) && data.GetDataPresent(DragDropHelpers.DragDropFeedbackDataFormat))
                    {
                        var myIndex = Parent.IndexOfChild(Model);
                        var draggedItemIndex = draggedItemParent.IndexOfChild(draggedItem);
                        var dragPositionDelta = myIndex - draggedItemIndex;
                        if (System.Math.Abs(dragPositionDelta) == 1)
                        {
                            var dragDropFeedback = data.GetData(DragDropHelpers.DragDropFeedbackDataFormat) as IDragDropFeedback;
                            switch (dragDropFeedback.DropLocation)
                            {
                                case DropOnItemLocation.TopQuarter:
                                    accept = dragPositionDelta < 0;
                                    break;
                                case DropOnItemLocation.BottomQuarter:
                                    accept = dragPositionDelta > 0;
                                    break;
                                case DropOnItemLocation.TopMiddleQuarter:
                                    if (!dragDropFeedback.AllowsChildren && (dragPositionDelta > 0))
                                    {
                                        // being dropped on non-folder below, so disallow
                                        accept = false;
                                    }
                                    break;
                                case DropOnItemLocation.BottomMiddleQuarter:
                                    if (!dragDropFeedback.AllowsChildren && (dragPositionDelta < 0))
                                    {
                                        // being dropped on non-folder above, so disallow
                                        accept = false;
                                    }
                                    break;
                            }
                            if (!accept)
                            {
                                dragDropFeedback.DropLocation = DropOnItemLocation.None;
                            }
                        }
                    }
                    var draggedContainer = draggedItem as IFileContainer;
                    if (accept && (draggedContainer != null))
                    {
                        accept = !draggedContainer.ContainsChild(dropTarget) && (draggedContainer != dropTarget);
                    }
                    if (accept)
                    {
                        accept = (dropTarget.Size < FileSystemConstants.MaxItemCount) || (dropTarget == draggedItemParent);
                    }
                }
            }
            else
            {
                accept = ((FileNode)dropTarget).FileSystem.Origin != FileSystemOrigin.LtoFlash; // cheesy way to prevent D&D working on 'advanced' view... Perhaps ideally this would be done in the XAML?
                if (accept)
                {
                    var draggedPrograms = data.GetData(ProgramDescriptionViewModel.DragDataFormat) as IEnumerable<ProgramDescription>;
                    if (draggedPrograms != null)
                    {
                        accept = CanAcceptFiles(dropTarget, draggedPrograms, null);
                    }
                    else
                    {
                        var files = data.GetData(DataFormats.FileDrop) as IEnumerable<string>;
                        accept = false && (files != null) && files.Any();
                    }
                }
            }

            return accept;
        }

#if false
        private bool AcceptDragData(System.Windows.IDataObject data)
        {
            IFileContainer dropTarget = Model as IFileContainer;
            if (dropTarget == null)
            {
                dropTarget = Parent;
            }
            var draggedItem = data.GetData(MenuLayoutViewModel.DragDataFormat) as IFile;
            bool accept = draggedItem != null;
            if (accept)
            {
                accept = ShouldAcceptDraggedItem(draggedItem);
                /*
                var draggedItemParent = draggedItem.Parent;
                accept = (draggedItem != this.Model);
                if (accept && (dropTarget != null))
                {
                    var draggedContainer = draggedItem as IFileContainer;
                    if (draggedContainer != null)
                    {
                        accept = !draggedContainer.ContainsChild(dropTarget) && (draggedContainer != dropTarget);
                    }
                    if (accept)
                    {
                        accept = (dropTarget.Size < FileSystemConstants.MaxItemCount) || (dropTarget == draggedItemParent);
                    }
                    if (accept)
                    {
                        int currentDepth = DepthFromRoot;
                        int draggedItemDepth = draggedItem.Depth;
                        if (draggedContainer != null)
                        {
                            ++draggedItemDepth; // count folders as one extra layer of depth, since they are pointless w/o containing any items
                        }
                        accept = currentDepth + draggedItemDepth <= FileSystemConstants.MaximumDepth;
                    }
                }
                */
            }
            else
            {
                IEnumerable<ProgramDescription> draggedPrograms = data.GetData(ProgramDescriptionViewModel.DragDataFormat) as IEnumerable<ProgramDescription>;
                accept = (draggedPrograms != null) && (draggedPrograms.Any() && ((draggedPrograms.Count() + dropTarget.Size) <= FileSystemConstants.MaxItemCount));
            }

            return accept;
        }
#endif // false

        private bool CanStartDrag(object parameter)
        {
            bool canExecute = IsEditable;
            return canExecute;
        }

        private void DragItems(object dragEventArgs)
        {
            var dragArgs = dragEventArgs as DragEventArgs;
            if (!dragArgs.Handled)
            {
                AcceptDragData(dragArgs);
            }
        }

        private void DropItems(object dragEventArgs)
        {
            var dragDropArgs = dragEventArgs as DragEventArgs;
            var dropData = dragDropArgs.Data;
            if ((dropData != null) && AcceptDragData(dropData))
            {
                var dropPosition = DropOnItemLocation.None;
                if (dropData.GetDataPresent(DragDropHelpers.DragDropFeedbackDataFormat))
                {
                    var dragDropFeedback = dropData.GetData(DragDropHelpers.DragDropFeedbackDataFormat) as IDragDropFeedback;
                    dropPosition = dragDropFeedback.DropLocation;
                }

                var menuLayout = dropData.GetData(MenuLayoutViewModel.DragMenuLayoutDataFormat) as MenuLayoutViewModel;
                if (menuLayout == null)
                {
                    menuLayout = this as MenuLayoutViewModel;
                }
                var droppedItem = dropData.GetData(MenuLayoutViewModel.DragDataFormat) as IFile;
                if (droppedItem != null)
                {
#if MEASURE_MOVE_TO_PARENT
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // MEASURE_MOVE_TO_PARENT
                    try
                    {
                        // Rearranging items in the menu.
                        menuLayout.StartItemsUpdate();
                        var droppedIndex = droppedItem.Parent.IndexOfChild(droppedItem);
                        var selfAsFileContainer = Model as IFileContainer;
                        var selfIndex = (Parent == null) ? -1 : Parent.IndexOfChild(Model);
                        var insertIndex = selfIndex; // defaults to insert before the the dropped item hits, unless Parent is null, which means move to bottom
                        // NOTE: This falls apart in multi-"root" system... will need to be addressed then!
                        var inSameParent = object.ReferenceEquals(droppedItem.Parent, Parent);
                        if (selfAsFileContainer != null)
                        {
                            if (Parent == null)
                            {
                                dropPosition = DropOnItemLocation.None;
                            }
                            switch (dropPosition)
                            {
                                case DropOnItemLocation.TopQuarter:
                                case DropOnItemLocation.BottomQuarter:
                                    insertIndex = GetInsertIndex(dropPosition, droppedItem);
                                    Parent.InsertChild(insertIndex, droppedItem, false);
                                    var insertFolder = menuLayout.FindViewModelForModel(Parent);
                                    if (insertIndex < 0)
                                    {
                                        insertIndex = System.Math.Max(0, insertFolder.Items.Count - 1);
                                    }
                                    insertFolder.Items[insertIndex].IsSelected = true;
                                    menuLayout.RetainFocus = insertFolder.Items[insertIndex].GetHashCode();
                                    break;
                                case DropOnItemLocation.None:
                                case DropOnItemLocation.BottomMiddleQuarter:
                                case DropOnItemLocation.TopMiddleQuarter:
                                    // drop into folder
                                    if (!droppedItem.Parent.MoveChildToNewParent(droppedItem, selfAsFileContainer, false))
                                    {
                                        throw new System.InvalidOperationException("Failed to move item!");
                                    }
                                    if (!((FolderViewModel)this).IsOpen)
                                    {
                                        IsSelected = true;
                                        ////menuLayout.RetainFocus = GetHashCode();
                                    }
                                    else
                                    {
                                        var indexInSelf = selfAsFileContainer.IndexOfChild(droppedItem);
                                        Items[indexInSelf].IsSelected = true;
                                        ////menuLayout.RetainFocus = Items[indexInSelf].GetHashCode();
                                    }
                                    ++menuLayout.RetainFocus;
                                    break;
                            }
                        }
                        else
                        {
                            // if an item is dropped on a non-folder, insert before the drop target item.
                            insertIndex = GetInsertIndex(dropPosition, droppedItem);
                            Parent.InsertChild(insertIndex, droppedItem, false);
                            var insertFolder = menuLayout.FindViewModelForModel(Parent);
                            if (insertIndex < 0)
                            {
                                insertIndex = System.Math.Max(0, insertFolder.Items.Count - 1);
                            }
                            insertFolder.Items[insertIndex].IsSelected = true;
                            ++menuLayout.RetainFocus;
                        }
                    }
                    finally
                    {
                        menuLayout.FinishItemsUpdate(true);
#if MEASURE_MOVE_TO_PARENT
                        stopwatch.Stop();
                        System.Diagnostics.Debug.WriteLine("## MoveChildToNewParent via DragDrop took " + stopwatch.Elapsed.ToString());
#endif // MEASURE_MOVE_TO_PARENT
                    }
                }
                else
                {
                    // Dropping items from the ROM list.
                    var items = dropData.GetData(ProgramDescriptionViewModel.DragDataFormat) as IEnumerable<ProgramDescription>;
                    if ((items != null) && items.Any())
                    {
                        IFileContainer dropTarget = Model as IFileContainer;
                        int insertIndex = -1;
                        if (dropTarget == null)
                        {
                            dropTarget = Parent;
                            insertIndex = dropTarget.IndexOfChild(Model);
                            switch (dropPosition)
                            {
                                case DropOnItemLocation.None:
                                case DropOnItemLocation.TopQuarter:
                                case DropOnItemLocation.TopMiddleQuarter:
                                    break;
                                case DropOnItemLocation.BottomMiddleQuarter:
                                case DropOnItemLocation.BottomQuarter:
                                    ++insertIndex;
                                    break;
                            }
                        }
                        else
                        {
                            // This is a folder, so enforce folder behaviors.
                            if (Parent == null)
                            {
                                dropPosition = DropOnItemLocation.None;
                            }
                            switch (dropPosition)
                            {
                                case DropOnItemLocation.TopQuarter:
                                    dropTarget = Parent;
                                    insertIndex = Parent.IndexOfChild(Model);
                                    break;
                                case DropOnItemLocation.None:
                                case DropOnItemLocation.TopMiddleQuarter:
                                case DropOnItemLocation.BottomMiddleQuarter:
                                    break;
                                case DropOnItemLocation.BottomQuarter:
                                    dropTarget = Parent;
                                    insertIndex = Parent.IndexOfChild(Model);
                                    ++insertIndex;
                                    break;
                            }
                        }
                        AddItems(menuLayout, dropTarget, items, insertIndex); // Saves items upon completion.
                    }
                }
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int GetInsertIndex(DropOnItemLocation dropLocation, IFile droppedItem)
        {
            var droppedIndex = droppedItem.Parent.IndexOfChild(droppedItem);
            var selfIndex = (Parent == null) ? -1 : Parent.IndexOfChild(Model);
            var insertIndex = selfIndex; // defaults to insert before the the dropped item hits, unless Parent is null, which means move to bottom
            // NOTE: This falls apart in multi-"root" system... will need to be addressed then!
            var inSameParent = object.ReferenceEquals(droppedItem.Parent, Parent);

            switch (dropLocation)
            {
                case DropOnItemLocation.None:
                case DropOnItemLocation.TopQuarter:
                case DropOnItemLocation.TopMiddleQuarter:
                    if (inSameParent && (droppedIndex < selfIndex))
                    {
                        // the dragged item was above this item in the list, and should be inserted
                        // after this one, so decrement the insert location
                        --insertIndex;
                    }
                    break;
                case DropOnItemLocation.BottomQuarter:
                case DropOnItemLocation.BottomMiddleQuarter:
                    if (!inSameParent || (droppedIndex > selfIndex))
                    {
                        // the dragged item was lower in the list and moving up, but should be inserted
                        // below this item, so increment the insert location
                        ++insertIndex;
                    }
                    break;
            }

            return insertIndex;
        }
    }
/*
    private void DropItems(object dragEventArgs)
    {
        var dragDropArgs = dragEventArgs as DragEventArgs;
        var dropData = dragDropArgs.Data;
        if ((dropData != null) && AcceptDragData(dropData))
        {
            var menuLayout = dropData.GetData(MenuLayoutViewModel.DragMenuLayoutDataFormat) as MenuLayoutViewModel;
            if (menuLayout == null)
            {
                menuLayout = this as MenuLayoutViewModel;
            }
            if (menuLayout != null)
            {
                menuLayout.StartItemsUpdate();
            }
            var droppedItem = dropData.GetData(MenuLayoutViewModel.DragDataFormat) as IFile;
            if (droppedItem != null)
            {
                if (droppedItem.Parent != null)
                {
                    var selfAsFileContainer = Model as IFileContainer;
                    if (selfAsFileContainer != null)
                    {
                        if (!selfAsFileContainer.AddChild(droppedItem))
                        {
                            // we're moving to the bottom, so remove and re-add
                            selfAsFileContainer.RemoveChild(droppedItem);
                            selfAsFileContainer.AddChild(droppedItem);
                        }
                    }
                    else
                    {
                        // if an item is dropped on a non-file, insert before the drop target item.
                        var insertLocation = Parent.IndexOf(Model);
                        Parent.Insert(insertLocation, droppedItem);
                    }
                }
            }
            else
            {
                var items = dropData.GetData(ProgramDescriptionViewModel.DragDataFormat) as IEnumerable<ProgramDescription>;
                if ((items != null) && items.Any())
                {
                    IFileContainer dropTarget = Model as IFileContainer;
                    int insertIndex = -1;
                    if (dropTarget == null)
                    {
                        dropTarget = Parent;
                        insertIndex = dropTarget.IndexOf(Model);
                    }
                    AddItems(menuLayout, dropTarget, items, insertIndex);
                }
            }
            if (menuLayout != null)
            {
                menuLayout.FinishItemsUpdate();
            }
            CommandManager.InvalidateRequerySuggested();
        }
    } */
}
