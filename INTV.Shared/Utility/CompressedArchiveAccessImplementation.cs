// <copyright file="CompressedArchiveAccessImplementation.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Different compressed archive implementations to choose from. Not all archive
    /// formats are necessarily compressed.
    /// </summary>
    public enum CompressedArchiveAccessImplementation
    {
        /// <summary>
        /// Do not provide access to any compression / archive formats.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use native-to-.NET implementations, or home-grown versions.
        /// </summary>
        Native,

        /// <summary>
        /// Use SharpZipLib, cloned from: https://github.com/icsharpcode/SharpZipLib
        /// </summary>
        SharpZipLib,

        /// <summary>
        /// Some other, custom implementation.
        /// </summary>
        Other,

        /// <summary>
        /// Use the default compression / archive implementation.
        /// </summary>
        Default = SharpZipLib,

        /// <summary>
        /// Use any available implementation.
        /// </summary>
        Any = -1
    }
}
