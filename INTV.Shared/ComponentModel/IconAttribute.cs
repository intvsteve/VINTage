// <copyright file="IconAttribute.cs" company="INTV Funhouse">
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

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Metadata attribute for an icon to associate with a type.
    /// </summary>
    [System.ComponentModel.Composition.MetadataAttribute]
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public sealed class IconAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of IconAttribute.
        /// </summary>
        /// <param name="icon">Name of the icon resource. Must be in the same assembly as the type to which this attribute is attached.</param>
        public IconAttribute(string icon)
        {
            Icon = icon;
        }

        /// <summary>
        /// Gets the icon resource name. Must be in same assembly as the type to which this attribute as attached.
        /// </summary>
        public string Icon { get; private set; }
    }
}
