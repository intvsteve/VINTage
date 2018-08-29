// <copyright file="IConverter`TSource`TDestination.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

namespace INTV.Core.Utility
{
    /// <summary>
    /// Basic interface used to implement a conversion from one data type to another.
    /// </summary>
    /// <typeparam name="TSource">Data type of the object to be converted.</typeparam>
    /// <typeparam name="TDestination">Data type of the object being convert to.</typeparam>
    public interface IConverter<TSource, TDestination>
    {
        /// <summary>
        /// Converts an instance of type TSource to an instance of TDestination.
        /// </summary>
        /// <param name="source">The instance to be converted.</param>
        /// <returns>The newly converted instance.</returns>
        TDestination Convert(TSource source);
    }
}
