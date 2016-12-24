// <copyright file="LuigiGenerationMode.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Describes how the PrepareForDeployment extension method should behave.
    /// </summary>
    public enum LuigiGenerationMode
    {
        /// <summary>
        /// Perform a standard LUIGI generation based on ROM and CFG CRC values.
        /// </summary>
        Standard,

        /// <summary>
        /// Force a LUIGI generation. This will explicitly assign feature flags and
        /// override whatever may already be present in the ROMsCache.
        /// </summary>
        FeatureUpdate,

        /// <summary>
        /// Force LUIGI generation. Only features from the original ROM will be used.
        /// </summary>
        Reset,

        /// <summary>
        /// If file is already in LUIGI format, pass it through without any processing.
        /// </summary>
        Passthrough
    }
}
