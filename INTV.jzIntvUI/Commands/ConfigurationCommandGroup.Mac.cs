// <copyright file="ConfigurationCommandGroup.Mac.cs" company="INTV Funhouse">
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

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif

namespace INTV.JzIntvUI.Commands
{
    /// <summary>
    /// MAC-specific implementation.
    /// </summary>
    public partial class ConfigurationCommandGroup
    {
        #region CommandGroup

        /// <inheritdoc />
        public override object Context
        {
            get { return null; }
        }

        #endregion // CommandGroup

        private static string OSResolvePathFromSettings(string path)
        {
            var url = NSUrl.FromString(path);
            var resolvedFilePath = url.FilePathUrl;
            var resolvedPath = resolvedFilePath.AbsoluteString;
            return resolvedPath;
        }

        private static string ResolvePathForSettings(string path)
        {
            var url = NSUrl.FromFilename(path);
            path = url.AbsoluteString;
            return path;
       }
    }
}
