// <copyright file="RomComparer.cs" company="INTV Funhouse">
// Copyright (c) 2015-2017 All Rights Reserved
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

////#define REPORT_PERFORMANCE

//// #define CANONICAL_COMPARE_SUPPORTED

using System;
using System.Collections;
using System.Collections.Generic;
using INTV.Core.Model.Program;

namespace INTV.Core.Model
{
    /// <summary>
    /// Provides a common base implementation for various IRom comparison implementations.
    /// </summary>
    public abstract class RomComparer : IComparer<IRom>, IComparer, IDisposable
    {
        /// <summary>
        /// The default ROM comparison mode.
        /// </summary>
        public const RomComparison DefaultCompareMode = RomComparison.Strict;

        ~RomComparer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the default comparison implementation for a given <see cref="RomComparison"/> mode.
        /// </summary>
        /// <param name="comparison">The desired comparison mode.</param>
        /// <returns>The default comparison implementation.</returns>
        public static RomComparer GetDefaultComparerForMode(RomComparison comparison)
        {
            RomComparer comparer = null;
            switch (comparison)
            {
                case RomComparison.Strict:
                    comparer = RomComparerStrict.Default;
                    break;
                case RomComparison.StrictRomCrcOnly:
                    comparer = RomComparerStrictCrcOnly.Default;
                    break;
#if CANONICAL_COMPARE_SUPPORTED
                case RomComparison.CanonicalStrict:
                    comparer = CanonicalRomComparerStrict.Default;
                    break;
                case RomComparison.CanonicalRomCrcOnly:
                    comparer = CanonicalRomComparer.Default;
                    break;
#endif // CANONICAL_COMPARE_SUPPORTED
                default:
                    throw new ArgumentException(string.Format(Resources.Strings.RomComparer_InvalidCompareRequestedFormat, comparison), "comparison");
            }
            return comparer;
        }

        /// <summary>
        /// Creates an instance of a particular comparer for a given <see cref="RomComparison"/> mode.
        /// </summary>
        /// <param name="comparison">The desired comparison mode.</param>
        /// <returns>A new instance of the comparison implementation. Comparer-specific features may be used if so desired.</returns>
        public static RomComparer GetComparer(RomComparison comparison)
        {
            RomComparer comparer = null;
            switch (comparison)
            {
                case RomComparison.Strict:
                    comparer = new RomComparerStrict();
                    break;
                case RomComparison.StrictRomCrcOnly:
                    comparer = new RomComparerStrictCrcOnly();
                    break;
#if CANONICAL_COMPARE_SUPPORTED
                case RomComparison.CanonicalStrict:
                    comparer = new CanonicalRomComparerStrict();
                    break;
                case RomComparison.CanonicalRomCrcOnly:
                    comparer = new CanonicalRomComparer();
                    break;
#endif // CANONICAL_COMPARE_SUPPORTED
                default:
                    throw new ArgumentException(string.Format(Resources.Strings.RomComparer_InvalidCompareRequestedFormat, comparison), "comparison");
            }
            return comparer;
        }

        #region IComparer

        /// <inheritdoc />
        public int Compare(object x, object y)
        {
            return Compare(x as IRom, null, y as IRom, null);
        }

        #endregion // IComparer

        #region IComparer<IRom>

        /// <inheritdoc />
        public int Compare(IRom x, IRom y)
        {
            var result = -1;
#if REPORT_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // REPORT_PERFORMANCE
            result = Compare(x, null, y, null);
#if REPORT_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine("ROM comparer: " + GetType().Name + "; result: " + result + "; duration: " + stopwatch.Elapsed + "; x: " + ((x == null) ? "<null>" : x.RomPath) + ", y: " + ((y == null) ? "<null>" : y.RomPath));
            }
#endif // REPORT_PERFORMANCE
            return result;
        }

        #endregion // IComparer<IRom>

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable

        /// <summary>
        /// Compare two ROM instances.
        /// </summary>
        /// <param name="x">The first ROM to compare.</param>
        /// <param name="programInformationRomX">Additional information about the <paramref name="x"/> ROM.</param>
        /// <param name="y">The second ROM to compare.</param>
        /// <param name="programInformationRomY">Additional information about the <paramref name="y"/> ROM.</param>
        /// <returns>If the two ROMs are considered equal, zero; if <paramref name="x"/> is considered 'greater than' <paramref name="y"/>, a positive number;
        /// if <paramref name="x"/> is considered 'less than' <paramref name="y"/>, a negative number.</returns>
        public abstract int Compare(IRom x, IProgramInformation programInformationRomX, IRom y, IProgramInformation programInformationRomY);

        /// <summary>
        /// The dispose pattern, used to clean up any additional resources that may be used during the comparison operation.
        /// </summary>
        /// <param name="disposing"><c>true</c> if IDisposable.Dispose() is called directly, or <c>false</c> if called from the Finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
