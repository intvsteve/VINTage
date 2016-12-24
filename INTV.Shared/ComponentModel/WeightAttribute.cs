// <copyright file="WeightAttribute.cs" company="INTV Funhouse">
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

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Metadata attribute to define the 'weight' of an item that may appear in a sorted list.
    /// </summary>
    [System.ComponentModel.Composition.MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class WeightAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the attribute.
        /// </summary>
        /// <param name="weight">A value in the range [0, 1], where values closer to one will result in the associated
        /// item being placed nearer the end of the list. Values closer to zero will put it toward the beginning.</param>
        public WeightAttribute(double weight)
        {
            Weight = weight;
        }

        /// <summary>
        /// Gets the weight of the associated item.
        /// </summary>
        public double Weight { get; private set; }
    }
}
