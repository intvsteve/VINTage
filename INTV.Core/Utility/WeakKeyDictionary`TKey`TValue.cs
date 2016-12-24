// <copyright file="WeakKeyDictionary`TKey`TValue.cs" company="INTV Funhouse">
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
    public class WeakKeyDictionary<TKey, TValue> where TKey : class
    {
        private Dictionary<WeakReference, TValue> _dictionary = new Dictionary<WeakReference, TValue>();

        /// <summary>
        /// Gets or sets a value associated with the given key.
        /// </summary>
        /// <param name="key">The key whose value is desired, or which is to be associated with the given value on set.</param>
        /// <returns>The value associated with the given key. If the key has been garbage collected, i.e. the reference
        /// is no longer valid, the default value for the TValue type is silently returned.</returns>
        public TValue this[TKey key]
        {
            get
            {
                return GetEntry(key);
            }
            set
            {
                AddEntry(key, value);
            }
        }

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
