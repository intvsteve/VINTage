// <copyright file="RandomUtilities.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.Core.Utility
{
    /// <summary>
    /// This class provides some basic utilities to easily generate pseudo-random values of various sizes.
    /// </summary>
    public static class RandomUtilities
    {
        private static Random _random = new Random();

        /// <summary>
        /// Generate a random 32-bit uint value.
        /// </summary>
        /// <returns>A pseudo-random 32-bit uint value.</returns>
        public static uint Next32()
        {
            byte[] bytes = new byte[4];
            _random.NextBytes(bytes);
            uint value = BitConverter.ToUInt32(bytes, 0);
            return value;
        }

        /// <summary>
        /// Generate a random 24-bit uint value.
        /// </summary>
        /// <returns>A pseudo-random 24-bit uint value.</returns>
        public static uint Next24()
        {
            byte[] bytes = new byte[3];
            _random.NextBytes(bytes);
            var intBuffer = new byte[4];
            bytes.CopyTo(intBuffer, 0);
            uint value = BitConverter.ToUInt32(intBuffer, 0);
            return value;
        }

        /// <summary>
        /// Generate a random ushort value.
        /// </summary>
        /// <returns>A pseudo-random ushort value.</returns>
        public static ushort Next16()
        {
            byte[] bytes = new byte[2];
            _random.NextBytes(bytes);
            ushort value = BitConverter.ToUInt16(bytes, 0);
            return value;
        }
    }
}
