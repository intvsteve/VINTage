// <copyright file="ProgramIdentifier.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

using System.Globalization;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// A unique identifier for an Intellivision program's ROM.
    /// </summary>
    public struct ProgramIdentifier
    {
        /// <summary>
        /// Helper value used to identify an invalid <see cref="ProgramIdentifier"/>.
        /// </summary>
        public static readonly ProgramIdentifier Invalid = new ProgramIdentifier();

        /// <summary>
        /// Initializes a new instance of <see cref="ProgramIdentifier"/>.
        /// </summary>
        /// <param name="romCrc">A CRC32 of the ROM.</param>
        /// <remarks>In this case, the lower 32 bits of the total <see cref="Id"/> will be zero.</remarks>
        public ProgramIdentifier(uint romCrc)
            : this(romCrc, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ProgramIdentifier"/>.
        /// </summary>
        /// <param name="romCrc">A CRC32 of the ROM.</param>
        /// <param name="otherData">Other data to help uniquely identify the ROM, e.g. a CRC32 of a .BIN-format ROM's .CFG file.</param>
        public ProgramIdentifier(uint romCrc, uint otherData)
            : this(((ulong)romCrc << 32) | otherData)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ProgramIdentifier"/>.
        /// </summary>
        /// <param name="uniqueIdentifier">The raw unique identifier value.</param>
        public ProgramIdentifier(ulong uniqueIdentifier)
        {
            _uniqueIdentifier = uniqueIdentifier;
        }

        /// <summary>
        /// Gets an unsigned 64-bit integral representation of the program's unique identifier.
        /// </summary>
        public ulong Id
        {
            get { return _uniqueIdentifier; }
        }
        private readonly ulong _uniqueIdentifier;

        /// <summary>
        /// Gets the ROM's CRC32 value -- ideally just that of the executable code.
        /// </summary>
        public uint DataCrc
        {
            get { return (uint)(Id >> 32); }
        }

        /// <summary>
        /// Gets the low 32-bits of the identifier. This could be a CRC of a .CFG file, or other data.
        /// </summary>
        public uint OtherData
        {
            get { return (uint)(Id & 0xFFFFFFFF); }
        }

        #region Operators

        /// <summary>
        /// Equality operator for <see cref="ProgramIdentifier"/>.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns><c>true</c> if the values are equal</returns>
        public static bool operator ==(ProgramIdentifier lhs, ProgramIdentifier rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Inequality operator for <see cref="ProgramIdentifier"/>.
        /// </summary>
        /// <param name="lhs">Left-hand side value.</param>
        /// <param name="rhs">Right-hand side value.</param>
        /// <returns><c>true</c> if the values are not equal</returns>
        public static bool operator !=(ProgramIdentifier lhs, ProgramIdentifier rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Converts an unsigned integer value to a <see cref="ProgramIdentifier"/> whose <see cref="ProgramIdentifier.OtherData"/> value is zero.
        /// </summary>
        /// <param name="romCrc">ROM program CRC32 value.</param>
        /// <returns>A new instance of <see cref="ProgramIdentifier"/> created from <paramref name="romCrc"/>.</returns>
        public static implicit operator ProgramIdentifier(uint romCrc)
        {
            return new ProgramIdentifier(romCrc);
        }

        #endregion // Operators

        #region Object Overrides

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:X8},{1:X8}", DataCrc, OtherData);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var equal = (obj != null) && (GetType() == obj.GetType()) && (_uniqueIdentifier == ((ProgramIdentifier)obj)._uniqueIdentifier);
            return equal;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _uniqueIdentifier.GetHashCode();
        }

        #endregion // Object Overrides
    }
}
