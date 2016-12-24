// <copyright file="SearchDirectories.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
using System.Collections.Specialized;
using System.Linq;

namespace INTV.Shared.Model
{
    /// <summary>
    /// This class stores the directories to search for ROMs. It also provides an INotifyCollectionChanged implementation.
    /// </summary>
    /// <remarks>Note that some the Add, Clear and Remove methods replace the HashSet implementation in order to provide a rudimentary
    /// INotifyCollectionChanged implementation. It does not replace other methods that can cause modifications to the set that should,
    /// in turn, raise CollectionChanged events. Perhaps subclassing HashSet is a bad idea, but I'm too lazy at the moment to wrap
    /// HashSet and implement a set various IList/IEnumerable interfaces. So there.</remarks>
    [Serializable]
    public sealed class SearchDirectories : HashSet<string>, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged

        /// <summary>
        /// This event is raised when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion // INotifyCollectionChanged

        /// <summary>
        /// Initializes a new instance of SearchDirectories.
        /// </summary>
        public SearchDirectories()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Model.SearchDirectories"/> class.
        /// </summary>
        /// <param name="collection">The initial set of values in the collection.</param>
        internal SearchDirectories(IEnumerable<string> collection)
            : base(collection)
        {
        }

        #region ISerializable

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Serialization descriptor.</param>
        /// <param name="context">Source and destination stream information, as well as additional user-supplied context.</param>
        private SearchDirectories(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion // ISerializable

        /// <summary>
        /// Adds the specified element to the collection.
        /// </summary>
        /// <param name="item"> The element to add.</param>
        /// <returns><c>true</c> if the element is added to the collection.</returns>
        public new bool Add(string item)
        {
            bool added = !this.Contains(item, INTV.Shared.Utility.PathComparer.Instance);
            if (added)
            {
                added = base.Add(item);
                if (added)
                {
                    var collectionChanged = CollectionChanged;
                    if (collectionChanged != null)
                    {
                        collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, IndexOf(item)));
                    }
                }
            }
            return added;
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public new void Clear()
        {
            bool hadItems = Count > 0;
            base.Clear();
            if (hadItems)
            {
                var collectionChanged = CollectionChanged;
                if (collectionChanged != null)
                {
                    collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        /// <summary>
        /// Removes the specified element.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns><c>true</c>if the element is successfully found and removed; otherwise, false.</returns>
        public new bool Remove(string item)
        {
            var removedIndex = IndexOf(item);
            var removed = base.Remove(item);
            if (removed)
            {
                var collectionChanged = CollectionChanged;
                if (collectionChanged != null)
                {
                    collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, removedIndex));
                }
            }
            return removed;
        }

        private void OnCollectionChanged(string item, string oldItem, int index)
        {
            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
            }
        }

        private int IndexOf(string item)
        {
            int foundIndex = -1;
            int currentIndex = 0;
            foreach (var path in this)
            {
                if (INTV.Shared.Utility.PathComparer.Instance.Equals(item, path))
                {
                    foundIndex = currentIndex;
                    break;
                }
                ++currentIndex;
            }
            return foundIndex;
        }
    }
}
