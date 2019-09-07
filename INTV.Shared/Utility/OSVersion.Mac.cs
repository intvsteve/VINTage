// <copyright file="OSVersion.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class OSVersion
    {
        /// <summary>
        /// Gets the 'friendly' generic OS name.
        /// </summary>
        public static string Name
        {
            get
            {
                var osName = "OS X";
                if (Current.Minor > 11)
                {
                    osName = "macOS";
                }
                return osName;
            }
        }

        private static OSVersion Initialize()
        {
            // Don't have support for NSOperatingSystemVersion yet. It's only available in
            // OS X 10.10 and later. *sigh*

            // For reference, on Mac, Environment.OSVersion effectively returns the same value as sysctl kern.osrelease.
            // To map that to the well-known Mac releases, here's a handy dandy partial table from Stack Overflow:
            // See: https://stackoverflow.com/questions/11072804/how-do-i-determine-the-os-version-at-runtime-in-os-x-or-ios-without-using-gesta
            // 17.x.x. macOS 10.13.x High Sierra
            // 16.x.x  macOS 10.12.x Sierra
            // 15.x.x  OS X  10.11.x El Capitan
            // 14.x.x  OS X  10.10.x Yosemite
            // 13.x.x  OS X  10.9.x  Mavericks
            // 12.x.x  OS X  10.8.x  Mountain Lion
            // 11.x.x  OS X  10.7.x  Lion
            // 10.x.x  OS X  10.6.x  Snow Leopard
            var versionString = NSProcessInfo.ProcessInfo.OperatingSystemVersionString;
            int foundPartNumber = 0;
            const int NumVersionParts = 3;
            int major = 0;
            int minor = 0;
            int patch = 0;
            var versionParts = System.Text.RegularExpressions.Regex.Split(versionString, @"\D+");
            foreach (var versionPart in versionParts)
            {
                int versionNumber;
                if (int.TryParse(versionPart, out versionNumber))
                {
                    switch (foundPartNumber)
                    {
                        case 0:
                            major = versionNumber;
                            break;
                        case 1:
                            minor = versionNumber;
                            break;
                        case 2:
                            patch = versionNumber;
                            break;
                    }
                    ++foundPartNumber;
                    if (foundPartNumber >= NumVersionParts)
                    {
                        break;
                    }
                }
            }
            return new OSVersion(major, minor, patch);
        }
    }
}
