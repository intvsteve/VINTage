// <copyright file="WeakKeyDictionary`TKey`TValue.cs" company="INTV Funhouse">
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace INTV.Core.Utility
{
    /// <summary>
    /// Implements a dictionary in which the keys to the elements are weakly referenced.
    /// </summary>
    /// <typeparam name="TKey">Data type of the dictionary's keys.</typeparam>
    /// <typeparam name="TValue">Data type of the dictionary's values.</typeparam>
    /// <remarks>Because the keys to this dictionary are weak references to the values, if the
    /// object acting as a key is garbage collected, the entry in the dictionary will 'go bad'.
    /// Each access to the dictionary will, by necessity, sweep for 'dead' objects.
    /// Also note that because of the object lifetime awareness via the WeakReference, this
    /// dictionary uses locks to protect access.</remarks>
    public class WeakKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary where TKey : class
    {
        private Dictionary<WeakReference, TValue> _dictionary = new Dictionary<WeakReference, TValue>();

        #region Properties

        #region ICollection Properties

        /// <inheritdoc/>
        public int Count
        {
            get { return _dictionary.Count; }
        }

        /// <inheritdoc/>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public object SyncRoot
        {
            get { return _dictionary; }
        }

        #endregion // ICollection Properties

        #region IDictionary Properties

        /// <inheritdoc/>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc/>
        ICollection IDictionary.Keys
        {
            get { return _dictionary.Keys; }
        }

        /// <inheritdoc/>
        ICollection IDictionary.Values
        {
            get { return _dictionary.Values; }
        }

        /// <inheritdoc/>
        object IDictionary.this[object key]
        {
            get { return GetEntry((TKey)key); }
            set { AddEntry((TKey)key, (TValue)value); }
        }

        #endregion // IDictionary Properties

        #region IDictionary<TKey, TValue> Properties

        /// <inheritdoc/>
        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys.Where(k => k.IsAlive).Select(k => (TKey)k.Target).ToList(); }
        }

        /// <inheritdoc/>
        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get { return GetEntry(key); }
            set { AddEntry(key, value); }
        }

        #endregion // IDictionary<TKey, TValue> Properties

        #endregion // Properties

        /// <summary>
        /// Add an entry to the dictionary.
        /// </summary>
        /// <param name="key">The key for the entry.</param>
        /// <param name="value">The value to associate with the key.</param>
        /// <returns><c>true</c> iff the value was added to the dictionary.</returns>
        public bool AddEntry(TKey key, TValue value)
        {
            bool added = false;
            lock (_dictionary)
            {
                PurgeDeadEntries();
                var entryKey = _dictionary.FirstOrDefault(e => e.Key.IsAlive && e.Key.Target == key).Key;
                if (entryKey == null)
                {
                    entryKey = new WeakReference(key);
                    added = true;
                }
                _dictionary[entryKey] = value;
            }
            return added;
        }

        /// <summary>
        /// Remove an entry from the dictionary.
        /// </summary>
        /// <param name="key">The key of the entry to remove.</param>
        /// <returns><c>true</c> iff the value was removed from the dictionary.</returns>
        public bool RemoveEntry(TKey key)
        {
            bool removed = false;
            lock (_dictionary)
            {
                var entry = _dictionary.FirstOrDefault(e => e.Key.IsAlive && e.Key.Target == key);
                if (entry.Key != null)
                {
                    removed = _dictionary.Remove(entry.Key);
                }
                PurgeDeadEntries();
            }
            return removed;
        }

        /// <summary>
        /// Get the value associated with the given key.
        /// </summary>
        /// <param name="key">The key for which a value is desired.</param>
        /// <returns>The value for the given key, or default(TValue) if the key is invalid.</returns>
        /// <remarks>This dictionary is not very informative if your key has been garbage collected, or is valid but does
        /// not refer to a valid entry.</remarks>
        public TValue GetEntry(TKey key)
        {
            TValue value = default(TValue);
            lock (_dictionary)
            {
                PurgeDeadEntries();
                var entry = _dictionary.FirstOrDefault(e => e.Key.IsAlive && e.Key.Target == key);
                if (entry.Key != null)
                {
                    value = (TValue)entry.Value;
                }
            }
            return value;
        }

        #region IEnumerable

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion // IEnumerable

        #region ICollection

        /// <inheritdoc/>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException("ICollection.CopyTo");
        }

        #endregion // ICollection

        #region IDictionary

        /// <inheritdoc/>
        public void Add(object key, object value)
        {
            AddEntry((TKey)key, (TValue)value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        /// <inheritdoc/>
        public IDictionaryEnumerator GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Remove(object key)
        {
            RemoveEntry((TKey)key);
        }

        #endregion // IDictionary

        #region IEnumerable<KeyValuePair<TKey, TValue>>

        #endregion // IEnumerable<KeyValuePair<TKey, TValue>>

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
        }

        #region ICollection<KeyValuePair<TKey, TValue>>

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            AddEntry(item.Key, item.Value);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            _dictionary.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return RemoveEntry(item.Key);
        }

        #endregion // ICollection<KeyValuePair<TKey, TValue>>

        #region IDictionary<TKey, TValue>

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            AddEntry(key, value);
        }

        /// <summary>
        /// Determines whether the key exists in the dictionary.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><c>true</c> if the key is valid and present.</returns>
        public bool ContainsKey(TKey key)
        {
            var containsKey = false;
            lock (_dictionary)
            {
                PurgeDeadEntries();
                containsKey = _dictionary.FirstOrDefault(e => e.Key.IsAlive && e.Key.Target == key).Key != null;
            }
            return containsKey;
        }

        public bool Remove(TKey key)
        {
            return RemoveEntry(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException("WeakKeyDictionary.TryGetValue()");
        }

        #endregion // IDictionary<TKey, TValue>

        private void PurgeDeadEntries()
        {
            var deadEntries = _dictionary.Where(e => !e.Key.IsAlive).Select(e => e.Key).ToList();
            foreach (var deadEntry in deadEntries)
            {
                _dictionary.Remove(deadEntry);
            }
        }
    }
}
