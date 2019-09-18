// <copyright file="PathComparer.cs" company="INTV Funhouse">
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
using System.IO;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Comparison implementation suitable for file paths.
    /// </summary>
    /// <remarks>Consider normalizing separators for ALL path comparisons.</remarks>
    public partial class PathComparer : IComparer<string>, IComparer, IEqualityComparer<string>, IEqualityComparer
    {
        /// <summary>
        /// We really only need one instance of this...
        /// </summary>
        public static readonly PathComparer Instance = new PathComparer();

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.PathComparer"/> class,
        /// using the default path comparison policy for the compile-time target platform.
        /// </summary>
        public PathComparer()
            : this(DefaultPolicy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.PathComparer"/> class.
        /// </summary>
        /// <param name="policy">The path comparison policy to use.</param>
        public PathComparer(StringComparison policy)
        {
            Policy = policy;
        }

        public StringComparison Policy { get; set; }

        #region IComparer<string> Members

        /// <inheritdoc />
        /// <remarks>NOTE: Always performs case insensitive path comparisons.</remarks>
        public int Compare(string x, string y)
        {
            return CompareCore(x, y, Policy);
        }

        #endregion // IComparer<string>

        #region IComparer

        /// <inheritdoc />
        /// <remarks>NOTE: Always performs case insensitive path comparisons.</remarks>
        public int Compare(object x, object y)
        {
            return Compare(x as string, y as string);
        }

        #endregion // IComparer

        #region IEqualityComparer<string>

        /// <inheritdoc />
        /// <remarks>NOTE: Always performs case insensitive path comparisons.</remarks>
        public bool Equals(string x, string y)
        {
            return Compare(x, y, Policy);
        }

        /// <inheritdoc />
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }

        #endregion // IEqualityComparer<string>

        #region IEqualityComparer

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            return Equals(x as string, y as string);
        }

        /// <inheritdoc />
        public int GetHashCode(object obj)
        {
            return GetHashCode(obj as string);
        }

        #endregion // IEqualityComparer

        /// <summary>
        /// Compares to strings as if they were paths.
        /// </summary>
        /// <param name="x">The first string to compare.</param>
        /// <param name="y">The second string to compare.</param>
        /// <param name="comparisonMode">How to perform string comparison.</param>
        /// <returns><c>true</c> if the two strings are the same based on comparisonMode,
        /// or if full paths compare as equal after converting to full paths via System.IO.Path.</returns>
        public bool Compare(string x, string y, StringComparison comparisonMode)
        {
            return CompareCore(x, y, comparisonMode) == 0;
        }

        private int CompareCore(string x, string y, StringComparison comparisonMode)
        {
            // TODO: Consider normalizing path separator in all circumstances.
            var result = string.Compare(x, y, comparisonMode);
            if (result != 0)
            {
                if (!string.IsNullOrWhiteSpace(x) && !string.IsNullOrWhiteSpace(y))
                {
                    result = string.Compare(
                        Path.GetFullPath(x).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                        Path.GetFullPath(y).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                        comparisonMode);
                }
            }
            return result;
        }
    }
}
