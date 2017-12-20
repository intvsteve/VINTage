// <copyright file="FTDIUtilities.Linux.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

namespace INTV.LtoFlash.Utility
{
    /// <summary>
    /// Linux-specific implementation to get FTDI Driver version.
    /// </summary>
    public static partial class FTDIUtilities
    {

        /// <summary>
        /// Gets the FTDI driver version as a string.
        /// </summary>
        public static string DriverVersionString
        {
            get
            {
                // TODO: No idea how to do this.
                var versionString = "?.?.?.?";
                return versionString;
            }
        }

        /// <summary>
        /// Gets the FTDI driver version.
        /// </summary>
        public static Version DriverVersion
        {
            get
            {
                // TODO: No idea how to do this.
                var version = new Version();
                return version;
            }
        }
    }
}
