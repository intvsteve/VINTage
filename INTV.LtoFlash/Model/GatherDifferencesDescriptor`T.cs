// <copyright file="GatherDifferencesDescriptor`T.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This delegate is used to compare to instances of a LFS file system entry.
    /// </summary>
    /// <typeparam name="T">Type of file system entry being compared.</typeparam>
    /// <param name="lhs">The 'left hand' side of the comparison.</param>
    /// <param name="rhs">The 'right hand' side of the comparison.</param>
    /// <param name="failedCompareEntryName">The name of the file system entry, set if an error occurred.</param>
    /// <param name="error">If set to non-<c>null</c> value, indicates an error occurred during the comparison.</param>
    /// <returns><c>true</c> if <paramref name="lhs"/> is considered equal to <paramref name="rhs"/>.</returns>
    public delegate bool FileSystemEntryComparer<T>(T lhs, T rhs, out string failedCompareEntryName, out Exception error) where T : class, IGlobalFileSystemEntry;

    /// <summary>
    /// This delegate is used to validate an entry for compatibility with the supplied device.
    /// </summary>
    /// <typeparam name="T">Type of file system entry being validated.</typeparam>
    /// <param name="entry">The entry to validate.</param>
    /// <param name="targetDevice">The device against which the entry is to be validated.</param>
    /// <param name="failedValidationEntryName">The name of the file system entry, set if validation failed.</param>
    /// <param name="error">If set to non-<c>null</c> value, indicates an error occurred during validation.</param>
    /// <returns><c>true</c> if the entry is valid.</returns>
    public delegate bool FileSystemEntryValidator<T>(T entry, Device targetDevice, out string failedValidationEntryName, out Exception error) where T : class, IGlobalFileSystemEntry;

    /// <summary>
    /// This class stores a description of how to generate the differences between two Locutus file system tables.
    /// </summary>
    /// <typeparam name="T">The type of file system entries in the table to be compared.</typeparam>
    public sealed class GatherDifferencesDescriptor<T> where T : class, IGlobalFileSystemEntry
    {
        /// <summary>
        /// Initializes a new instance of GatherDifferencesDescriptor.
        /// </summary>
        /// <param name="entityType">The type of the file system entities this descriptor works with.</param>
        /// <param name="targetFileSystemTable">The LFS file system table to be compared against a reference version.</param>
        /// <param name="comparer">The comparison function to use to determine if a reference and target file system entity are different.</param>
        public GatherDifferencesDescriptor(LfsEntityType entityType, FixedSizeCollection<T> targetFileSystemTable, FileSystemEntryComparer<T> comparer)
            : this(entityType, targetFileSystemTable, comparer, AlwaysValid)
        {
        }

        /// <summary>
        /// Initializes a new instance of GatherDifferencesDescriptor.
        /// </summary>
        /// <param name="entityType">The type of the file system entities this descriptor works with.</param>
        /// <param name="targetFileSystemTable">The LFS file system table to be compared against a reference version.</param>
        /// <param name="comparer">The comparison function to use to determine if a reference and target file system entity are different.</param>
        /// <param name="validator">The validation function to use to determine if an entry is valid.</param>
        public GatherDifferencesDescriptor(LfsEntityType entityType, FixedSizeCollection<T> targetFileSystemTable, FileSystemEntryComparer<T> comparer, FileSystemEntryValidator<T> validator)
        {
            TargetFileSystemTable = targetFileSystemTable;
            Compare = comparer;
            Validate = validator ?? AlwaysValid;
            EntityType = entityType;
        }

        /// <summary>
        /// Gets the Locutus file system table to compare against.
        /// </summary>
        public FixedSizeCollection<T> TargetFileSystemTable { get; private set; }

        /// <summary>
        /// Gets the comparison function to use for equality checking.
        /// </summary>
        public FileSystemEntryComparer<T> Compare { get; private set; }

        /// <summary>
        /// Gets the validation function to use when validating an entry.
        /// </summary>
        public FileSystemEntryValidator<T> Validate { get; private set; }

        /// <summary>
        /// Gets the type of LFS entities involved operated upon by this descriptor.
        /// </summary>
        public LfsEntityType EntityType { get; private set; }

        private static bool AlwaysValid(T entry, Device targetDevice, out string failedValidationEntryName, out Exception error)
        {
            failedValidationEntryName = null;
            error = null;
            return true;
        }
    }
}
