// <copyright file="LocalizedNameAttribute.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Metadata attribute to provide a string that can be defined as a resource.
    /// </summary>
    [System.ComponentModel.Composition.MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class LocalizedNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the attribute.
        /// </summary>
        /// <param name="resourceType">A type used to locate the assembly containing the string identified by a key.</param>
        /// <param name="resourceKey">The key in the 'Strings' resource in the assembly that defines the given type.</param>
        public LocalizedNameAttribute(Type resourceType, string resourceKey)
        {
            Name = resourceType.GetResourceString(resourceKey);
        }

        /// <summary>
        /// Gets the localized string.
        /// </summary>
        public string Name { get; private set; }
    }
}
