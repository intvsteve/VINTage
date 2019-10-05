// <copyright file="CompressedArchiveAccess.CompressedArchiveIdentifier.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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

namespace INTV.Shared.CompressedArchiveAccess
{
    /// <summary>
    /// Implementation of <see cref="CompressedArchiveIdentifier"/>.
    /// </summary>
    public abstract partial class CompressedArchiveAccess
    {
        private struct CompressedArchiveIdentifier : IEqualityComparer<CompressedArchiveIdentifier>, IComparable<CompressedArchiveIdentifier>
        {
            /// <summary>
            /// Initializes a new instance of <see cref="CompressedArchiveIdentifier"/>.
            /// </summary>
            /// <param name="format">The compressed archive format to use in the identifier.</param>
            /// <param name="implementation">The compressed archive access implementation kind to use in the identifier.</param>
            public CompressedArchiveIdentifier(CompressedArchiveFormat format, CompressedArchiveAccessImplementation implementation)
            {
                _format = format;
                _implementation = implementation;
            }

            /// <summary>
            /// Gets the format used in the identifier.
            /// </summary>
            public CompressedArchiveFormat Format
            {
                get { return _format; }
            }
            private CompressedArchiveFormat _format;

            /// <summary>
            /// Gets the implementation used in the identifier.
            /// </summary>
            public CompressedArchiveAccessImplementation Implementation
            {
                get { return _implementation; }
            }
            private CompressedArchiveAccessImplementation _implementation;

            /// <inheritdoc />
            public int CompareTo(CompressedArchiveIdentifier other)
            {
                var result = Format - other.Format;
                if (result == 0)
                {
                    if (Implementation != CompressedArchiveAccessImplementation.Any)
                    {
                        if (other.Implementation != CompressedArchiveAccessImplementation.Any)
                        {
                            result = Implementation - other.Implementation;
                        }
                    }
                }
                return result;
            }

            /// <inheritdoc />
            public bool Equals(CompressedArchiveIdentifier x, CompressedArchiveIdentifier y)
            {
                return x.CompareTo(y) == 0;
            }

            /// <inheritdoc />
            public int GetHashCode(CompressedArchiveIdentifier obj)
            {
                return obj.Format.GetHashCode();
            }
        }
    }
}
