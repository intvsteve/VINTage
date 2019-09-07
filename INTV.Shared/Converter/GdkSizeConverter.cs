// <copyright file="GdkSizeConverter.cs" company="INTV Funhouse">
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
using System.Globalization;

namespace INTV.Shared.Converter
{
    /// <summary>
    /// A simple <see cref="TypeConverter"/> for <see cref="Gdk.Size"/>.
    /// </summary>
    public sealed class GdkSizeConverter : TypeConverter
    {
        private TypeConverter _intConverter = TypeDescriptor.GetConverter(typeof(int));

        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var parts = ((string)value).Replace("(", string.Empty).Replace(")", string.Empty).Split(new[] { ',' });
                var width = (int)_intConverter.ConvertFromString(context, culture, parts[0]);
                var height = (int)_intConverter.ConvertFromString(context, culture, parts[1]);
                return new Gdk.Size(width, height);
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var size = (Gdk.Size)value;
                var s = $"({_intConverter.ConvertToString(context, culture, size.Width)},{_intConverter.ConvertToString(context, culture, size.Height)})";
                return s;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
