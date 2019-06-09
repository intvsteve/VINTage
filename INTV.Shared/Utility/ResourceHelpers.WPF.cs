// <copyright file="ResourceHelpers.WPF.cs" company="INTV Funhouse">
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
using System.Windows.Media;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public static partial class ResourceHelpers
    {
        /// <summary>
        /// Creates a properly formatted string to locate a resource in an assembly.
        /// </summary>
        /// <param name="type">Any object whose implementation is in the assembly in which the resource is supposed to exist.</param>
        /// <param name="relativeResourcePath">The relative path to the resource within the type's assembly.</param>
        /// <returns>The packed resource string suitable to locate the resource.</returns>
        public static string CreatePackedResourceString(this Type type, string relativeResourcePath)
        {
            if (relativeResourcePath.StartsWith("pack://application:,,,/"))
            {
                return relativeResourcePath; // already in packed format
            }
            var assembly = type.Assembly;
            var assemblyName = new System.Reflection.AssemblyName(assembly.FullName);
            var resourceString = "pack://application:,,,/" + assemblyName.Name + ";component/" + relativeResourcePath;
            return resourceString;
        }

        /// <summary>
        /// Loads an image resource from an assembly.
        /// </summary>
        /// <param name="type">Any object whose implementation is in the assembly in which the image resource is supposed to exist.</param>
        /// <param name="relativeResourcePath">The relative path to the image resource within the type's assembly.</param>
        /// <returns>The image.</returns>
        public static ImageSource LoadImageResource(this Type type, string relativeResourcePath)
        {
            var resourceName = type.CreatePackedResourceString(relativeResourcePath);
            var imageUri = new System.Uri(resourceName, System.UriKind.RelativeOrAbsolute);
            var image = new System.Windows.Media.Imaging.BitmapImage(imageUri);
            return image;
        }
    }
}
