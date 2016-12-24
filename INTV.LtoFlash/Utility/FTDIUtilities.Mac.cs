// <copyright file="FTDIUtilities.Mac.cs" company="INTV Funhouse">
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
using System.Linq;

namespace INTV.LtoFlash.Utility
{
    /// <summary>
    /// Mac-specific implementation of the FTDI utilities.
    /// </summary>
    public static partial class FTDIUtilities
    {
        /// <summary>
        /// If the OS minor version is less than this, use the old version of the driver.
        /// </summary>
        internal const int UseNewDriverOSMinorVersion = 9;

        /// <summary>
        /// Gets the current version of the FTDI VCP driver.
        /// </summary>
        public static Version DriverVersion
        {
            get
            {
                var version = new Version();
                var versionString = DriverVersionString;
                if (!string.IsNullOrEmpty(versionString))
                {
                    version = new Version(versionString);
                }
                return version;
            }
        }

        /// <summary>
        /// Gets the driver version as a string.
        /// </summary>
        public static string DriverVersionString
        {
            get
            {
                var versionString = string.Empty; // change to resource for unknown?
                try
                {
                    var driverPlistPath = "/Library/Extensions/FTDIUSBSerialDriver.kext/Contents/Info.plist";
                    if (INTV.Shared.Utility.OSVersion.Current.Minor < UseNewDriverOSMinorVersion)
                    {
                        driverPlistPath = "/System" + driverPlistPath;
                    }
                    if (System.IO.File.Exists(driverPlistPath))
                    {
                        using (var plist = System.IO.File.OpenRead(driverPlistPath))
                        {
                            var plistXmlDocument = System.Xml.Linq.XDocument.Load(plist);
                            versionString = plistXmlDocument.Descendants("key").First(k => k.Value.Equals("CFBundleVersion", StringComparison.Ordinal)).ElementsAfterSelf().First().Value;
                        }
                    }
                }
                catch (Exception)
                {
                    // quietly fail if something goes wrong.
                }
                return versionString;
            }
        }
    }
}
