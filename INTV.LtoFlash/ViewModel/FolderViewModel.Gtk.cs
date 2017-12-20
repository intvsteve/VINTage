// <copyright file="FolderViewModel.Gtk.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using INTV.LtoFlash.Model;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class FolderViewModel
    {
        /// <summary>
        /// Gets the tree store.
        /// </summary>
        protected internal Gtk.TreeStore TreeStore { get; private set; }

        /// <summary>
        /// Initializes the GTK-specific fields -- back pointer to the GTK-specific model (Gtk.TreeStore).
        /// </summary>
        /// <param name="treeStore">The Gtk.TreeStore used by the visual.</param>
        protected internal void InitializeGtkModel(Gtk.TreeStore treeStore)
        {
            TreeStore = treeStore;
        }
        
        private static bool GetTreeIterForItem(out Gtk.TreeIter itemIter, Gtk.TreeIter parentIter, Gtk.TreeStore treeStore, FileNodeViewModel item, int index)
        {
            // if item is null, just get iterator at index
            itemIter = Gtk.TreeIter.Zero;
            var validIter = false;
            var putAtFront = (index < 0) && (item == null);
            if (parentIter.Equals(Gtk.TreeIter.Zero))
            {
                if (putAtFront)
                {
                    validIter = treeStore.IterChildren(out itemIter);
                }
                else
                {
                    validIter = (treeStore.IterNChildren() > index) && treeStore.IterNthChild(out itemIter, index);
                }
            }
            else
            {
                if (putAtFront)
                {
                    validIter = treeStore.IterChildren(out itemIter, parentIter);
                }
                else
                {
                    validIter = (treeStore.IterNChildren(parentIter) > index) && treeStore.IterNthChild(out itemIter, parentIter, index);
                }
            }
            var node = validIter ? treeStore.GetValue(itemIter, 0) as FileNodeViewModel : null;
            var foundIt = (node == null) || (item == null) || object.ReferenceEquals(node, item);
            return foundIt && !itemIter.Equals(Gtk.TreeIter.Zero);
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (OSDispatcher.IsMainThread)
            {
                // This must be done synchronously to work for drag/drop, etc.
                ItemsCollectionChangedCore(sender, e);
            }
            else
            {
                // NOTE: This has been known to result in the UI not syncing up correctly
                // if invoked during a drag/drop operation - despite the implemenation being
                // essentially the same as the body of this function.
                OSDispatcher.Current.InvokeOnMainDispatcher(() => ItemsCollectionChangedCore(sender, e));
            }
        }

        private void ItemsCollectionChangedCore(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if ((sender != null) && (e != null))
            {
                var treeStore = TreeStore;
                Gtk.TreeIter treeIter;
                var gotSelfIter = GetIterForItem(out treeIter, treeStore);
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        AddItemsToTreeStore(treeStore, e.NewItems.Cast<FileNodeViewModel>().ToList(), e.NewStartingIndex);
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        RemoveItemsFromTreeStore(treeStore, e.OldItems.Cast<FileNodeViewModel>().ToList(), e.OldStartingIndex, -1);
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                        var indexForInsert = RemoveItemsFromTreeStore(treeStore, e.OldItems.Cast<FileNodeViewModel>().ToList(), e.OldStartingIndex, e.NewStartingIndex);
                        AddItemsToTreeStore(treeStore, e.NewItems.Cast<FileNodeViewModel>().ToList(), indexForInsert);
                        break;
                    default:
                        INTV.Shared.Utility.ErrorReporting.ReportNotImplementedError("Unhandled collection change: " + e.Action);
                        break;
                }
            }
            else if ((sender == null) && (e == null) && (Items != null))
            {
                // Replaced model, so re-populate contents w/ the new one.
                // Invoked during initialization code.
            }
        }

        private void AddItemsToTreeStore(Gtk.TreeStore treeStore, IList<FileNodeViewModel> itemsToAdd, int startingIndex)
        {
            Gtk.TreeIter treeIter;
            var gotSelfIter = GetIterForItem(out treeIter, treeStore);
            Gtk.TreeIter insertIter;
            bool putAtFront = startingIndex == 0;
            var insertIterValid = GetTreeIterForItem(out insertIter, treeIter, treeStore, null, startingIndex - 1);
            for (int i = 0; i < itemsToAdd.Count; ++i)
            {
                var item = itemsToAdd[i];
                var folder = item as FolderViewModel;
                if (folder != null)
                {
                    // Ensure that the tree store is properly initialized.
                    folder.InitializeGtkModel(treeStore);
                }
                if (insertIterValid)
                {
                    if (putAtFront)
                    {
                        insertIter = treeStore.InsertNodeBefore(insertIter);
                        putAtFront = false;
                    }
                    else
                    {
                        insertIter = treeStore.InsertNodeAfter(insertIter);
                    }
                    treeStore.SetValue(insertIter, 0, item);
                }
                else if (treeIter.Equals(Gtk.TreeIter.Zero))
                {
                    treeStore.AppendValues(item);
                }
                else
                {
                    treeStore.AppendValues(treeIter, item);
                }
            }
        }

        private int RemoveItemsFromTreeStore(Gtk.TreeStore treeStore, IList<FileNodeViewModel> itemsToRemove, int startingIndex, int indexForInsert)
        {
            Gtk.TreeIter treeIter;
            var gotSelfIter = GetIterForItem(out treeIter, treeStore);
            for (int i = 0; i < itemsToRemove.Count; ++i)
            {
                var item = itemsToRemove[i];
                Gtk.TreeIter itemIter;

                // TODO: This seems wrong w.r.t. using the index here - probably cause trouble w/ multiselect
                if (GetTreeIterForItem(out itemIter, treeIter, treeStore, item, startingIndex + i))
                {
                    if (item.Parent == Model)
                    {
                        // Moving something under the same parent, so adjust indexForInsert if needed.
                        // If the item is to be moved to an index after its current index, decrement the
                        // new location, because the item before it in the list will be removed.
                        if ((indexForInsert > startingIndex) && (indexForInsert < (this.Items.Count - 1)))
                        {
                            --indexForInsert;
                        }
                    }
                    var treePath = treeStore.GetPath(itemIter);
                    treeStore.Remove(ref itemIter);
                }
            }
            return indexForInsert;
        }

        /// <summary>
        /// Replaceds the items collection.
        /// </summary>
        /// <param name="oldItems">Old items.</param>
        partial void ReplacedItemsCollection(ObservableViewModelCollection<FileNodeViewModel, IFile> oldItems)
        {
            if (oldItems != null)
            {
                oldItems.CollectionChanged -= ItemsCollectionChanged;
            }
            if (Items != null)
            {
                Items.CollectionChanged += ItemsCollectionChanged;
                ItemsCollectionChanged(null, null);
            }
        }
    }
}
