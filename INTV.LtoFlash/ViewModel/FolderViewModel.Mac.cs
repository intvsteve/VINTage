// <copyright file="FolderViewModel.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.LtoFlash.Model;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class FolderViewModel
    {
        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public FolderViewModel(System.IntPtr handle)
            : base (handle)
        {
        }

        /// <inheritdoc />
        public override uint ItemCount
        {
            get { return _itemCount; }
            protected set { _itemCount = value; }
        }
        private uint _itemCount;

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var itemsToAdd = Enumerable.Empty<FileNodeViewModel>();
            var itemsToRemove = Enumerable.Empty<FileNodeViewModel>();
            var indexSetToAdd = new NSMutableIndexSet();
            var indexSetToRemove = new NSMutableIndexSet();
            var children = NSArray.FromArray<FileNodeViewModel>(Children).ToList();
            if ((sender != null) && (e != null))
            {
                switch(e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        var newItems = new List<FileNodeViewModel>();
                        for (int i = 0; i < e.NewItems.Count; ++i)
                        {
                            var item = (FileNodeViewModel)e.NewItems[i];
                            var index = (e.NewStartingIndex < 0) ? (children.Count + i) : (e.NewStartingIndex + i);
                            indexSetToAdd.Add((uint)index);
                            newItems.Add(item);
                        }
                        itemsToAdd = newItems;
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        var oldItems = new List<FileNodeViewModel>();
                        for (int i = 0; i < e.OldItems.Count; ++i)
                        {
                            var item = (FileNodeViewModel)e.OldItems[i];
                            var index = e.OldStartingIndex < 0 ? children.IndexOf(item) : (e.OldStartingIndex + i);
                            indexSetToRemove.Add((uint)index);
                            oldItems.Add(item);
                        }
                        itemsToRemove = oldItems;
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                        oldItems = new List<FileNodeViewModel>();
                        for (int i = 0; i < e.OldItems.Count; ++i)
                        {
                            var item = (FileNodeViewModel)e.OldItems[i];
                            var index = e.OldStartingIndex + i; // children.IndexOf(item);
                            indexSetToRemove.Add((uint)index);
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
                            indexSetToAdd.Add((uint)index);
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
                itemsToRemove = children.Except(Items);
                foreach (var item in itemsToRemove)
                {
                    for (uint idx = 0; idx < Children.Count; ++idx)
                    {
                        var element = Children.ValueAt(idx);
                        if (element == item.Handle)
                        {
                            indexSetToRemove.Add(idx);
                        }
                    }
                }
                itemsToAdd = Items.Except(children);
                uint i = 0;
                foreach (var item in itemsToAdd)
                {
                    indexSetToAdd.Add(i);
                    ++i;
                }
            }

            if (itemsToRemove.Any())
            {
                WillChange(NSKeyValueChange.Removal, indexSetToRemove, (NSString)ItemsPropertyName);
                Children.RemoveObjectsAtIndexes(indexSetToRemove);
                ItemCount = (uint)Children.Count;
                DidChange(NSKeyValueChange.Removal, indexSetToRemove, (NSString)ItemsPropertyName);
            }
            if (itemsToAdd.Any())
            {
                WillChange(NSKeyValueChange.Insertion, indexSetToAdd, (NSString)ItemsPropertyName);
                Children.InsertObjects(itemsToAdd.ToArray(), indexSetToAdd);
                ItemCount = (uint)Children.Count;
                DidChange(NSKeyValueChange.Insertion, indexSetToAdd, (NSString)ItemsPropertyName);
            }
            if ((sender == null) && (e == null))
            {
                RaisePropertyChanged(StatusPropertyName); // addresses some lack-of-update problems (drop stuff here not going away)
            }
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
