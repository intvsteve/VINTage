// <copyright file="FixedSizeCollection`T.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

////#define DEBUG_ENTRY_REMOVAL
////#define ENABLE_ACTIVITY_LOGGER

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Specializes IList so it is of fixed size.
    /// </summary>
    /// <typeparam name="T">The type of element stored in the list.</typeparam>
    /// <remarks>This class presumes that <c>null</c> elements are not useful, and will treat them as holes in the collection that can be
    /// reclaimed to store non-null elements.</remarks>
    public class FixedSizeCollection<T> : IList<T>, IFixedSizeCollection, INotifyCollectionChanged where T : class
    {
        private static int _instanceId;
        private static Logger _logger;

        private List<T> _elements;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of FixedSizeCollection.
        /// </summary>
        /// <param name="size">The number of elements in the fixed-sized collection.</param>
        public FixedSizeCollection(int size)
        {
            InstanceId = ++_instanceId;
            _elements = new List<T>(Enumerable.Repeat<T>(null, size));
        }

        private FixedSizeCollection()
        {
            InstanceId = ++_instanceId;
        }

        #endregion // Constructors

        #region INotifyCollectionChanged Events

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion // INotifyCollectionChanged Events

        #region Properties

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public static Logger Logger
        {
            get
            {
                var logger = _logger;
                if (logger == null)
                {
                    try
                    {
                        var logDir = INTV.Shared.Model.RomListConfiguration.Instance.ErrorLogDirectory;
                        _logger = new Logger(System.IO.Path.Combine(logDir, "FileSystemDeletionLog.txt"));
                    }
                    catch (Exception)
                    {
                    }
                }
                return logger;
            }
        }

        #region ICollection<T> Properties

        /// <inheritdoc />
        public int Count
        {
            get { return _elements.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion // ICollection<T> Properties

        #region IList<T> Properties

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                return _elements[index];
            }
            set
            {
                var oldItem = _elements[index];
                _elements[index] = value;
                OnCollectionChanged(value, oldItem, index);
            }
        }

        #endregion // IList<T> Properties

        #region IFixedSizeCollection Properties

        /// <inheritdoc />
        public int Size
        {
            get { return Count; }
        }

        /// <inheritdoc />
        public int ItemsInUse
        {
            get { return _elements.Count(e => e != null); }
        }

        /// <inheritdoc />
        public int ItemsRemaining
        {
            get { return Count - ItemsInUse; }
        }

        #endregion // IFixedSizeCollection Properties

        /// <summary>
        /// Gets a value that uniquely identifies the instance. Really only useful for debugging purposes.
        /// </summary>
        public int InstanceId { get; private set; }

        #endregion // Properties

        #region IList<T> Methods

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            return _elements.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            var oldItem = _elements[index];
            if (oldItem != item)
            {
                _elements[index] = item;
                OnCollectionChanged(item, oldItem, index);
            }
        }

        /// <inheritdoc />
        public virtual void RemoveAt(int index)
        {
            var oldItem = _elements[index];
            _elements[index] = null;
            LogObjectRemoval(oldItem);
            OnCollectionChanged(null, oldItem, index);
        }

        #endregion // IList<T> Methods

        #region ICollection<T> Methods

        /// <inheritdoc />
        public virtual void Add(T item)
        {
            AddItem(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            for (int i = 0; i < _elements.Count; ++i)
            {
                _elements[i] = null;
            }
            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return _elements.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public virtual bool Remove(T item)
        {
            int index = IndexOf(item);
            bool removed = index >= 0;
            if (removed)
            {
                RemoveAt(index);
            }
            return removed;
        }

        #endregion // ICollection<T> Methods

        #region IEnumerable<T> Methods

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        #endregion // IEnumerable<T> Methods

        #region IEnumerable Methods

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_elements).GetEnumerator();
        }

        #endregion // IEnumerable Methods

        /// <summary>
        /// Logs the given string using the shared logger instance.
        /// </summary>
        /// <param name="message">The message to log. The instance ID will be included with the message.</param>
        [System.Diagnostics.Conditional("ENABLE_ACTIVITY_LOGGER")]
        public void LogActivity(string message)
        {
            var logger = Logger;
            if (logger != null)
            {
                logger.Log(string.Format("Instance: {0} - {1}", InstanceId, message));
            }
        }

        /// <summary>
        /// Adds an element to the collection using the first available index.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        /// <returns>The index at which the element was stored.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if there are no longer any free locations in the collection.</exception>
        protected int AddItem(T item)
        {
            var addIndex = IndexOf(null);
            if (addIndex < 0)
            {
                ErrorReporting.ReportError<IndexOutOfRangeException>(ReportMechanism.Default, "Bad index", "FixedSizeCollection.AddItem");
            }
            var currentIndex = IndexOf(item);
            if (currentIndex < 0)
            {
                _elements[addIndex] = item;
                OnCollectionChanged(item, null, addIndex);
            }
            else
            {
                addIndex = currentIndex;
            }
            return addIndex;
        }

        /// <summary>
        /// Raises the collection changed event.
        /// </summary>
        /// <param name="item">The new item (for addition / replacement / update).</param>
        /// <param name="oldItem">The old item (for removal / replacement / update).</param>
        /// <param name="index">The index of where change occurred.</param>
        protected void OnCollectionChanged(T item, T oldItem, int index)
        {
            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
            }
        }

        [System.Diagnostics.Conditional("DEBUG_ENTRY_REMOVAL")]
        private void LogObjectRemoval(T entry)
        {
            if ((entry != null) && typeof(T).FullName.EndsWith("Fork", StringComparison.InvariantCultureIgnoreCase))
            {
                var logger = Logger;
                if (logger != null)
                {
                    logger.Log(string.Format("GKT Instance {0}: deleted: {1}\nStack Trace:\n{2}", InstanceId, entry, System.Environment.StackTrace));
                }
            }
        }
    }
}
