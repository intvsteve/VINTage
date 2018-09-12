// <copyright file="ConvertToMultiple`T`TSource`TDestination.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;

namespace INTV.Core.Utility
{
    /// <summary>
    /// A base implementation of <see cref="IConvertToMultiple{TSource,TDestination}"/> that provides access to a default, singleton instance of the converter.
    /// </summary>
    /// <typeparam name="T">Data type of a class derived from this one.</typeparam>
    /// <typeparam name="TSource">Data type of the object to be converted.</typeparam>
    /// <typeparam name="TDestination">Data type of the object being convert to.</typeparam>
    public abstract class ConvertToMultiple<T, TSource, TDestination> : IConvertToMultiple<TSource, TDestination> where T : IConvertToMultiple<TSource, TDestination>, new()
    {
        private static readonly Lazy<T> ConverterInstance = new Lazy<T>(() => new T());

        /// <summary>
        /// Initializes new instance of <see cref="ConvertToMultiple{T,TSource,TDestination}"/>
        /// </summary>
        protected ConvertToMultiple()
        {
        }

        /// <summary>
        /// Gets the instance of the converter.
        /// </summary>
        public static T Instance
        {
            get { return ConverterInstance.Value; }
        }

        /// <inheritdoc />
        public abstract IEnumerable<TDestination> Convert(TSource source);
    }
}
