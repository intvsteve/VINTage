// <copyright file="RunExternalProgram.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public static partial class RunExternalProgram
    {
        static partial void VerifyIsExecutable(string programPath)
        {
            var unixFileInfo = new Mono.Unix.UnixFileInfo(programPath);
            if (!unixFileInfo.CanAccess(Mono.Unix.Native.AccessModes.X_OK))
            {
                var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.NoExecutePermissions_Format, programPath);
                throw new System.InvalidOperationException(message);
            }
        }

        private static void OSOpenFileInDefaultProgram(string filePath, string uriScheme)
        {
            var success = false;
            if (uriScheme == System.Uri.UriSchemeFile)
            {
                success = NSWorkspace.SharedWorkspace.OpenFile(filePath);
            }
            else
            {
                var url = NSUrl.FromString(filePath);
                success = NSWorkspace.SharedWorkspace.OpenUrl(url);
            }
            if (!success)
            {
                var message = string.Format("Failed to open: {0}", filePath);
                throw new System.InvalidOperationException(message);
            }
        }

        private static void OSSendEmail(string emailString)
        {
            var url = NSUrl.FromString(emailString);
            var success = NSWorkspace.SharedWorkspace.OpenUrl(url);
            if (!success)
            {
                var message = string.Format("Failed to send mail: {0}", emailString);
                throw new System.InvalidOperationException(message);
            }
        }
    }
}
