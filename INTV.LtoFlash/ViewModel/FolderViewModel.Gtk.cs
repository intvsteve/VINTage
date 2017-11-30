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

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
                    var itemsToAdd = Enumerable.Empty<FileNodeViewModel>();
                    var itemsToRemove = Enumerable.Empty<FileNodeViewModel>();
                    if ((sender != null) && (e != null))
                    {
                        var treeStore = TreeStore;
                        Gtk.TreeIter treeIter;
                        var gotSelfIter = GetIterForItem(out treeIter, treeStore);
                        switch (e.Action)
                        {
                            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                                var newItems = new List<FileNodeViewModel>();
                                Gtk.TreeIter insertIter;
                                var insertIterValid = GetTreeIterForItem(out insertIter, treeIter, treeStore, null, e.NewStartingIndex - 1);
                                for (int i = 0; i < e.NewItems.Count; ++i)
                                {
                                    var item = (FileNodeViewModel)e.NewItems[i];
                                    var folder = item as FolderViewModel;
                                    if (folder != null)
                                    {
                                        // Ensure that the tree store is properly initialized.
                                        folder.InitializeGtkModel(treeStore);
                                    }
                                    // TODO: Make sure this works for multi-adds.
                                    if (insertIterValid)
                                    {
                                        insertIter = treeStore.InsertNodeAfter(insertIter);
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
                                    //var index = (e.NewStartingIndex < 0) ? (children.Count + i) : (e.NewStartingIndex + i);
                                    //indexSetToAdd.Add((uint)index);
                                    newItems.Add(item);
                                }
                                itemsToAdd = newItems;
                                break;
                            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                                var oldItems = new List<FileNodeViewModel>();
                                for (int i = 0; i < e.OldItems.Count; ++i)
                                {
                                    var item = (FileNodeViewModel)e.OldItems[i];
                                    Gtk.TreeIter itemIter;
                                    if (GetTreeIterForItem(out itemIter, treeIter, treeStore, item, e.OldStartingIndex + i))
                                    {
                                        //oldItems.Add(item);
                                        treeStore.Remove(ref itemIter);
                                    }
                                }
                                itemsToRemove = oldItems;
                                break;
                            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                                oldItems = new List<FileNodeViewModel>();
                                for (int i = 0; i < e.OldItems.Count; ++i)
                                {
                                    var item = (FileNodeViewModel)e.OldItems[i];
                                    var index = e.OldStartingIndex + i; // children.IndexOf(item);
                                    //indexSetToRemove.Add((uint)index);
                                    oldItems.Add(item);
                                }
                                itemsToRemove = oldItems;
                                newItems = new List<FileNodeViewModel>();
                                for (int i = 0; i < e.NewItems.Count; ++i)
                                {
                                    var item = (FileNodeViewModel)e.NewItems[i];
                                    var index = e.NewStartingIndex + i;
                                    // TODO Is this actually possible? Is it really a bug?
                                    ////if (e.NewStartingIndex > e.OldStartingIndex)
                                    ////{
                                    ////    --index; // NOTE: Probably a bug here: if we delete multiple consecutive, gotta fix it up?
                                    ////}
                                    //indexSetToAdd.Add((uint)index);
                                    newItems.Add(item);
                                }
                                itemsToAdd = newItems;
                                break;
                            default:
                                INTV.Shared.Utility.ErrorReporting.ReportNotImplementedError("Unhandled collection change: " + e.Action);
                                break;
                        }
                    }
                    else if ((sender == null) && (e == null) && (Items != null))
                    {
                        // Replaced model, so re-populate NSArray w/ the new one.
                        //itemsToRemove = children.Except(Items);
                        foreach (var item in itemsToRemove)
                        {
                            //for (uint idx = 0; idx < Children.Count; ++idx)
                            {
                                //var element = Children.ValueAt(idx);
                                //if (element == item.Handle)
                                {
                                    //indexSetToRemove.Add(idx);
                                }
                            }
                        }
                        //itemsToAdd = Items.Except(children);
                        uint i = 0;
                        foreach (var item in itemsToAdd)
                        {
                            //indexSetToAdd.Add(i);
                            ++i;
                        }
                    }

                    if (itemsToRemove.Any())
                    {
                        //WillChange(NSKeyValueChange.Removal, indexSetToRemove, (NSString)ItemsPropertyName);
                        //Children.RemoveObjectsAtIndexes(indexSetToRemove);
                        //ItemCount = (uint)Children.Count;
                        //DidChange(NSKeyValueChange.Removal, indexSetToRemove, (NSString)ItemsPropertyName);
                    }
                    if (itemsToAdd.Any())
                    {
                        //WillChange(NSKeyValueChange.Insertion, indexSetToAdd, (NSString)ItemsPropertyName);
                        //Children.InsertObjects(itemsToAdd.ToArray(), indexSetToAdd);
                        //ItemCount = (uint)Children.Count;
                        //DidChange(NSKeyValueChange.Insertion, indexSetToAdd, (NSString)ItemsPropertyName);
                    }
                    if ((sender == null) && (e == null))
                    {
//                RaisePropertyChanged(StatusPropertyName); // addresses some lack-of-update problems (drop stuff here not going away)
                    }
                });
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
