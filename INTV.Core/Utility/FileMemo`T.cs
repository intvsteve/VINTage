// <copyright file="FileMemo`T.cs" company="INTV Funhouse">
// Copyright (c) 2017-2019 All Rights Reserved
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

#define ENABLE_MEMOS
////#define ENABLE_DEBUG_REPORTING

using System;
using System.Collections.Concurrent;
using System.Linq;

namespace INTV.Core.Utility
{
    /// <summary>
    /// This class provides core implementation for a simple memo system for data on some kind of storage device (file system, ZIP, etc.).
    /// </summary>
    /// <typeparam name="T">The data type of the memo of interest.</typeparam>
    public abstract class FileMemo<T>
    {
        private readonly ConcurrentDictionary<StorageLocation, Tuple<long, DateTime, T>> _memos = new System.Collections.Concurrent.ConcurrentDictionary<StorageLocation, Tuple<long, DateTime, T>>();

        protected FileMemo()
        {
        }

        /// <summary>
        /// Gets the default value for a memo. Note that this is necessary because the default value many not be the same as default(T).
        /// </summary>
        protected abstract T DefaultMemoValue { get; }

        /// <summary>
        /// Checks for a pre-existing memo.
        /// </summary>
        /// <param name="location">Location of the file for which the memo is desired.</param>
        /// <param name="memo">The stored value for the memo. If no memo is available, <see cref="DefaultMemoValue"/> is returned.</param>
        /// <returns><c>true</c> if a memo for <paramref name="location"/> was found, <c>false</c> otherwise.</returns>
        /// <remarks>In addition to checking for the memo, if the given file does not exist, any existing memo for it will be removed.</remarks>
        public bool CheckMemo(StorageLocation location, out T memo)
        {
            memo = DefaultMemoValue;
            var foundMemo = false;
            Tuple<long, DateTime, T> memorandum;
            if (!location.Exists())
            {
                if (_memos.TryRemove(location, out memorandum))
                {
                    DebugOutput("CheckMemo removed: " + location.Path + ": size: " + memorandum.Item1 + ", mod: " + memorandum.Item2 + ", memo: " + memorandum.Item3);
                }
            }
            else
            {
                foundMemo = _memos.TryGetValue(location, out memorandum) && (location.Size() == memorandum.Item1) && (location.LastModificationTimeUtc() == memorandum.Item2);
            }
            if (foundMemo)
            {
                memo = memorandum.Item3;
            }
            return foundMemo;
        }

        /// <summary>
        /// Adds a memo.
        /// </summary>
        /// <param name="location">Location of the file for which the memo is to be stored.</param>
        /// <param name="memo">The memo to store. See remarks.</param>
        /// <returns><c>true</c> if the memo was added; <c>false</c> otherwise.</returns>
        /// <remarks>The value of <paramref name="memo"/> is validated via a call to <see cref="IsValidMemo"/>. If the memo is not valid,
        /// the existing memo, if any, will be removed.</remarks>
        public bool AddMemo(StorageLocation location, T memo)
        {
#if ENABLE_MEMOS
            var added = IsValidMemo(memo) && location.Exists();
            if (added)
            {
                _memos[location] = new Tuple<long, DateTime, T>(location.Size(), location.LastModificationTimeUtc(), memo);
            }
            else
            {
                Tuple<long, DateTime, T> memorandum;
                if (_memos.TryRemove(location, out memorandum))
                {
                   DebugOutput("AddMemo removed: " + location.Path + ": size: " + memorandum.Item1 + ", mod: " + memorandum.Item2 + ", memo: " + memorandum.Item3);
                }
            }
#else
            var added = IsValidMemo(memo);
#endif // ENABLE_MEMOS
            return added;
        }

        /// <summary>
        /// Returns the existing memo value, or, if not present, requests a memo for insertion.
        /// </summary>
        /// <param name="location">Location of the file for which the memo is to be retrieved, or possibly stored.</param>
        /// <param name="data">Implementation-specific data used for the creation of the memo if a value is needed.</param>
        /// <param name="memo">Receives the value of the memo, either already stored, or newly generated and stored.</param>
        /// <returns><c>true</c> if a valid memo was either stored or created and stored.</returns>
        public bool CheckAddMemo(StorageLocation location, object data, out T memo)
        {
            memo = DefaultMemoValue;
            var haveMemo = CheckMemo(location, out memo);
            if (!haveMemo)
            {
                memo = GetMemo(location, data);
                if (IsValidMemo(memo))
                {
                    haveMemo = AddMemo(location, memo);
                }
            }
            return haveMemo;
        }

        /// <summary>
        /// Implementations provide this method to create the value for the memo.
        /// </summary>
        /// <param name="location">Location of the file for which the memo is to be created.</param>
        /// <param name="data">Implementation-defined data used for memo creation.</param>
        /// <returns>The value for the memo.</returns>
        protected abstract T GetMemo(StorageLocation location, object data);

        /// <summary>
        /// Implementations provide this method to report whether the given memo value is valid.
        /// </summary>
        /// <param name="memo">The memo value to verify.</param>
        /// <returns><c>true</c> if the value of <paramref name="memo"/> is valid; <c>false</c> otherwise.</returns>
        protected abstract bool IsValidMemo(T memo);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [System.Diagnostics.Conditional("ENABLE_DEBUG_REPORTING")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        [System.Diagnostics.Conditional("ENABLE_DEBUG_REPORTING")]
        private void PrintKeys()
        {
            var keys = _memos.Keys.OrderBy(k => k.Path, StringComparer.OrdinalIgnoreCase);
            foreach (var key in keys)
            {
                System.Diagnostics.Debug.WriteLine(key.Path);
            }
        }
    }
}
