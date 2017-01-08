// <copyright file="PrescaleMode.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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

namespace INTV.JzIntv.Model
{
    /// <summary>
    /// Common graphics prescale modes for the emulator.
    /// </summary>
    public enum PrescaleMode
    {
        /// <summary>No prescaling.</summary>
        None = 0,

        /// <summary>Scale by a factor of two.</summary>
        Scale2x,

        /// <summary>Scale by a factor of three.</summary>
        Scale3x,

        /// <summary>Scale by a factor of four.</summary>
        Scale4x,

        /// <summary>Rotate 180 degrees.</summary>
        Rotate180,
    }
}
