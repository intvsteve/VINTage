// <copyright file="FileMemo`T.cs" company="INTV Funhouse">
// Copyright (c) 2017-2018 All Rights Reserved
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
        private readonly ConcurrentDictionary<string, Tuple<long, DateTime, T>> _memos = new System.Collections.Concurrent.ConcurrentDictionary<string, Tuple<long, DateTime, T>>(StringComparer.OrdinalIgnoreCase);

        protected FileMemo(IStorageAccess storageAccess)
        {
            StorageAccess = storageAccess;
        }

        protected IStorageAccess StorageAccess { get; private set; }

        /// <summary>
        /// Gets the default value for a memo. Note that this is necessary because the default value many not be the same as default(T).
        /// </summary>
        protected abstract T DefaultMemoValue { get; }

        /// <summary>
        /// Checks for a pre-existing memo.
        /// </summary>
        /// <param name="filePath">Path to the file for which the memo is desired.</param>
        /// <param name="memo">The stored value for the memo. If no memo is available, <see cref="DefaultMemoValue"/> is returned.</param>
        /// <returns><c>true</c> if a memo for <paramref name="filePath"/> was found, <c>false</c> otherwise.</returns>
        /// <remarks>In addition to checking for the memo, if the given file does not exist, any existing memo for it will be removed.</remarks>
        public bool CheckMemo(string filePath, out T memo)
        {
            memo = DefaultMemoValue;
            var foundMemo = false;
            Tuple<long, DateTime, T> memorandum;
            if (!IStorageAccessHelpers.FileExists(filePath, StorageAccess))
            {
                if (_memos.TryRemove(filePath, out memorandum))
                {
                    DebugOutput("CheckMemo removed: " + filePath + ": size: " + memorandum.Item1 + ", mod: " + memorandum.Item2 + ", memo: " + memorandum.Item3);
                }
            }
            else
            {
                foundMemo = _memos.TryGetValue(filePath, out memorandum) && (IStorageAccessHelpers.FileSize(filePath, StorageAccess) == memorandum.Item1) && (IStorageAccessHelpers.LastFileWriteTimeUtc(filePath, StorageAccess) == memorandum.Item2);
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
        /// <param name="filePath">Path to the file for which the memo is to be stored.</param>
        /// <param name="memo">The memo to store. See remarks.</param>
        /// <returns><c>true</c> if the memo was added; <c>false</c> otherwise.</returns>
        /// <remarks>The value of <paramref name="memo"/> is validated via a call to <see cref="IsValidMemo"/>. If the memo is not valid,
        /// the existing memo, if any, will be removed.</remarks>
        public bool AddMemo(string filePath, T memo)
        {
#if ENABLE_MEMOS
            var added = IsValidMemo(memo) && IStorageAccessHelpers.FileExists(filePath, StorageAccess);
            if (added)
            {
                _memos[filePath] = new Tuple<long, DateTime, T>(IStorageAccessHelpers.FileSize(filePath, StorageAccess), IStorageAccessHelpers.LastFileWriteTimeUtc(filePath, StorageAccess), memo);
            }
            else
            {
                Tuple<long, DateTime, T> memorandum;
                if (_memos.TryRemove(filePath, out memorandum))
                {
                   DebugOutput("AddMemo removed: " + filePath + ": size: " + memorandum.Item1 + ", mod: " + memorandum.Item2 + ", memo: " + memorandum.Item3);
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
        /// <param name="filePath">Path to the file for which the memo is to be retrieved, or possibly stored.</param>
        /// <param name="data">Implementation-specific data used for the creation of the memo if a value is needed.</param>
        /// <param name="memo">Receives the value of the memo, either already stored, or newly generated and stored.</param>
        /// <returns><c>true</c> if a valid memo was either stored or created and stored.</returns>
        public bool CheckAddMemo(string filePath, object data, out T memo)
        {
            memo = DefaultMemoValue;
            var haveMemo = CheckMemo(filePath, out memo);
            if (!haveMemo)
            {
                memo = GetMemo(filePath, data);
                if (IsValidMemo(memo))
                {
                    haveMemo = AddMemo(filePath, memo);
                }
            }
            return haveMemo;
        }

        /// <summary>
        /// Implementations provide this method to create the value for the memo.
        /// </summary>
        /// <param name="filePath">Path to the file for which the memo is to be created.</param>
        /// <param name="data">Implementation-defined data used for memo creation.</param>
        /// <returns>The value for the memo.</returns>
        protected abstract T GetMemo(string filePath, object data);

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
            var keys = _memos.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase);
            foreach (var key in keys)
            {
                System.Diagnostics.Debug.WriteLine(key);
            }
        }
    }
}
