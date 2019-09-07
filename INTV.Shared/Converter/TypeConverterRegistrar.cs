// <copyright file="TypeConverterRegistrar.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using System.ComponentModel;
using System.Linq;

namespace INTV.Shared.Converter
{
    /// <summary>
    /// A handy utility to register custom type converters.
    /// </summary>
    /// <remarks>Inspired by <see href="http://www.kinlan.co.uk/2006/10/assigining-typeconverter-to-class-you.html"/>.</remarks>
    public static class TypeConverterRegistrar
    {
        /// <summary>
        /// Registers a <see cref="TypeConverterAttribute"/> for a type.
        /// </summary>
        /// <typeparam name="T">The type to which a type converter attribute is added.</typeparam>
        /// <typeparam name="TC">The type of the specific <see cref="TypeConverter"/> implementation to associate with the type..</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if the type <typeparamref name="T"/> already has a <see cref="TypeConverterAttribute"/> associated with it.</exception>
        public static void RegisterAttribute<T, TC>() where TC : TypeConverter, new()
        {
            var attributes = TypeDescriptor.GetAttributes(typeof(T)).Cast<Attribute>();
            if (!attributes.OfType<TypeConverterAttribute>().Any())
            {
                var typeConverterAttribute = new TypeConverterAttribute(typeof(TC));
                TypeDescriptor.AddAttributes(typeof(T), typeConverterAttribute);
            }
            else
            {
                throw new InvalidOperationException("A TypeConverterAttribute has already been assigned to the type: " + typeof(T).FullName);
            }
        }

        /// <summary>
        /// Tries to register a <see cref="TypeConverterAttribute"/> for a type.
        /// </summary>
        /// <typeparam name="T">The type to which a type converter attribute is added.</typeparam>
        /// <typeparam name="TC">The type of the specific <see cref="TypeConverter"/> implementation to associate with the type..</typeparam>
        /// <returns><c>true</c>, if registeration of the converter attribute was successful, <c>false</c> otherwise.</returns>
        public static bool TryRegisterAttribute<T, TC>() where TC : TypeConverter, new()
        {
            var registered = false;
            try
            {
                RegisterAttribute<T, TC>();
                registered = true;
            }
            catch (InvalidOperationException)
            {
            }
            return registered;
        }
    }
}
