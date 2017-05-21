// <copyright file="OSVersion.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
        private static System.Version Initialize()
        {
            // Don't have support for NSOperatingSystemVersion yet. It's only available in
            // OS X 10.10 and later. *sigh*
            var versionString = NSProcessInfo.ProcessInfo.OperatingSystemVersionString;
            int foundPartNumber = 0;
            const int numVersionParts = 3;
            int major = 0;
            int minor = 0;
            int patch = 0;
            var versionParts = System.Text.RegularExpressions.Regex.Split(versionString, @"\D+");
            foreach (var versionPart in versionParts)
            {
                int versionNumber;
                if (int.TryParse(versionPart, out versionNumber))
                {
                    switch(foundPartNumber)
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
                    if (foundPartNumber >= numVersionParts)
                    {
                        break;
                    }
                }
            }
            var version = new System.Version(major, minor, patch);
            return version;
        }
    }
}
